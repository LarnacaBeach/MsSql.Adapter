using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MsSql.Adapter.Generator.Helpers;
using MsSql.Adapter.Generator.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace MsSql.Adapter.Generator;

[Generator]
public class AdapterGenerator : IIncrementalGenerator
{
    private static readonly string MssqlAdapterAttributeName = nameof(MsSqlAdapterAttribute);
    private static readonly string MssqlAdapterAttributeNamespace = typeof(MsSqlAdapterAttribute).Namespace;
    public static readonly DiagnosticDescriptor GeneratorException = new(
        "AG0001",
        "Generator Exception",
        "{Message} {InnerExceptionMessage} {StackTrace}",
        "Usage",
        DiagnosticSeverity.Error,
        true);
    public static readonly DiagnosticDescriptor InvalidSymbol = new(
        "AG0002",
        "Invalid Symbol",
        "{Identifier} symbol could not be determined",
        "Usage",
        DiagnosticSeverity.Warning,
        true);
    public static readonly DiagnosticDescriptor InvalidOutputPath = new(
        "AG0003",
        "Invalid Output Path",
        "{Filepath} could not be generated, because output path could not be found",
        "Usage",
        DiagnosticSeverity.Warning,
        true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(IsSyntaxTargetForGeneration, GetSemanticTargetForGeneration)
            .Where(static m => m is not null)!;

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax c && c.AttributeLists.Count > 0 && IsPartial(c);
    }

    public static bool IsPartial(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax) is ITypeSymbol type && HasMssqlAdapterAttribute(type))
        {
            // return the class
            return classDeclarationSyntax;
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    public static bool HasMssqlAdapterAttribute(ISymbol type)
    {
        return type.GetAttributes()
                   .Any(a => a.AttributeClass?.Name == MssqlAdapterAttributeName &&
                             a.AttributeClass.ContainingNamespace.ToDisplayString() == MssqlAdapterAttributeNamespace);
    }

    static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        try
        { 
            var distinctClasses = classes.Distinct();
            var classesToGenerate = GetTypesToGenerate(compilation, distinctClasses, context);

            foreach (var classToGenerate in classesToGenerate)
            {
                var dbMeta = SourceGenerationHelper.GetDatabaseMeta(classToGenerate.ResultPath);

                // Add the generated code to the compilation
                context.AddSource($"{classToGenerate.Name}Requests.g.cs", SourceGenerationHelper.GetTemplateSource("Templates/Requests.sbncs", classToGenerate, dbMeta));
                context.AddSource($"{classToGenerate.Name}Responses.g.cs", SourceGenerationHelper.GetTemplateSource("Templates/Responses.sbncs", classToGenerate, dbMeta));
                context.AddSource($"I{classToGenerate.Name}Service.g.cs", SourceGenerationHelper.GetTemplateSource("Templates/IService.sbncs", classToGenerate, dbMeta));
                context.AddSource($"{classToGenerate.Name}ServiceBase.g.cs", SourceGenerationHelper.GetTemplateSource("Templates/ServiceBase.sbncs", classToGenerate, dbMeta));
                context.AddSource($"{classToGenerate.Name}Service.g.cs", SourceGenerationHelper.GetTemplateSource("Templates/Service.sbncs", classToGenerate, dbMeta));

                // generate javascript overrides
                AddFile(compilation, context, $"javascript/{classToGenerate.Name.ToLowerInvariant()}service_grpc_pb_overrides.d.ts", SourceGenerationHelper.GetTemplateSource("Templates/javascript/service_grpc_pb_overrides.d.sbntxt", classToGenerate, dbMeta, false));
                AddFile(compilation, context, $"javascript/{classToGenerate.Name.ToLowerInvariant()}service_grpc_pb_overrides.js", SourceGenerationHelper.GetTemplateSource("Templates/javascript/service_grpc_pb_overrides.sbntxt", classToGenerate, dbMeta, false));
                AddFile(compilation, context, $"javascript/{classToGenerate.Name.ToLowerInvariant()}service_pb_overrides.d.ts", SourceGenerationHelper.GetTemplateSource("Templates/javascript/service_pb_overrides.d.sbntxt", classToGenerate, dbMeta, false));
                AddFile(compilation, context, $"javascript/{classToGenerate.Name.ToLowerInvariant()}service_pb_overrides.js", SourceGenerationHelper.GetTemplateSource("Templates/javascript/service_pb_overrides.sbntxt", classToGenerate, dbMeta, false));
            }
        }
        catch (Exception ex)
        {
            // Report a diagnostic if an exception occurs while generating code; allows consumers to know what is going on
            context.ReportDiagnostic(Diagnostic.Create(GeneratorException, Location.None, ex.Message, ex.InnerException?.Message, ex.StackTrace));
            //System.Diagnostics.Debugger.Launch();
        }
    }

    static List<ClassToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        var classesToGenerate = new List<ClassToGenerate>();
        INamedTypeSymbol? classAttribute = compilation.GetTypeByMetadataName(typeof(MsSqlAdapterAttribute).FullName);

        if (classAttribute == null)
        {
            // nothing to do if this type isn't available
            return classesToGenerate;
        }

        foreach (var classDeclarationSyntax in classes)
        {
            // stop if we're asked to
            context.CancellationToken.ThrowIfCancellationRequested();

            SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
            {
                // report diagnostic, something went wrong
                context.ReportDiagnostic(Diagnostic.Create(InvalidSymbol, Location.None, classDeclarationSyntax.Identifier));
                continue;
            }

            var name = classSymbol.Name;
            string collectorResultPath = "obj/result.json";
            string optionsKey = "DalServiceOptions";
            string optionsConnectionStringKey = "ConnectionString";
            string optionsConnectionUserKey = "ConnectionUser";
            string optionsConnectionPasswordKey = "ConnectionPassword";
            var nameSpace = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToString();

            foreach (AttributeData attributeData in classSymbol.GetAttributes())
            {
                if (!classAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                {
                    continue;
                }

                foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
                {
                    var value = namedArgument.Value.Value;

                    if (value == null)
                    {
                        continue;
                    }

                    switch (namedArgument.Key)
                    {
                        case nameof(MsSqlAdapterAttribute.CollectorResultPath):
                            collectorResultPath = value.ToString();
                            break;
                        case nameof(MsSqlAdapterAttribute.OptionsKey):
                            optionsKey = value.ToString();
                            break;
                        case nameof(MsSqlAdapterAttribute.OptionsConnectionStringKey):
                            optionsConnectionStringKey = value.ToString();
                            break;
                        case nameof(MsSqlAdapterAttribute.OptionsConnectionUserKey):
                            optionsConnectionUserKey = value.ToString();
                            break;
                        case nameof(MsSqlAdapterAttribute.OptionsConnectionPasswordKey):
                            optionsConnectionPasswordKey = value.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }

            classesToGenerate.Add(new ClassToGenerate(
                name,
                nameSpace,
                compilation.Options.SourceReferenceResolver?.NormalizePath(collectorResultPath, null) ?? string.Empty,
                optionsKey,
                optionsConnectionStringKey,
                optionsConnectionUserKey,
                optionsConnectionPasswordKey
            ));
        }

        return classesToGenerate;
    }

    private static void AddFile(Compilation compilation, SourceProductionContext context, string filepath, string content)
    {
        var absolutePath = compilation.Options.SourceReferenceResolver?.NormalizePath($"obj/GeneratedFiles/{filepath}", null);

        if (absolutePath == null)
        {
            // report diagnostic, something went wrong
            context.ReportDiagnostic(Diagnostic.Create(InvalidOutputPath, Location.None, filepath));
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
        File.WriteAllText(absolutePath, content);
    }
}

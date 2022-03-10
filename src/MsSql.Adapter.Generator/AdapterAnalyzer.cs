using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MsSql.Adapter.Generator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AdapterAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor EnumerationMustBePartial
           = new("AA0001",
                 "Class must be partial",
                 "The enumeration '{0}' must be partial",
                 nameof(AdapterAnalyzer),
                 DiagnosticSeverity.Error,
                 true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
           = ImmutableArray.Create(EnumerationMustBePartial);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            if (!AdapterGenerator.HasMssqlAdapterAttribute(context.Symbol))
            {
                return;
            }
            var type = (INamedTypeSymbol)context.Symbol;

            foreach (var declaringSyntaxReference in type.DeclaringSyntaxReferences)
            {
                if (declaringSyntaxReference.GetSyntax() is not ClassDeclarationSyntax classDeclaration || AdapterGenerator.IsPartial(classDeclaration))
                {
                    continue;
                }

                var error = Diagnostic.Create(EnumerationMustBePartial, classDeclaration.Identifier.GetLocation(), type.Name);

                context.ReportDiagnostic(error);
            }
        }
    }
}

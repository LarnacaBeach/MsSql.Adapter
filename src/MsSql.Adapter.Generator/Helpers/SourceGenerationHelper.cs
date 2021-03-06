using MsSql.Adapter.Generator.Models;
using Scriban;
using Scriban.Runtime;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.IO;
using System;

namespace MsSql.Adapter.Generator.Helpers;

public static class SourceGenerationHelper
{
    private static readonly string ToolName = typeof(AdapterGenerator).Assembly.GetName().Name;
    private static readonly string ToolVersion = typeof(AdapterGenerator).Assembly.GetName().Version.ToString();
    private static string Header = "";

    public static string GetTemplateSource(string templatePath, ClassToGenerate classToGenerate, DatabaseMeta dbMeta, bool addNullableFlag = true)
    {
        var templateContent = GetContent(templatePath);
        var template = Template.Parse(templateContent);
        var context = new TemplateContext
        {
            MemberRenamer = member => member.Name,
            TemplateLoader = new TemplateLoader()
        };
        var scriptObject = new ScriptObject();

        scriptObject.Import(typeof(DalHelper), ScriptMemberImportFlags.Method, renamer: context.MemberRenamer);
        scriptObject.Import(classToGenerate, ScriptMemberImportFlags.Field, renamer: context.MemberRenamer);
        scriptObject.Add("DbMeta", dbMeta);


        // Add custom functions to be used in the template
        context.PushGlobal(scriptObject);

        var sb = new StringBuilder();

        sb.Append(GetHeader());

        if (addNullableFlag)
        {
            sb.AppendLine("#nullable enable");
        }

        // Add the generated code to the compilation
        sb.AppendLine(template.Render(context));
        
        return sb.ToString();
    }

    /// <summary>
    /// Implementation of this helper method adapted from:
    /// https://www.cazzulino.com/source-generators.html
    /// </summary>
    public static string GetContent(string relativePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var baseName = assembly.GetName().Name;
        var resourceName = relativePath
            .TrimStart('.')
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');

        using var stream = assembly.GetManifestResourceStream($"{baseName}.{resourceName}");

        if (stream == null)
        {
            throw new NotSupportedException("Unable to get embedded resource content, because the stream was null");
        }

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    public static DatabaseMeta GetDatabaseMeta(string resultPath)
    {
        var jsonContent = File.ReadAllText(resultPath);
        var meta = JsonSerializer.Deserialize<Collector.Types.DatabaseMeta>(jsonContent, new JsonSerializerOptions
        {
            IncludeFields = true,
        });

        if (meta == null)
        {
            throw new InvalidDataException($"{resultPath} could not be deserialized");
        }

        return new DatabaseMeta(meta);
    }

    public static string GetHeader()
    {
        if (Header.Length == 0)
        {
            var template = Template.Parse(@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by {{ ToolName }} {{ ToolVersion }}.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
");

            // Add the generated code to the compilation
            Header = template.Render(new { ToolName, ToolVersion }, member => member.Name);
        }

        return Header;
    }
}
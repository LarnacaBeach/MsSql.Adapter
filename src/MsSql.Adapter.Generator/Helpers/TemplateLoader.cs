using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System.Threading.Tasks;

namespace MsSql.Adapter.Generator.Helpers;

internal class TemplateLoader : ITemplateLoader
{
    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        return templateName;
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        return SourceGenerationHelper.GetContent(templatePath);
    }

    public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        return new ValueTask<string>(Load(context, callerSpan, templatePath));
    }
}
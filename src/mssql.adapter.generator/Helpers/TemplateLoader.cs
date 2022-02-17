using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace mssql.adapter.generator.helpers;

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
namespace MsSql.Adapter.Generator.Models;

public readonly struct ClassToGenerate
{
    public readonly string Name;
    public readonly string Namespace;
    public readonly string ResultPath;
    public readonly string OptionsKey;
    public readonly string OptionsConnectionStringKey;
    public readonly string OptionsConnectionUserKey;
    public readonly string OptionsConnectionPasswordKey;

    public ClassToGenerate(
        string name,
        string ns,
        string resultPath,
        string optionsKey,
        string optionsConnectionStringKey,
        string optionsConnectionUserKey,
        string optionsConnectionPasswordKey
    )
    {
        Name = name.EndsWith("Service") ? name.Substring(0, name.Length - 7) : name;
        Namespace = ns;
        ResultPath = resultPath;
        OptionsKey = optionsKey;
        OptionsConnectionStringKey = optionsConnectionStringKey;
        OptionsConnectionUserKey = optionsConnectionUserKey;
        OptionsConnectionPasswordKey = optionsConnectionPasswordKey;
    }
}
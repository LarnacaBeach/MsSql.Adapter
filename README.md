# MsSql.Adapter.Generator

![Build status](https://github.com/LarnacaBeach/MsSql.Adapter/actions/workflows/BuildAndPack.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/MsSql.Adapter.Generator.svg)](https://www.nuget.org/packages/MsSql.Adapter.Generator/)
[![License](https://img.shields.io/github/license/LarnacaBeach/MsSql.Adapter.svg)](https://github.com/LarnacaBeach/MsSql.Adapter/blob/main/LICENSE)


A Source Generator package that generates methods for a class, including associated request & response classes, based on the `result.json` file created by [dotnet-mssql-collector](https://github.com/LarnacaBeach/MsSql.Adapter/tree/main/src/MsSql.Adapter.Collector) tool.

> This source generator requires the .NET 6 SDK. You can target earlier frameworks like .NET Core 3.1 etc, but the _SDK_ must be at least 6.0.100

Add the package (and required dependencies) to your application using

```bash
dotnet add package MsSql.Adapter.Generator
dotnet add package MsSql.Adapter.Standard.Types
dotnet add package MsSql.Adapter.Utils
```

This adds the `<PackageReference>` to your project. You can additionally mark the generator package as `PrivateAsets="all"` and `ExcludeAssets="runtime"`.

> Setting `PrivateAssets="all"` means any projects referencing this one won't get a reference to the _MsSql.Adapter.Generator_ package. Setting `ExcludeAssets="runtime"` ensures the MsSql.Adapter.Generator.Attributes.dll_ file is not copied to your build output (it is not required at runtime).

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>Latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <!-- Add the package -->
    <PackageReference Include="MsSql.Adapter.Generator" Version="1.0.6" PrivateAssets="all" ExcludeAssets="runtime" />
  </ItemGroup>

</Project>
```

Adding the package will automatically add a marker attribute, `[MssqlAdapterAttribute]`, to your project.

To use the generator, add the `[MssqlAdapterAttribute]` attribute to a partial class. For example:

```csharp
namespace Test
{
    [MsSqlAdapter]
    public partial class DalService
    {
    }
}
```

A class which holds the configuration needs to also exist in the same namespace (class name and properties names can be set in the attribute):
```csharp
namespace Test
{
    [DataContract]
    public class DalServiceOptions
    {
        [DataMember(Order = 1)]
        public string ConnectionString { get; set; } = "";

        [DataMember(Order = 2)]
        public string? ConnectionUser { get; set; }

        [DataMember(Order = 3)]
        public string? ConnectionPassword { get; set; }
    }
}
```

This will generate an interface that can be used by the `protobuf-net.Grpc`. For example:

```csharp
[ServiceContract(Name = "Test.ExampleDatabase.ExampleDatabaseService")]
public interface IDalService
{

    Task<FirstStoredProcedureResponse> FirstStoredProcedure(FirstStoredProcedureRequest req);
}
```
Additional files that can be used in javascript to add support for nullable values are generated in `obj/GeneratedFiles/javascript/` folder.

For a boilerplate project which creates a gRPC service check [MsSql.Adapter](https://github.com/LarnacaBeach/mssql.adapter/tree/main/examples/mssql.adapter)

## Preserving usages of the `[MsSqlAdapter]` attribute

The `[MsSqlAdapter]` attribute is decorated with the `[Conditional]` attribute, so their usage will not appear in the build output of your project. If you use reflection at runtime on one of your `class`es, you will not find `[MsSqlAdapter]` in the list of custom attributes.

If you wish to preserve these attributes in the build output, you can define the `MSSQL_ADAPTER_USAGES` MSBuild variable. Note that this means your project will have a runtime-dependency on _MsSql.Adapter.Generator.Attributes.dll_ so you need to ensure this is included in your build output.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <!--  Define the MSBuild constant to preserve usages   -->
    <DefineConstants>MSSQL_ADAPTER_USAGES</DefineConstants>
  </PropertyGroup>

  <ItemGroup>
    <!-- Add the package -->
    <PackageReference Include="MsSql.Adapter.Generator" Version="1.0.6" PrivateAssets="all" />
    <!--              ☝ You must not exclude the runtime assets in this case -->
  </ItemGroup>

</Project>
```
# mssql.adapter.generator

A Source Generator package that generates methods for a class, including associated requests & response classes, based on the `results.json` file created by `dotnet-mssql-collector` tool.

> This source generator requires the .NET 6 SDK. You can target earlier frameworks like .NET Core 3.1 etc, but the _SDK_ must be at least 6.0.100

Add the package to your application using

```bash
dotnet add package mssql.adapter.generator
```

This adds a `<PackageReference>` to your project. You can additionally mark the package as `PrivateAsets="all"` and `ExcludeAssets="runtime"`.

> Setting `PrivateAssets="all"` means any projects referencing this one won't get a reference to the _mssql.adapter.generator_ package. Setting `ExcludeAssets="runtime"` ensures the _mssql.adapter.generator.attributes.dll_ file is not copied to your build output (it is not required at runtime).

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>Latest</LangVersion>
  </PropertyGroup>

  <!-- Add the package -->~~~~
  <PackageReference Include="mssql.adapter.generator" Version="1.0.0"
    PrivateAssets="all" ExcludeAssets="runtime" />
  <!-- -->

</Project>
```

Adding the package will automatically add a marker attribute, `[MssqlAdapterAttribute]`, to your project.

To use the generator, add the `[MssqlAdapterAttribute]` attribute to a partial class. For example:

```csharp
[MssqlAdapter]
public partial class DalService
{
}
```

This will generate an interface that can be used by the `protobuf-net.Grpc`. For example:

```csharp
[ServiceContract(Name = "mssql.adapter.ExampleDatabase.ExampleDatabaseService")]
public interface IDalService
{

    Task<FirstStoredProcedureResponse> FirstStoredProcedure(FirstStoredProcedureRequest req);
}
```
Additional files that can be used in javascript to add support for nullable values are generated in `obj/GeneratedFiles/javascript/` folder.

For a boilerplate project which creates a gRPC service check https://github.com/LarnacaBeach/mssql.adapter/tree/master/examples/mssql.adapter
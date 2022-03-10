# MsSql.Adapter.Collector

![Build status](https://github.com/LarnacaBeach/MsSql.Adapter/actions/workflows/BuildAndPack.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/MsSql.Adapter.Collector.svg)](https://www.nuget.org/packages/MsSql.Adapter.Collector/)
[![License](https://img.shields.io/github/license/LarnacaBeach/MsSql.Adapter.svg)](https://github.com/LarnacaBeach/MsSql.Adapter/blob/main/LICENSE)


This tool will collect information about all the stored procedures in a database and store the results in a `result.json` file.

## Use the tool as a global tool
```bash
dotnet tool install -g MsSql.Adapter.Collector

mssql-adapter-collector --connection "MultipleActiveResultSets=true; Application Name=SQLCollect
or; Data Source=localhost; Initial Catalog=my_database; User ID=test; Password=test"
```

## Use the tool as a local tool
```bash
dotnet new tool-manifest
dotnet tool install MsSql.Adapter.Collector

dotnet mssql-adapter-collector --connection "MultipleActiveResultSets=true; Application Name=SQLCollect
or; Data Source=localhost; Initial Catalog=my_database; User ID=test; Password=test"
```

## Available options
Command Line Argument | Environment Variable | Default Value | Description
--- | --- | --- | ---
--connection | SqlCollectorServiceOptions__ConnectionString | | The database connection string.
--user | SqlCollectorServiceOptions__ConnectionUser | | The database connection user.
--password | SqlCollectorServiceOptions__ConnectionPassword | | The database connection password.
--pattern | SqlCollectorServiceOptions__ProcedurePattern | (?i)(^prc__?)(?!.*internal).* | The pattern to use for identifying valid stored procedures.
--previous | SqlCollectorServiceOptions__PreviousResultFile | result_prev.json | The previous generated results, used to keep same order for members.
--output | SqlCollectorServiceOptions__ResultFile | result.json | The output file path relative to current working directory.
--skip-response | SqlCollectorServiceOptions__SkipOutputParams | false | Skip parsing response of stored procedures.
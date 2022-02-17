using Microsoft.AspNetCore.Server.Kestrel.Core;
using mssql.adapter;
using mssql.adapter.helpers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (args.Contains("--proto"))
{
    // generate proto definition and exit
    var schema = DalService.GetProto();
    var contentRoot = builder.Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
    var path = Path.Join(contentRoot, "protos", "dalservice.proto");

    File.WriteAllText(path, schema);

    Console.WriteLine($"Proto definitions dumped to {path}");

    return;
}

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureEndpointDefaults(listenOptions =>
    {
        // support grpc on http protocol
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

StartupHelper.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

StartupHelper.ConfigureMiddleware(app, app.Services);

app.Run();
﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProtoBuf.Grpc.Server;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MsSql.Adapter.helpers
{
    public static class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddGrpcHealthChecks()
                .AddAsyncCheck("", () =>
                {
                    return Task.FromResult(HealthCheckResult.Healthy());
                }, Array.Empty<string>());
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));

            services
                .Configure<DalServiceOptions>(configuration.GetSection(nameof(DalServiceOptions)))
                .AddOptions()
                .AddSingleton<DalService>();

            services.AddCodeFirstGrpc(config =>
            {
                config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });

            services.AddCodeFirstGrpcReflection();
        }

        public static void ConfigureMiddleware(IApplicationBuilder app, IServiceProvider services)
        {
            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions
            {
                DefaultEnabled = true
            });

            app.UseEndpoints(endpoints =>
            {
                // health checks
                endpoints.MapGrpcHealthChecksService();

                endpoints.MapGrpcService<DalService>();
                endpoints.MapCodeFirstGrpcReflectionService();
            });
        }
    }
}
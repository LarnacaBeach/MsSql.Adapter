using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MsSql.Collector
{
    internal class Program
    {
        private static Task<int> Main(string[] args)
        {
            var toolDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var workingDirectory = Directory.GetCurrentDirectory();
            var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();

            Console.WriteLine($@"
{asciiart}
tool directory: {toolDirectory}
work directory: {workingDirectory}
tool version  : {version.Major}.{version.Minor}.{version.Build}
            ");

            var service = GetService(workingDirectory, args);
            var rootCommand = new RootCommand {
                new Option<string?>("--connection", "The database connection string."),
                new Option<string?>("--user", "The database connection user."),
                new Option<string?>("--password", "The database connection password."),
                new Option<string?>("--pattern", "The pattern to use for identifying valid stored procedures."),
                new Option<string?>("--previous", "The previous generated results, used to keep same order for members."),
                new Option<string?>("--output", "The output file path relative to current working directory."),
                new Option<bool?>("--skip-response", "Skip parsing response of stored procedures."),
            };

            rootCommand.Description = "MsSql.Adapter.Collector";

            rootCommand.Handler = CommandHandler.Create(async () =>
            {
                var resp = await service.WriteDatabaseMetaToFile(workingDirectory);

                Console.WriteLine(resp.StatusMessage);

                if (resp.Fail())
                {
                    Environment.Exit(resp.StatusCode);
                }
            });

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args);
        }

        private static SqlCollectorService GetService(string workingDirectory, string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(workingDirectory)
                .AddUserSecrets<SqlCollectorServiceOptions>()
                .AddCommandLine(args, new Dictionary<string, string>()
                 {
                     { "--connection", $"{nameof(SqlCollectorServiceOptions)}:{nameof(SqlCollectorServiceOptions.ConnectionString)}" },
                     { "--user", $"{nameof(SqlCollectorServiceOptions)}:{nameof(SqlCollectorServiceOptions.ConnectionUser)}" },
                     { "--password", $"{nameof(SqlCollectorServiceOptions)}:{nameof(SqlCollectorServiceOptions.ConnectionPassword)}" },
                     { "--pattern", $"{nameof(SqlCollectorServiceOptions)}:{nameof(SqlCollectorServiceOptions.ProcedurePattern)}" },
                     { "--previous", $"{nameof(SqlCollectorServiceOptions)}:{nameof(SqlCollectorServiceOptions.PreviousResultFile)}" },
                     { "--output", $"{nameof(SqlCollectorServiceOptions)}:{nameof(SqlCollectorServiceOptions.ResultFile)}" },
                     { "--skip-response", $"{nameof(SqlCollectorServiceOptions)}:{nameof(SqlCollectorServiceOptions.SkipOutputParams)}" },
                 })
                .Build();
            var services = new ServiceCollection()
                .Configure<SqlCollectorServiceOptions>(configuration.GetSection(nameof(SqlCollectorServiceOptions)))
                .AddOptions()
                .AddSingleton<SqlCollectorService>()
                .BuildServiceProvider(true);

            return services.GetRequiredService<SqlCollectorService>();
        }

        private static string asciiart = @"
  __  __    ___       _      _      _           _                ___     _ _        _           
 |  \/  |__/ __| __ _| |___ /_\  __| |__ _ _ __| |_ ___ _ _ ___ / __|___| | |___ __| |_ ___ _ _ 
 | |\/| (_-<__ \/ _` | |___/ _ \/ _` / _` | '_ \  _/ -_) '_|___| (__/ _ \ | / -_) _|  _/ _ \ '_|
 |_|  |_/__/___/\__, |_|  /_/ \_\__,_\__,_| .__/\__\___|_|      \___\___/_|_\___\__|\__\___/_|  
                   |_|                    |_|                                                   

";
    }
}
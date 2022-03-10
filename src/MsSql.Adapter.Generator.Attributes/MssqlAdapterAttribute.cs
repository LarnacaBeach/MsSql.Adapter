using System;

namespace MsSql.Adapter.Generator
{
    /// <summary>
    /// Add to a Class to indicate that adapter methods should be generated for it
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MsSqlAdapterAttribute : System.Attribute
    {
        /// <summary>
        /// The path to the result.json file created by dotnet-mssql-collector
        /// If not provided will try to load it from ./obj/result.json
        /// </summary>
        public string? CollectorResultPath { get; set; }

        /// <summary>
        /// The type used to retrieve service IOptions.
        /// If not provided will default to DalServiceOptions
        /// </summary>
        public string? OptionsKey { get; set; }

        /// <summary>
        /// The type used to retrieve the database connection string from the service IOptions.
        /// If not provided will default to ConnectionString
        /// </summary>
        public string? OptionsConnectionStringKey { get; set; }

        /// <summary>
        /// The type used to retrieve the database user from the service IOptions.
        /// If not provided will default to ConnectionUser
        /// </summary>
        public string? OptionsConnectionUserKey { get; set; }

        /// <summary>
        /// The type used to retrieve the database password from the service IOptions.
        /// If not provided will default to ConnectionPassword
        /// </summary>
        public string? OptionsConnectionPasswordKey { get; set; }
    }
}
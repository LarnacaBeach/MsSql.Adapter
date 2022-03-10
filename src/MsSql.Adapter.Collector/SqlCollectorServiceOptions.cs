namespace MsSql.Collector
{
    public class SqlCollectorServiceOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ConnectionUser { get; set; } = string.Empty;
        public string ConnectionPassword { get; set; } = string.Empty;
        public string ProcedurePattern { get; set; } = "(?i)(^prc__?)(?!.*internal).*";
        public bool SkipOutputParams { get; set; } = false;
        public string ResultFile { get; set; } = "result.json";
        public string PreviousResultFile { get; set; } = "result_prev.json";
    }
}
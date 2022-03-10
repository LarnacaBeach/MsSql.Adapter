using System.Collections.Generic;

using System.Runtime.Serialization;

namespace MsSql.Adapter.Collector.Types
{
    [DataContract]
    public class ProcedureMeta
    {
        [DataMember(Order = 1)]
        public string SpName { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public List<ParamMeta> Request { get; set; } = new List<ParamMeta>();

        [DataMember(Order = 3)]
        public List<ResponseItem> Responses { get; set; } = new List<ResponseItem>();

        [DataMember(Order = 5)]
        public List<string> Errors { get; set; } = new List<string>();
    }
}
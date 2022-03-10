using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MsSql.Collector.Types
{
    [DataContract]
    public class ResponseItem
    {
        [DataMember(Order = 1)]
        public string Name { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public List<ParamMeta> Params { get; set; } = new List<ParamMeta>();

        [DataMember(Order = 3)]
        public int Order { get; set; } = 0;
    }
}
using System.Collections.Generic;

using System.Runtime.Serialization;

namespace MsSql.Adapter.Collector.Types
{
    [DataContract]
    public class ParamMeta
    {
        [DataMember(Order = 1)]
        public string Name { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public string SqlType { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public List<TvpParamMeta>? TVP { get; set; }

        [DataMember(Order = 4)]
        public bool IsNullable { get; set; } = false;

        [DataMember(Order = 5)]
        public int Order { get; set; } = 0;
    }
}
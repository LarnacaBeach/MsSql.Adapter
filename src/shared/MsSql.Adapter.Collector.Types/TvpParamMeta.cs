using System.Runtime.Serialization;

namespace MsSql.Adapter.Collector.Types
{
    [DataContract]
    public class TvpParamMeta
    {
        [DataMember(Order = 1)]
        public string Name { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public string SqlType { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public bool IsNullable { get; set; } = false;

        [DataMember(Order = 4)]
        public int Order { get; set; } = 0;
    }
}
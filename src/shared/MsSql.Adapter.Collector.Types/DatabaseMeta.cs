using System;
using System.Runtime.Serialization;

namespace MsSql.Adapter.Collector.Types
{
    [Serializable]
    [DataContract]
    public class DatabaseMeta
    {
        [DataMember(Order = 1)]
        public string Name { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public ProcedureMeta[] Procedures { get; set; } = new ProcedureMeta[0];
    }
}
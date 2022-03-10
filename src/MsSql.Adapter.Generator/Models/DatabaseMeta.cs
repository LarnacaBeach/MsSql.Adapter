using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MsSql.Adapter.Generator.Models;

[DataContract]
public class DatabaseMeta
{
    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public List<ProcedureMeta> Procedures { get; set; }

    public DatabaseMeta(Collector.Types.DatabaseMeta dbMeta)
    {
        Name = dbMeta.Name;
        Procedures = dbMeta.Procedures
            .Select(p => new ProcedureMeta(p))
            .ToList();
    }
}
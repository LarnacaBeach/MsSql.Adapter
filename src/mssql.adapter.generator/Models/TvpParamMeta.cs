using System.Runtime.Serialization;

namespace MsSql.Adapter.Generator.Models;

[DataContract]
public class TvpParamMeta
{
    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public string SqlType { get; set; }

    [DataMember(Order = 3)]
    public bool IsNullable { get; set; }

    [DataMember(Order = 4)]
    public int Order { get; set; }

    public TvpParamMeta(Collector.Types.TvpParamMeta tvpParamMeta)
    {
        Name = tvpParamMeta.Name;
        SqlType = tvpParamMeta.SqlType;
        IsNullable = tvpParamMeta.IsNullable;
        Order = tvpParamMeta.Order;
    }
}
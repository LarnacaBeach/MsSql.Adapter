using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MsSql.Adapter.Generator.Models;

[DataContract]
public class ParamMeta
{
    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public string SqlType { get; set; }

    [DataMember(Order = 3)]
    public List<TvpParamMeta>? TVP { get; set; }

    [DataMember(Order = 4)]
    public bool HasDefaultValue { get; set; }

    [DataMember(Order = 5)]
    public int Order { get; set; }

    [DataMember(Order = 5)]
    public int TVPMaxOrder { get; set; }

    public ParamMeta(Collector.Types.ParamMeta paramMeta)
    {
        Name = paramMeta.Name;
        SqlType = paramMeta.SqlType;
        TVP = paramMeta.TVP
            ?.Select(tvp => new TvpParamMeta(tvp))
            ?.ToList();
        TVPMaxOrder = TVP
            ?.Select(x => x.Order)
            ?.DefaultIfEmpty()
            ?.Max() ?? 0;
        HasDefaultValue = paramMeta.HasDefaultValue;
        Order = paramMeta.Order;
    }
}

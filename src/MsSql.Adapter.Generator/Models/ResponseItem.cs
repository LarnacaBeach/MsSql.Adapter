using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace MsSql.Adapter.Generator.Models;

[DataContract]
public class ResponseItem
{
    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public List<ParamMeta> Params { get; set; }

    [DataMember(Order = 3)]
    public int Order { get; set; }

    [DataMember(Order = 4)]
    public int ParamsMaxOrder { get; set; }

    [DataMember(Order = 5)]
    public bool IsOperationResult { get; set; }

    public ResponseItem(Collector.Types.ResponseItem responseItem)
    {
        Name = responseItem.Name;
        Params = responseItem.Params
            .Select(p => new ParamMeta(p))
            .ToList();
        ParamsMaxOrder = Params
            .Select(x => x.Order)
            .DefaultIfEmpty()
            .Max();
        Order = responseItem.Order;
        IsOperationResult = Collector.Types.Helpers.IsOperationResult(responseItem);
    }
}
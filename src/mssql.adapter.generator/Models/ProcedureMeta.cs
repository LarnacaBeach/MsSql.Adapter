using System.Collections.Generic;
using System.Runtime.Serialization;


namespace MsSql.Adapter.Generator.Models;

[DataContract]
public class ProcedureMeta
{
    [DataMember(Order = 1)]
    public string SpName { get; set; }

    [DataMember(Order = 2)]
    public List<ParamMeta> Request { get; set; }

    [DataMember(Order = 3)]
    public List<ResponseItem> Responses { get; set; } = new List<ResponseItem>();

    [DataMember(Order = 5)]
    public List<string> Errors { get; set; } = new List<string>();

    [DataMember(Order = 6)]
    public int RequestMaxOrder { get; set; }

    public ProcedureMeta(Collector.Types.ProcedureMeta procedureMeta)
    {
        SpName = procedureMeta.SpName;
        Request = procedureMeta.Request
            .Select(p => new ParamMeta(p))
            .ToList();
        RequestMaxOrder = Request
            .Select(x => x.Order)
            .DefaultIfEmpty()
            .Max();
        Responses = procedureMeta.Responses
            .Select(r => new ResponseItem(r))
            .ToList();
        Errors = procedureMeta.Errors;
    }
}
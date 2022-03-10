using System.Runtime.Serialization;

namespace MsSql.Adapter
{
    [DataContract]
    public class DalServiceOptions
    {
        [DataMember(Order = 1)]
        public string ConnectionString { get; set; } = "";

        [DataMember(Order = 2)]
        public string? ConnectionUser { get; set; }

        [DataMember(Order = 3)]
        public string? ConnectionPassword { get; set; }
    }
}
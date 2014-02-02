using System.Runtime.Serialization;

namespace BingMapsAR.Models
{
    [DataContract]
    public class ResultSet
    {
        [DataMember(Name = "__copyright", EmitDefaultValue = false)]
        public string Copyright { get; set; }

        [DataMember(Name = "results", EmitDefaultValue = false)]
        public Result[] Results { get; set; }
    }
}

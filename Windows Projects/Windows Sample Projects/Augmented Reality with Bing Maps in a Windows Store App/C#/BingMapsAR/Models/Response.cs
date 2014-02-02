using System.Runtime.Serialization;

namespace BingMapsAR.Models
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "d", EmitDefaultValue = false)]
        public ResultSet ResultSet { get; set; }
    }
}

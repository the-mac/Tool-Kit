using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BM_EF5_WebService.Common
{
    [DataContract]
    [KnownType(typeof(City))]
    [KnownType(typeof(Country))]
    public class BaseEntity
    {
        [DataMember]
        public string WKT { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double Distance { get; set; }
    }

    [DataContract]
    public class City : BaseEntity
    {
        [DataMember]
        public string CountryISO { get; set; }

        [DataMember]
        public int Population { get; set; }
    }

    [DataContract]
    public class Country : BaseEntity
    {
        [DataMember]
        public string ISO { get; set; }

        [DataMember]
        public int Population { get; set; }
    }

    [DataContract]
    public class Response
    {
        [DataMember]
        public List<BaseEntity> Results { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Error { get; set; }
    }
}

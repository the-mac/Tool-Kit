using GART.Data;

namespace BingMapsAR.Models
{
    public class PoiItem : ARItem
    {
        public string Name { get; set; }

        public string AddressLine { get; set; }

        public string Locality { get; set; }

        public string PostalCode { get; set; }

        public string Phone { get; set; }

        public string EntityTypeID { get; set; }

        public double Distance { get; set; }
    }
}

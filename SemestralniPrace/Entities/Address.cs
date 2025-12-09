using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNumber { get; set; }
        public Post Post { get; set; }
        public string FullAddress => $"{Street} {HouseNumber} {StreetNumber} {Post.CityWithPSC}";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNumber { get; set; }

        public List<Buyer> Buyers { get; set; }
        public Post Post { get; set; }
    }
}

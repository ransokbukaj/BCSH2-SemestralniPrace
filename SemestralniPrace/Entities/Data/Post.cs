using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data
{
    internal class Post
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string PSC { get; set; }

        public List<Adress> Adresses { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Home
{
    public class UserStatistics
    {
        public int AmountOfChanges { get; set; }
        public int AmountOfInserts { get; set; }

        public int AmountOfUpdate { get; set; }
        public int AmountOfDelete { get; set; }
        public DateTime LastChange { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Home
{
    public class ArtistStatistic
    {
        public int AmounArtPiece { get; set; }
        public int AmountSold { get; set; }
        public double FullProfit { get; set; }
        public double MinProfit { get; set; }
        public double MaxProfit { get; set; }
        public double AvgProfit { get; set; }
    }
}

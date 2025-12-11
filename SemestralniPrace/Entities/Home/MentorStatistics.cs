using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Home
{
    public class MentorBranchStatistics
    {
        public int ArtistsInBranch { get; set; }
        public int AmountofSales { get; set; }
        public double TotalProfit { get; set; }
    }

    public class MostSuccesfulMentore
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int AmountOfArtPieces { get; set; }
    }
}

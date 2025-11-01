using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    internal class Exhibition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public string Description { get; set; }

        public List<ArtPiece> ArtPieces { get; set; }
        public List<Visit> Visits { get; set; }
    }
}

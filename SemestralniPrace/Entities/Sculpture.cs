using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Sculpture : ArtPiece
    {
        public double Depth { get; set; }
        public double Weight { get; set; }

        public Material Material { get; set; }
    }
}

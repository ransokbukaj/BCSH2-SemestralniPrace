using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    internal class Painting : ArtPiece
    {
        public Foundation Base { get; set; }
        public Technique Technique { get; set; }

    }
}

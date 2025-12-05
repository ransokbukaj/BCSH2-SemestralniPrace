using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Painting : ArtPiece
    {
        public Counter Base { get; set; }
        public Counter Technique { get; set; }
    }
}

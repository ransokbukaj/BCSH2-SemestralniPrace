using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data
{
    public abstract class ArtPiece
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly PublishedDate { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public List<Artist> Creators { get; set; }
        public Sale? Sale { get; set; }
    }
}

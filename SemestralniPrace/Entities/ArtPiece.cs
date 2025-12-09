using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public abstract class ArtPiece
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        //public List<Artist> Creators { get; set; }
        //public List<Attachment> Attachments { get; set; }
        //public Exhibition? Exhibition { get; set; }
        //public Sale? Sale { get; set; }

        public string ArtistNames { get; set; }
        public Counter ExhibitionCounter { get; set; }
        public Sale Sale { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}

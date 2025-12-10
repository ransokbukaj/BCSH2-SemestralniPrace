using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ArtPiece
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public int ExhibitionId { get; set; }
        public int SaleId { get; set; }
    }
}

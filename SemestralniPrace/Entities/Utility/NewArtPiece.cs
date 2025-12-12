using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Home
{
    public class NewArtPiece
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public string Authors { get; set; }
        public DateTime PublishedDate { get; set; }
   
        public int ExhibitionId { get; set; }
        public int SaleId { get; set; }

        public string TypeToString
        {
            get
            {
                return Type switch
                {
                    "S" => "Socha",
                    "O" => "Obraz",
                    _ => "Neznámý typ"
                };
            }
        }
    }
}

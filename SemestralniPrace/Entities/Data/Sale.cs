using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Data
{
    public class Sale
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime DateOfSale { get; set; }
        public string CardNumber { get; set; }
        public string AccountNumber { get; set; }

        public PaymentMethod TypeOfPayment { get; set; }
        public List<ArtPiece> ArtPieces { get; set; }
    }
}

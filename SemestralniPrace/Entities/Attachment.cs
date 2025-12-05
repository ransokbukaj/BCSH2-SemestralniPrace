using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Attachment
    {
        public int IdPriloha { get; set; }

        public byte[] Soubor { get; set; }

        public string TypSouboru { get; set; }

        public string NazevSouboru { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Attachment
    {
        public int Id { get; set; }

        public byte[] File { get; set; }

        public string FileType { get; set; }

        public string FileName { get; set; }
    }
}

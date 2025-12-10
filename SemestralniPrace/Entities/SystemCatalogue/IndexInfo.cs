using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o indexu
    /// </summary>
    public class IndexInfo
    {
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public string IndexType { get; set; }
        public string Columns { get; set; }
        public int? RowCount { get; set; }
        public int? DistinctKeys { get; set; }
        public string Status { get; set; }

        public bool IsValid => Status?.ToUpper() == "PLATNÝ";
        public bool IsUnique => IndexType?.Contains("UNIKÁTNÍ") == true;
    }
}

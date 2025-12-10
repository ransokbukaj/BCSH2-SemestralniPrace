using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o tabulce v databázi
    /// </summary>
    public class TableInfo
    {
        public string TableName { get; set; }
        public string Description { get; set; }
        public int ColumnCount { get; set; }
        public int ForeignKeyCount { get; set; }
        public int? EstimatedRowCount { get; set; }
        public decimal? SizeMB { get; set; }
        public DateTime? LastAnalyzed { get; set; }

        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
    }
}

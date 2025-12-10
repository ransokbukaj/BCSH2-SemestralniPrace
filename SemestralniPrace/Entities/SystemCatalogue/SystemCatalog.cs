using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Kompletní systémový katalog databáze
    /// </summary>
    public class SystemCatalog
    {
        public DateTime GeneratedDate { get; set; }
        public string DatabaseUser { get; set; }

        public List<TableInfo> Tables { get; set; } = new List<TableInfo>();
        public List<ViewInfo> Views { get; set; } = new List<ViewInfo>();
        public List<PrimaryKeyInfo> PrimaryKeys { get; set; } = new List<PrimaryKeyInfo>();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new List<ForeignKeyInfo>();
        public List<IndexInfo> Indexes { get; set; } = new List<IndexInfo>();
        public List<SequenceInfo> Sequences { get; set; } = new List<SequenceInfo>();
        public List<TriggerInfo> Triggers { get; set; } = new List<TriggerInfo>();
        public List<ProcedureInfo> Procedures { get; set; } = new List<ProcedureInfo>();

        // Statistiky
        public int TotalTableCount => Tables?.Count ?? 0;
        public int TotalViewCount => Views?.Count ?? 0;
        public int TotalColumnCount
        {
            get
            {
                int count = 0;
                if (Tables != null)
                {
                    foreach (var table in Tables)
                    {
                        count += table.Columns?.Count ?? 0;
                    }
                }
                return count;
            }
        }
        public int TotalForeignKeyCount => ForeignKeys?.Count ?? 0;
        public int TotalIndexCount => Indexes?.Count ?? 0;
        public int TotalTriggerCount => Triggers?.Count ?? 0;
        public int TotalProcedureCount => Procedures?.Count ?? 0;
        public int TotalSequenceCount => Sequences?.Count ?? 0;
    }
}

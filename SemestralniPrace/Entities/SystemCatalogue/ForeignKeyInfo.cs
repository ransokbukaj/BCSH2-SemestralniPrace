using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o cizím klíči
    /// </summary>
    public class ForeignKeyInfo
    {
        public string SourceTable { get; set; }
        public string ConstraintName { get; set; }
        public string SourceColumns { get; set; }
        public string TargetTable { get; set; }
        public string TargetColumns { get; set; }
        public string DeleteRule { get; set; }

        public string Relationship => $"{SourceTable} ({SourceColumns}) → {TargetTable} ({TargetColumns})";
    }
}

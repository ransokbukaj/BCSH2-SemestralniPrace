using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o primárním klíči
    /// </summary>
    public class PrimaryKeyInfo
    {
        public string TableName { get; set; }
        public string ConstraintName { get; set; }
        public string Columns { get; set; }
    }
}

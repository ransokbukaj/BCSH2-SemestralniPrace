using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o sloupci tabulky
    /// </summary>
    public class ColumnInfo
    {
        public int Order { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string Nullable { get; set; }
        public string DefaultValue { get; set; }
        public string Description { get; set; }
        public string IsPrimaryKey { get; set; }
        public string IsForeignKey { get; set; }

        public bool IsNullable => Nullable?.ToUpper() == "ANO";
        public bool IsPK => IsPrimaryKey?.ToUpper() == "ANO";
        public bool IsFK => IsForeignKey?.ToUpper() == "ANO";
    }
}

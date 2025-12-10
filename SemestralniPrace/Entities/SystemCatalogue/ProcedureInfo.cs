using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o stored proceduře nebo funkci
    /// </summary>
    public class ProcedureInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }
        public int ParameterCount { get; set; }

        public bool IsValid => Status?.ToUpper() == "PLATNÝ";
        public bool IsFunction => Type?.Contains("FUNKCE") == true;
        public bool IsProcedure => Type?.Contains("PROCEDURA") == true;
    }
}
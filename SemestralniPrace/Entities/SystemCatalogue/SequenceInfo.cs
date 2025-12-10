using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o sekvenci
    /// </summary>
    public class SequenceInfo
    {
        public string SequenceName { get; set; }
        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }
        public int IncrementBy { get; set; }
        public long? LastValue { get; set; }
        public string Cache { get; set; }
        public string IsCyclic { get; set; }
        public string IsOrdered { get; set; }

        public bool Cyclic => IsCyclic?.ToUpper() == "ANO";
        public bool Ordered => IsOrdered?.ToUpper() == "ANO";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o triggeru
    /// </summary>
    public class TriggerInfo
    {
        public string TriggerName { get; set; }
        public string TableName { get; set; }
        public string Event { get; set; }
        public string TriggerType { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public bool IsActive => Status?.ToUpper() == "AKTIVNÍ";
    }
}
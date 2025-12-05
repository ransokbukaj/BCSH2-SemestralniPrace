using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class HistoryLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string TypeOfOperation { get; set; }
        public string DescriptionOfChnage { get; set; }
        public DateTime DateOfChange { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public int EditedRowId { get; set; }
    }
}

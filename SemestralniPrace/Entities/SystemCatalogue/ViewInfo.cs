using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Informace o pohledu (VIEW)
    /// </summary>
    public class ViewInfo
    {
        public string ViewName { get; set; }
        public string Description { get; set; }
        public int ColumnCount { get; set; }
        public string Definition { get; set; }
    }
}

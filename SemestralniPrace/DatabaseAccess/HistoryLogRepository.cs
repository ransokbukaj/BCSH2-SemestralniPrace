using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Account;

namespace DatabaseAccess
{
    public class HistoryLogRepository : IHistoryLogRepository
    {
        public List<HistoryLog> GetList()
        {
            throw new NotImplementedException();
        }
    }
}

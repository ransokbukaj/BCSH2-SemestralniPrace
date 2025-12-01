using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class VisitRepository : IVisitRepository
    {
        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public List<Visit> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Visit item)
        {
            throw new NotImplementedException();
        }
    }
}

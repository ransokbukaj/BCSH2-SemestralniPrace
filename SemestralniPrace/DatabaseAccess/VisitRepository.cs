using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;

namespace DatabaseAccess
{
    public class VisitRepository : IVisitRepository
    {
        public List<Visit> GetList()
        {
            throw new NotImplementedException();
        }

        public void SaveItem(Visit visit)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}

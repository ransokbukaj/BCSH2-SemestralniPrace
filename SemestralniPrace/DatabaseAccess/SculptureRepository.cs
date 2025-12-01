using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class SculptureRepository : ISculptureRepository
    {
        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public List<Sculpture> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Sculpture item)
        {
            throw new NotImplementedException();
        }
    }
}

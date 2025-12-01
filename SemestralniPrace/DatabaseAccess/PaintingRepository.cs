using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class PaintingRepository : IPaintingRepository
    {
        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public List<Painting> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Painting item)
        {
            throw new NotImplementedException();
        }
    }
}

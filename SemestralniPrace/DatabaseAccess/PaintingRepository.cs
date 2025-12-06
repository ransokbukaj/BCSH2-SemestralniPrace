using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;

namespace DatabaseAccess
{
    public class PaintingRepository : IPaintingRepository
    {
        public List<Painting> GetList()
        {
            throw new NotImplementedException();
        }

        public void SaveItem(Painting painting)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}

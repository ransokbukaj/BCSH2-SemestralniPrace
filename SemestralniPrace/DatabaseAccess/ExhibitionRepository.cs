using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class ExhibitionRepository : IExhibitionRepository
    {
        public List<Exhibition> GetList()
        {
            throw new NotImplementedException();
        }

        public void SaveItem(Exhibition exhibition)
        {
            if (exhibition.Id == 0)
            {
                // insert
            }
            else
            {
                // update
            }
            throw new NotImplementedException();
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}

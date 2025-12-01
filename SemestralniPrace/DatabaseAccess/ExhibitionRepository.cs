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
        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public List<Exhibition> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Exhibition item)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class BuyerRepository : IBuyerRepository
    {
        public List<Buyer> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Buyer item)
        {
            if (item.Id == 0)
            {
                // insert
            }
            else
            {
                // update
            }
            throw new NotImplementedException();
        }

        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }
    }
}

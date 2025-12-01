using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class SaleRepository : ISaleRepository
    {
        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public List<Sale> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Sale item)
        {
            throw new NotImplementedException();
        }
    }
}

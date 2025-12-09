using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseAccess
{
    public class SaleRepository : ISaleRepository
    {
        public List<Sale> GetList()
        {
            
        }

        public void SaveItem(Sale sale)
        {
            
        }

        public void DeleteItem(int id)
        {
            
        }
    }
}
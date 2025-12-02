using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface ISaleRepository
    {
        List<Sale> GetList();

        void SaveItem(Sale sale);

        void DeleteItem(int id);
    }
}

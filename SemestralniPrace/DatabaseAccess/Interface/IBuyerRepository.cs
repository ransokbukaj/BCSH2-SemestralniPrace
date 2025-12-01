using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IBuyerRepository
    {
        List<Buyer> GetList();

        bool SaveItem(Buyer item);

        bool DeleteItem(int itemId);
    }
}

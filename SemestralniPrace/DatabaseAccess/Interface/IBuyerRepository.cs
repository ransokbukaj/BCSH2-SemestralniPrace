using Entities;
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

        void SaveItem(Buyer buyer);

        void DeleteItem(int id);
    }
}

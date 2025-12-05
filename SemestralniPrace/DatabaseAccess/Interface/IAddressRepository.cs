using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IAddressRepository
    {
        List<Address> GetList();

        void SaveItem(Address address);

        void DeleteItem(int id);
    }
}

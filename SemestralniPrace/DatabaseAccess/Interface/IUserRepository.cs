using Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IUserRepository
    {
        List<User> GetList();

        bool SaveItem(User item);

        bool DeleteItem(int itemId);
    }
}

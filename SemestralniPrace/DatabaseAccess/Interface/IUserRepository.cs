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

        void SaveItem(User user);

        void DeleteItem(int id);

        void ChangePassword(int id, string newPassword);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;

namespace DatabaseAccess
{
    public class UserRepository : IUserRepository
    {
        public List<User> GetList()
        {
            throw new NotImplementedException();
        }

        public void SaveItem(User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdatePassword(int id, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(int id, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}

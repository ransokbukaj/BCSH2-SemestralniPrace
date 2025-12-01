using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Account;

namespace DatabaseAccess
{
    public class UserRepository : IUserRepository
    {
        public List<User> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(User item)
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

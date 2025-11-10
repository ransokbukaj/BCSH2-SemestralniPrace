using Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public static class UserManager
    {
        public static User? CurrentUser;

        static UserManager()
        {
            CurrentUser = null;
        }

        public static bool Register()
        {
            return true;
        }

        public static bool LogIn(string username, string password)
        {
            return true;
        }

        public static void LogOut()
        {
            CurrentUser = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Account
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public DateOnly RegisterDate { get; set; }
        public DateOnly LastLogin { get; set; }
        public DateOnly LastChange { get; set; }

        public Role Role { get; set; }
        public List<LogHistory> UserHistory { get; set; }
    }
}

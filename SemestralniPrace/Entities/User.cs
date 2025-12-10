using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        /// <summary>
        /// Použít pouze při registraci!!!
        /// </summary>
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public Counter Role { get; set; }
    }
}

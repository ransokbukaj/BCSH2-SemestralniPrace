using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Artist
    {
        public Artist() { }

        public Artist(int id, string firstName, string lastName, DateTime dateOfBirth, DateTime dateOfDeath, string description)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Description = description;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfDeath { get; set; }
        public string Description { get; set; }

        public string FullName { get => $"{FirstName} {LastName}";  }
    }
}

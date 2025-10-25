using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.DTOs.Identity
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmaed { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

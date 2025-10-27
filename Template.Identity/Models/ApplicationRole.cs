using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Identity.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string RoleDescription { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

    }
}

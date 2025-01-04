using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.DTOs
{
    public class AccountActivationDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ActivationLink { get; set; }
    }
}

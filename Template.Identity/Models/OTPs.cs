using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Common;

namespace Template.Identity.Models
{
    public class OTPs : BaseObject
    {
        public string? OTP { get; set; }
        public string? Email { get; set; }
        public string? OTPType { get; set; }
        public string? Token { get; set; }
    }
}

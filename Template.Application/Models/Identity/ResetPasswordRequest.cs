using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Models.Identity
{
    public class ResetPasswordRequest
    {
        public string? Email { get; set; }
        public string NewPassword { get; set; }
        public string OTP { get; set; }
    }

    public class ResetPasswordResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}

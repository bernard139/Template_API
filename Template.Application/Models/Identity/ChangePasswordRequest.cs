using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Template.Application.Models.Identity
{
    public class ChangePasswordRequest
    {
        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordResponse
    {
        public bool Status { get; set; } 
        public string Message { get; set; }
    }
}

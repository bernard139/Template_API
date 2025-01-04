using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Responses
{
    public class BasicResponse
    {
        [DefaultValue(true)]
        public bool IsSuccessful { get; set; }
        [DefaultValue(null)]
        public ErrorResponse Error { get; set; }

        public BasicResponse()
        {
            IsSuccessful = false;
        }
        public BasicResponse(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}

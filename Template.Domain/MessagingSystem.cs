using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Common;

namespace Template.Domain
{
    public class MessagingSystem : BaseObject
    {
        public string Message { get; set; }
        public string MessageType { get; set; }
        public bool Status { get; set; }
    }
}

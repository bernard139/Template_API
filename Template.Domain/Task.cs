using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Common;

namespace Template.Domain
{
    public class Task : BaseObject
    {
        public string Name { get; set; } 
        public string Description { get; set; }
    }
}

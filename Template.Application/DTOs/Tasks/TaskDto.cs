
using Template.Application.DTOs.Common;
using Template.Application.DTOs.Tasks;
using Template.Domain;

namespace Template.Application.DTOs.Tasks
{
    public class TaskDto : BaseDto
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }

    }
}

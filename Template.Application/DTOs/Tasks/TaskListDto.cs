
using Template.Application.DTOs.Common;
using Template.Domain;

namespace Template.Application.DTOs.Tasks
{
    public class TaskListDto : BaseDto
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
    }
}

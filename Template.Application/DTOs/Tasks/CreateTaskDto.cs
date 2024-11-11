using Template.Application.DTOs.Common;
using Template.Application.DTOs.Tasks;

namespace Template.Application.DTOs.Tasks
{
    public class CreateTaskDto : BaseDto, ITaskDto
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public string Status { get; set; }
    }
}

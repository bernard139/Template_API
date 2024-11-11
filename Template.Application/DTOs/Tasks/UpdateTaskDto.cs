using Template.Application.DTOs.Common;
using Template.Domain;
using Template.Application.DTOs.Tasks;

namespace Template.Application.DTOs.Tasks
{
    public class UpdateTaskDto : BaseDto, ITaskDto
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
    }
}

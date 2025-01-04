using Template.Application.DTOs.Common;
using Template.Application.DTOs.Tasks;

namespace Template.Application.DTOs.Tasks
{
    public class CreateTaskDto : BaseDto, ITaskDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

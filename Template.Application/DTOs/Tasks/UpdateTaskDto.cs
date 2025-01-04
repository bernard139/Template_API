using Template.Application.DTOs.Common;
using Template.Domain;
using Template.Application.DTOs.Tasks;

namespace Template.Application.DTOs.Tasks
{
    public class UpdateTaskDto : BaseDto, ITaskDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

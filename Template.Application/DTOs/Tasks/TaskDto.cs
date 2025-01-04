
using Template.Application.DTOs.Common;
using Template.Application.DTOs.Tasks;
using Template.Domain;

namespace Template.Application.DTOs.Tasks
{
    public class TaskDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class TaskModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

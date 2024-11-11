namespace Template.Application.DTOs.Tasks
{
    public interface ITaskDto
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
    }
}

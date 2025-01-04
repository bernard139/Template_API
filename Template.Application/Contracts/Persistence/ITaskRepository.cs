
using Template.Application.Contracts.Persistence;
using Task = Template.Domain.Task;

namespace Template.Application.Contracts.Persistence
{
    public interface ITaskRepository : IGenericRepository<Task>
    {
    }
}

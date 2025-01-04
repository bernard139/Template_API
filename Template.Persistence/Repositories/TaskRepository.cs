using Microsoft.EntityFrameworkCore;
using Template.Application.Contracts.Persistence;
using Template.Application.DTOs.Tasks;
using Template.Domain;

namespace Template.Persistence.Repositories
{
    public class TaskRepository : GenericRepository<Domain.Task>, ITaskRepository
    {
        private readonly TemplateDbContext _dbContext;
        public TaskRepository(TemplateDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

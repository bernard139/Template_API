using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Persistence;

namespace Template.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TemplateDbContext _context;
        //private ITaskRepository _taskRepository;

        public UnitOfWork(TemplateDbContext context)
        {
            _context = context;
        }

        //public ITaskRepository TaskRepository => 
        //    _taskRepository ??= new TaskRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        //ITaskRepository TaskRepository { get; }
        Task Save();
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain;

namespace Template.Application.Contracts.Persistence
{
    public interface ITemplateDbContext
    {
        public DbSet<Domain.Task> Tasks { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

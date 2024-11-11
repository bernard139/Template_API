using Microsoft.EntityFrameworkCore;
using Template.Domain;
using Template.Domain.Common;

namespace Template.Persistence
{
    public class TemplateDbContext : DbContext
    {
        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseObject>())
            {
                entry.Entity.ModifiedDate = DateTime.Now;

                if (entry.State == EntityState.Added) 
                    entry.Entity.CreatedDate = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        //public DbSet<Task> Tasks { get; set;}
    }
}

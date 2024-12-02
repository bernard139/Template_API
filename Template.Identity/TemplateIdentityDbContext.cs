
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Identity.Configurations;
using Template.Identity.Models;

namespace Template.Identity
{
    public class TemplateIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public TemplateIdentityDbContext(DbContextOptions<TemplateIdentityDbContext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        }

        public DbSet<OTPs> OTPs { get; set; }
    }
}
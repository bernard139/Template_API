using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Identity;

namespace Template.Identity
{
    public class TemplateIdentityDbContextFactory : IDesignTimeDbContextFactory<TemplateIdentityDbContext>
    {
        public TemplateIdentityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Template.API"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TemplateIdentityDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("TemplateIdentityConnectionString"));

            return new TemplateIdentityDbContext(optionsBuilder.Options);
        }
    }

    //public class TemplateIdentityDbContextFactory : IDesignTimeDbContextFactory<TemplateIdentityDbContext>
    //{
    //    public TemplateIdentityDbContext CreateDbContext(string[] args)
    //    {
    //        var builder = new DbContextOptionsBuilder<TemplateIdentityDbContext>();
    //        builder.UseSqlServer("YourActualConnectionString");

    //        return new TemplateIdentityDbContext(builder.Options);
    //    }
    //}
}
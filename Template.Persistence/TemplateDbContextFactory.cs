using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Template.Persistence
{
    public class TemplateDbContextFactory : IDesignTimeDbContextFactory<TemplateDbContext>
    {
        public TemplateDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TemplateDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("TemplateConnectionString"));

            return new TemplateDbContext(optionsBuilder.Options);
        }
    }
}
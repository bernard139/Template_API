using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Infrastructure;
using Template.Application.Contracts.Misc;
using Template.Application.Models;

namespace Template.Misc
{
    public static class MiscServicesRegistration
    {
        public static IServiceCollection ConfigureMiscServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IFileStorageService, FileStorageService>();

            return services;
        }
    }
}

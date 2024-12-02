using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using Template.Application.Contracts.Infrastructure;
using Template.Application.Models;
using Template.Infrastructure.Mail;

namespace Template.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SmtpSettings>>().Value);
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailRequests, EmailRequests>();

            return services;
        }
    }
}

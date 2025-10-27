using Template.Application.Models;
using Template.Application.Models.Enums;

namespace Template.Application.Contracts.Infrastructure
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(Email email);
    }
}

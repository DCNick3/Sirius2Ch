using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Sirius2Ch.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TODO: Add email sending logic
            return Task.CompletedTask;
        }
    }
}
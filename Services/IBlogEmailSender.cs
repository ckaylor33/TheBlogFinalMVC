using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace TheBlogFinalMVC.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string name, string emailFrom, string htmlMessage, string subject);
    }
}

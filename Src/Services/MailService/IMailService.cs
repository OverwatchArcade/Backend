using MimeKit;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.MailService
{
    public interface IMailService
    {
        public void SendEmail(string username, string toEmail, EmailTypes emailType, object data);
    }
}
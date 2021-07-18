using System;
using System.IO;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;

namespace OWArcadeBackend.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static string CreateSubject(EmailTypes emailType)
        {
            switch(emailType)
            {
                case EmailTypes.Signup:
                    return "Thank you for signing up";
                default:
                    throw new ArgumentOutOfRangeException(nameof(emailType), emailType, null);
            }
        }

        private static StringBuilder ReplaceGeneral(StringBuilder builder)
        {
            builder.Replace("{{TWITTER_IMG}}", Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_EMAIL_SOCIAL_TWITTER_ICON);
            builder.Replace("{{DISCORD_IMG}}", Environment.GetEnvironmentVariable("BACKEND_URL") + ImageConstants.IMG_EMAIL_SOCIAL_DISCORD_ICON);
            return builder;
        }
        
        /// <summary>
        /// Customizes the email template. Sets variables accordingly 
        /// </summary>
        /// <param name="emailType"></param>
        /// <param name="username"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static StringBuilder CreateEmailTemplate(EmailTypes emailType, string username, object data)
        {
            var builder = new StringBuilder();
            
            switch(emailType)
            {
                case EmailTypes.Signup:
                {
                    using var reader = File.OpenText("Emails/signup.html");
                    builder.Append(reader.ReadToEnd());
                    builder.Replace("{{USER}}", username);
                    builder.Replace("{{SUBJECT}}", CreateSubject(EmailTypes.Signup));
                    builder.Replace("{{HERO_IMG}}",
                        Environment.GetEnvironmentVariable("BACKEND_URL") +
                        ImageConstants.IMG_EMAIL_HERO_SIGNUP);
                    builder.Replace("{{CONFIRM_URL}}", (string) data);
                    ReplaceGeneral(builder);
                    return builder;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(emailType), emailType, null);
            }
        }

        private MimeMessage CreateMimeMessage(string username, string toEmail, EmailTypes emailType, object data)
        {
            var mimeMessage = new MimeMessage();
            var subject = CreateSubject(emailType);
            
            mimeMessage.From.Add(new MailboxAddress("OverwatchArcade.Today", "info@overwatcharcade.today"));
            mimeMessage.To.Add(new MailboxAddress(username, toEmail));
            mimeMessage.Subject = subject;
            
            
            var bodyBuilder = new BodyBuilder {HtmlBody = CreateEmailTemplate(emailType, username, data).ToString()};
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            return mimeMessage;
        }

        public void SendEmail(string username, string toEmail, EmailTypes emailType, object data)
        {
            var mimeMessage = CreateMimeMessage(username, toEmail, emailType, data);
            using var client = new SmtpClient();
            client.Connect(
                _configuration["SMTP:Host"],
                int.Parse(_configuration["SMTP:Port"]),
                bool.Parse(_configuration["SMTP:SSL"]));
            client.Authenticate(_configuration["SMTP:Username"], _configuration["SMTP:Password"]);

            client.Send(mimeMessage);
            client.Disconnect(true);
        }
    }
}
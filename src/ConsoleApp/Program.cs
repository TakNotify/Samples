using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TakNotify;

namespace ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // constants
            const string _smtpServer = "smtp.gmail.com";
            const int _smtpPort = 587;
            const string _smtpUsername = "user@gmail.com";
            const string _smtpPassword = "[pass]";
            const bool _smtpUseSsl = true;

            var _emailTo = new[] { "user@example.com" }.ToList();
            var _emailFrom = "user@gmail.com";
            var _emailSubject = "[TakNotify] Test Message";
            var _emailBody = $"This message was sent from {Environment.MachineName} at {DateTime.Now.ToLongTimeString()}.";
            var _emailIsHtml = false;

            Console.WriteLine("TakNotify testing...");

            // logger
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug).AddConsole();
            });

            // smtp provider
            var emailProvider = new SmtpProvider(new SmtpProviderOptions
            {
                Server = _smtpServer,
                Port = _smtpPort,
                Username = _smtpUsername,
                Password = _smtpPassword,
                UseSSL = _smtpUseSsl
            }, loggerFactory);

            // get notification instance
            var notification = Notification.GetInstance(loggerFactory.CreateLogger<Notification>());
            notification.AddProvider(emailProvider);

            // email message
            var message = new EmailMessage
            {
                ToAddresses = _emailTo,
                FromAddress = _emailFrom,
                Subject = _emailSubject,
                Body = _emailBody,
                IsHtml = _emailIsHtml
            };

            // send the message
            var result = await notification.SendEmailWithSmtp(message);

            return result.IsSuccess ? 0 : 1;
        }
    }
}

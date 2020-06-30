using MailKit.Net.Smtp;
using MimeKit;
using Nwassa.Core.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nwassa.Core.Helpers
{
    public class EmailGenerator : IEmailGenerator
    {
        private readonly NotificationMetadata _notificationMetadata;
        public EmailGenerator(NotificationMetadata notificationMetaData)
        {
            _notificationMetadata = notificationMetaData;
        }
        private MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(message.Sender);
            mimeMessage.To.Add(message.Reciever);
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            { Text = message.Content };
            return mimeMessage;
        }

        public string SendMail(EmailModel model)
        {
            EmailMessage message = new EmailMessage
            {
                Sender = new MailboxAddress("Self", _notificationMetadata.Sender),
                Reciever = new MailboxAddress("Self", model.Reciever),
                Subject = "Welcome",
                Content = "Hello World!"
            };
            var mimeMessage = CreateMimeMessageFromEmailMessage(message);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect(_notificationMetadata.SmtpServer,
                _notificationMetadata.Port, true);
                smtpClient.Authenticate(_notificationMetadata.UserName,
                _notificationMetadata.Password);
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
            }
            return "Email sent successfully";
        }

        public class EmailModel
        {
            public string Reciever { get; set; }

            public string Subject { get; set; }

            public string Content { get; set; }
        }
    }
}

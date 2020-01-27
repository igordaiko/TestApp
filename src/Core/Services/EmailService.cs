using System.Net;
using System.Net.Mail;
using ImportCoordinator.Contracts;

namespace ImportCoordinator.Core.Services
{
    public class EmailService
    {
        public EmailService(Session session)
        {
            _session = session;
            _settings = Settings.Instance.Email;
            _client = new SmtpClient(_settings.Host, _settings.Port);

            if (_settings.UserName == null && _settings.Password == null)
                return;

            _client.DeliveryMethod = SmtpDeliveryMethod.Network;
            _client.UseDefaultCredentials = false;
            _client.Credentials = new NetworkCredential(_settings.UserName, _settings.Password);
        }

        public ResponseVoid Send(MailModel mailModel)
        {
            var response = new ResponseVoid();

            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(mailModel.FromAddress, mailModel.FromName),
                    Subject = mailModel.Subject,
                    Body = mailModel.Body,
                    IsBodyHtml = false
                };

                message.To.Add(new MailAddress(mailModel.ToAddress, mailModel.ToName));
                mailModel.Attachments.ForEach(x => message.Attachments.Add(x));
                _client.Send(message);
            }
            catch (SmtpFailedRecipientException)
            {
                response.SetError("Recipient(s) failed");
            }

            return response;
        }

        private readonly SmtpClient _client;
        private readonly EmailSettings _settings;
        private readonly Session _session;
    }
}
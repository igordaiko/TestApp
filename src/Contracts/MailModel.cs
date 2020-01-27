using System.Collections.Generic;
using System.Net.Mail;

namespace ImportCoordinator.Contracts
{
    public class MailModel
    {
        public string FromName { get; set; }

        public string FromAddress { get; set; }

        public string ToName { get; set; }

        public string ToAddress { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}

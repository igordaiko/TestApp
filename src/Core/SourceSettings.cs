using System;

namespace ImportCoordinator.Core
{
    public class SourceSettings
    {
        public EventSettings EventSettings { get; set; }

        public MailSettings MailSettings { get; set; }

        public DirectoriesSettings DirectoriesSettings { get; set; }

        public DateTime ExpiresIn { get; set; }

        public bool Expired => DateTime.Now.AddDays(1) < ExpiresIn;
    }


    public class EventSettings
    {
        public string Event { get; set; }

        public string EventEndpoint { get; set; }

        public string EventStringKey { get; set; }
    }

    public class MailSettings
    {
        public string CriticalMail { get; set; }

        public string SourceMail { get; set; }
    }

    public class DirectoriesSettings
    {
        public string InboxName { get; set; }

        public string OutName { get; set; }

        public string ConfigsName { get; set; }
    }
}

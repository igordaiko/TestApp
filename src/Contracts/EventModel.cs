using System;

namespace ImportCoordinator.Contracts
{
    public class Event
    {
        public string Action { get; set; }

        public string State{ get; set; }

        public string Id { get; set; }

        public string PackageName { get; set; }

        public DateTime EventTime { get; set; }

        public Data Data { get; set; }
    }

    public class Data
    {
        public EventStatus Status { get; set; }

        public string Description { get; set; }

        public Source SourceName { get; set; }

        public string ValidationCode { get; set; }
    }
}

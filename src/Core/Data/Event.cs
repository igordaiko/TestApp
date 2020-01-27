using System;
using LinqToDB.Mapping;

namespace ImportCoordinator.Core.Data
{
    [Table("events")]
    public class Event
    {
        [Column, PrimaryKey, Identity]
        public int Id { get; set; }

        [Column]
        public string Bank { get; set; }

        [Column]
        public string Package { get; set; }

        [Column]
        public string State { get; set; }

        [Column]
        public string Status { get; set; }

        [Column]
        public string Action { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public DateTime? Datetime { get; set; }

        [Column]
        public string Uuid { get; set; }
    }
}

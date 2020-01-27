using LinqToDB.Mapping;

namespace ImportCoordinator.Core.Data
{
    [Table("services")]
    public class ServiceModel
    {
        [Column, PrimaryKey, Identity]
        public long Id { get; set; }

        [Column]
        public string ServiceName { get; set; }

        [Column]
        public string ServiceId { get; set; }

        [Column]
        public Roles Roles { get; set; }
    }
}

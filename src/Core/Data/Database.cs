using LinqToDB;
using LinqToDB.Data;

namespace ImportCoordinator.Core.Data
{
    public class Database : DataConnection
    {

        internal static Database Open()
        {
            var db = new Database(Settings.Instance.Database.ConnectionString);

            db.BeginTransaction();

            return db;
        }

        private Database(string connectionString) : base("PostgreSQL.9.3", connectionString)
        {

        }

        public ITable<Event> Events => GetTable<Event>();

        public ITable<ServiceModel> Services => GetTable<ServiceModel>();

    }
}

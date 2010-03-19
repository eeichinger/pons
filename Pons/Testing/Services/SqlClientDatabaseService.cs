using Spring.Data.Common;

namespace Viz.Testing.Services
{
    public class SqlClientDatabaseService : DatabaseService
    {
        public SqlClientDatabaseService(IDbProvider dbProvider, bool createNew)
            : base(dbProvider, () => SqlClientDatabaseHelper.CreateDatabase(dbProvider.ConnectionString, createNew))
        {}
    }
}
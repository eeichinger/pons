using Spring.Data.Common;

namespace Pons.Services
{
    public class SqlClientDatabaseService : DatabaseService
    {
        public SqlClientDatabaseService(IDbProvider dbProvider, bool createNew)
            : base(dbProvider, () => SqlClientDatabaseHelper.CreateDatabase(dbProvider.ConnectionString, createNew))
        {}
    }
}
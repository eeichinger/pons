using Spring.Data.Common;
using Spring.Data.Core;

namespace Pons.Services
{
    public class SqlClientDatabaseService : DatabaseService
    {
        public SqlClientDatabaseService(AdoTemplate adoTemplate, bool createNew)
            : base(adoTemplate.DbProvider, () => SqlClientDatabaseHelper.CreateDatabase(adoTemplate.DbProvider.ConnectionString, createNew))
        {}
        public SqlClientDatabaseService(IDbProvider dbProvider, bool createNew)
            : base(dbProvider, () => SqlClientDatabaseHelper.CreateDatabase(dbProvider.ConnectionString, createNew))
        {}
    }
}
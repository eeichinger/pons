using Pons.Services;
using Spring.Data.Common;
using Spring.Data.Core;
using Spring.Objects.Factory.Attributes;

namespace Pons
{
    public abstract class DatabaseTestsBase : AbstractTestFixture
    {
        protected override System.Collections.IEnumerable PerFixtureServices()
        {
            yield return base.PerFixtureServices();
            yield return new ApplicationContextService(() => new TestApplicationContext(false));
            yield return new SqlClientDatabaseService(this.DbProvider, false)
                             {
                                 Create = { Resource("Sql/create_tables.sql") },
                             };
        }

        protected AdoTemplate adoTemplate { get; private set; }

        [Required]
        public IDbProvider DbProvider
        {
            set { adoTemplate = new AdoTemplate(value); }
            get { return adoTemplate.DbProvider; }
        }        
    }
}
using System.Data;
using NUnit.Framework;
using Pons.Services;
using Spring.Data.Common;
using Spring.Data.Core;
using Spring.Objects.Factory.Attributes;

namespace Pons
{
    [TestFixture]
    public class DatabaseTestsWithAutomaticDatabaseRecreate : AbstractTestFixture
    {
        protected override System.Collections.IEnumerable PerFixtureServices()
        {
            yield return new ApplicationContextService(() => new TestApplicationContext(false));
        }

        protected override System.Collections.IEnumerable PerTestServices() 
        {
            yield return new SqlClientDatabaseService(this.AdoTemplate, false)
            {
                Create = { Resource("Sql/create_tables.sql") },
            };
        }

        [Required]
        public AdoTemplate AdoTemplate { get; set; }

        [Test]
        public void access_database()
        {
            AdoTemplate.ExecuteNonQuery(CommandType.Text, "INSERT [TestTable](A,B) VALUES(1, 'V') ");
        }

        [Test]
        public void access_database2()
        {
            AdoTemplate.ExecuteNonQuery(CommandType.Text, "INSERT [TestTable](A,B) VALUES(1, 'V') ");
        }
    }
}
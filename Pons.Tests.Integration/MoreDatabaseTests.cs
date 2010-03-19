using System.Data;
using NUnit.Framework;
using Pons.Services;
using Spring.Transaction;

namespace Pons
{
    [TestFixture]
    public class MoreDatabaseTests : DatabaseTestsBase
    {
        protected override System.Collections.IEnumerable PerTestServices()
        {
            yield return base.PerTestServices();
            yield return new PlatformTransactionService(TransactionManager);
        }

        public IPlatformTransactionManager TransactionManager { get; set; }

        [Test]
        public void access_database()
        {
            adoTemplate.ExecuteNonQuery(CommandType.Text, "INSERT [TestTable](A,B) VALUES(1, 'V') ");
        }

        [Test]
        public void access_database2()
        {
            adoTemplate.ExecuteNonQuery(CommandType.Text, "INSERT [TestTable](A,B) VALUES(1, 'V') ");
        }

    }
}
using System.Data;
using NUnit.Framework;

namespace Pons
{
    [TestFixture]
    public class DatabaseTests : DatabaseTestsBase
    {
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
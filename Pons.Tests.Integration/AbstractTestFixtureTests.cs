using System.Collections;
using NUnit.Framework;

namespace Pons
{
    [TestFixture]
    public class AbstractTestFixtureTests
    {
        [Test]
        public void DisposesServicesOnSetupFixtureException()
        {
            bool disposeCalled = false;
            var services = new[]
                               {
                                   new DisposableAdapter(()=>{ disposeCalled = true; })
                               };
            var tf = new TestTestFixture(services);
            try
            {
                tf.FixtureSetUp();
                Assert.Fail("should throw");
            }
            catch (AssertionException e)
            {
                Assert.AreEqual("Expected", e.Message);
            }
            Assert.IsTrue(disposeCalled);
        }

        private class TestTestFixture : AbstractTestFixture
        {
            private IEnumerable services;

            public TestTestFixture(IEnumerable services)
            {
                this.services = services;
            }

            protected override System.Collections.IEnumerable PerFixtureServices()
            {
                return services;
            }

            protected override void  OnFixtureSetUp()
            {
                Assert.Fail("Expected");
            }
        }
    }
}
using System.Collections;
using NUnit.Framework;

namespace Viz.Testing
{
    [TestFixture]
    public class AbstractTestTests
    {
        [Test]
        public void DisposesServicesOnSetupException()
        {
            bool disposeCalled = false;
            var services = new[]
                               {
                                   new DisposableAdapter(()=>{ disposeCalled = true; })
                               };
            var tf = new TestTestFixture( services );
            try
            {
                tf.SetUp();
                Assert.Fail("should throw");
            }
            catch (AssertionException e)
            {
                Assert.AreEqual("Expected", e.Message);
            }
            Assert.IsTrue(disposeCalled);
        }

        private class TestTestFixture : AbstractTest
        {
            private IEnumerable services;

            public TestTestFixture(IEnumerable services)
            {
                this.services = services;
            }

            protected override System.Collections.IEnumerable PerTestServices()
            {
                return services;
            }

            protected override void OnSetUp()
            {
                Assert.Fail("Expected");
            }
        }
    }

}
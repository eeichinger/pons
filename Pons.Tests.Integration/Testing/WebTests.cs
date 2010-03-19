using System.Collections;
using NUnit.Framework;
using Viz.Testing.Services;

namespace Viz.Testing
{
    [TestFixture]
    public class WebTests : AbstractTestFixture
    {
        protected override IEnumerable PerFixtureServices()
        {
            yield return new WebServerService("TestData/TestWeb");
            yield return new SeleniumServerService();
        }

        protected override IEnumerable PerTestServices()
        {
            yield return new SeleniumSessionService("*iexplore", Get<SeleniumServerService>(), Get<WebServerService>());
        }

        public ISeleniumSession Selenium
        {
            get { return Get<ISeleniumSession>(true); }
        }

        [Test]
        public void FilloutForm()
        {
            Selenium.SetSpeed("1500");
            Selenium.Open("/Test/Default.aspx");
            Selenium.Type("//*[@id='name']", "FooBar");
            Selenium.Click("//*[@id='submit']");
            Selenium.WaitForPageToLoad("1000");
            string result = Selenium.GetValue("//*[@id='storedName']");
            Assert.AreEqual("FooBar", result);
        }
    }
}

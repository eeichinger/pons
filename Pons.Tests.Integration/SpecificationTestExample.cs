using System.Data;
using System.Web;
using NUnit.Framework;
using Pons.Services;
using Spring.Data.Core;
using Spring.Objects.Factory.Attributes;

namespace Pons
{
    /// <summary>
    /// This example demonstrates combining all features to run a full integration test
    /// </summary>
    [Specification]
    public class SpecificationTestExample : SpecificationWithServices
    {
        protected override System.Collections.IEnumerable PerFixtureServices()
        {
            // create container and automatically populate this specification with objects from the container
            yield return new ApplicationContextService(() => new TestApplicationContext(false));
            // create database and schema if necessary
            yield return new SqlClientDatabaseService(this.AdoTemplate, false)
            {
                Create = { Resource("Sql/create_tables.sql") },
            };
            // launch webserver
            yield return new WebServerService("TestData/TestWeb");
            // launch client controller
            yield return new SeleniumServerService();
        }

        protected override System.Collections.IEnumerable PerTestServices()
        {
            // launch client browser
            yield return new SeleniumSessionService("*iexplore", Get<SeleniumServerService>(), Get<WebServerService>());
            // NOTE: we cannot use transactions here, since the WebServer runs in its own AppDomain and 
            //       thus wouldn't see our data! Ideas anyone?
        }

        [Required]
        public AdoTemplate AdoTemplate { get; set; }

        /// <summary>
        /// For convient access to the current SeleniumSession
        /// </summary>
        public ISeleniumSession Selenium { get { return base.Get<ISeleniumSession>(true); } }

        private const string TESTDATA = "MyTestData";

        private string readData;

        [Given]
        public override void Given()
        {
            AdoTemplate.ExecuteNonQuery(CommandType.Text, "INSERT [TestTable](A,B) VALUES(1, 'MyTestData') ");
        }
        
        [When]
        public override void When()
        {
//            Selenium.SetSpeed("1500");
            Selenium.Open("/SpecificationTestExample/ReadData.aspx?id=1&connString=" + HttpUtility.UrlEncode(this.AdoTemplate.DbProvider.ConnectionString));
            Selenium.WaitForPageToLoad("1000");
            readData = Selenium.GetValue("//*[@id='readData']");
        }

        protected override void Cleanup()
        {
            AdoTemplate.ExecuteNonQuery(CommandType.Text, "TRUNCATE TABLE [TestTable]");            
        }

        [Test]
        public void readData_was_set_correct()
        {
            Assert.AreEqual(TESTDATA, readData);
        }
    }
}
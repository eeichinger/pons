using System.Net;
using Selenium;
using Spring.Data.Common;
using Spring.Data.Core;

namespace Viz.Testing
{
    public abstract class AbstractRegressionTestFixture : AbstractTestFixture
    {
        private ISelenium selenium;
        private AdoTemplate adoTemplate;
        private string _browserUrl;

        public ISelenium Selenium
        {
            get { return selenium; }
        }

        /// <summary>
        /// Sets the DbProvider, via Dependency Injection.
        /// </summary>
        /// <value>The IDbProvider.</value>
        public IDbProvider DbProvider
        {
            set
            {
                adoTemplate = new AdoTemplate(value);
            }
        }

        public AdoTemplate AdoTemplate
        {
            get { return adoTemplate; }
        }

        protected abstract string WebRootPhysicalPath { get; }

        protected abstract DatabaseScripts DatabaseScripts();

        protected override void OnFixtureSetUp()
        {
            base.OnFixtureSetUp();

            var webRootPhysicalPath = this.WebRootPhysicalPath;
            if (webRootPhysicalPath != null)
            {
                RegisterForFixtureTearDown(new WebServer(webRootPhysicalPath));
                RegisterForFixtureTearDown(new SeleniumServer());
                //            RegisterForFixtureTearDown(new DatabaseFromBackup(this, "VizECM93.bak", "VizECM93_Test", "VizECM93", "VizECM93_log"));

                int webPort = Get<WebServer>().Port;
                IPAddress ipAddr = Network.GetNonLocalIPv4Address();
                _browserUrl = "http://" + ipAddr + ":" + webPort;

                // warmup
                WarmUp(_browserUrl);
            }
        }

        protected virtual void WarmUp(string browserUrl)
        {
            WebRequest req = WebRequest.Create(browserUrl);
            req.Timeout = 2000;
            using (WebResponse res = req.GetResponse())
            {
                res.Close();
            }
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();

            SeleniumServer seleniumServer = Get<SeleniumServer>(false);
            if (seleniumServer != null)
            {
                selenium = seleniumServer.GetSelenium("*iexplore", _browserUrl);
                selenium.Start();
            }
        }

        protected override void OnTearDown()
        {
            if (selenium != null)
            {
                selenium.Close();
                selenium.Stop();
            }
            base.OnTearDown();
        }
    }
}
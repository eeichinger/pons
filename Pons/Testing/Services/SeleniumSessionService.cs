using System;

namespace Viz.Testing.Services
{
    public class SeleniumSessionService : IServiceFactory
    {
        private string browserString;
        private string browserUrl;
        private SeleniumServerService _seleniumServerService;

        public SeleniumSessionService(string browserString, SeleniumServerService _seleniumServerService, WebServerService webServerService)
            :this(browserString, _seleniumServerService, webServerService.ClientUrl)
        {}

        public SeleniumSessionService(string browserString, SeleniumServerService _seleniumServerService, string browserUrl)
        {
            this._seleniumServerService = _seleniumServerService;
            this.browserString = browserString;
            this.browserUrl = browserUrl;
        }

        public IDisposable GetService(object fixtureInstance)
        {
            return _seleniumServerService.GetSession(browserString, browserUrl);
        }
    }
}
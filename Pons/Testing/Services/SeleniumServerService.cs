using System;
using System.Diagnostics;
using System.IO;
using Selenium;
using Viz.Testing.Support;

namespace Viz.Testing.Services
{
    public class SeleniumServerService : IDisposable
    {
        private readonly int _port;
        private Process proc;
        private readonly string _jarPath;

        public int Port
        {
            get { return _port; }
        }

        public string JarPath
        {
            get { return _jarPath; }
        }

        public SeleniumServerService()
            :this(-1, null)
        {}

        public SeleniumServerService(int port, string jarPath)
        {
            if (port < 0)
            {
                port = GetPort();
            }
            if (jarPath == null)
            {
                jarPath = TestResourceLoader.GetAssemblyResourceUri(typeof(SeleniumServerService), "selenium-server.jar");
            }
            _jarPath = jarPath;
            _port = port;
            Start();
        }

        protected virtual int GetPort()
        {
            return Network.GetNextAvailableTcpPort();
        }

        public SeleniumServerService Start()
        {
            FileInfo tmpJar = TestResourceLoader.ExportResource(JarPath, new FileInfo(Path.GetTempFileName()) );
            
            proc = new Process();
            proc.StartInfo.FileName = @"java.exe";
            proc.StartInfo.Arguments = string.Format(@"-jar {0} -port {1}", tmpJar.FullName, _port);
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            bool ok = proc.Start();
            if (!ok)
            {
                throw new SystemException("failed to start SeleniumServer");
            }
            return this;
        }

        public void Dispose()
        {
            if (proc != null)
            {
                try
                {
                    GetSession("*mock", null).ShutDownSeleniumServer();
                }
                catch(Exception ex)
                {
                    Trace.WriteLine("Exception on shutting down SeleniumServer:" + ex);
                }

                if (!proc.HasExited)
                {
                    if (!proc.WaitForExit(500))
                    {
                        proc.Kill();
                    }
                }
                proc.Dispose();                
            }
        }

        public ISeleniumSession GetSession( string browserString, string browserUrl )
        {
            return new SeleniumSession("localhost", this.Port, browserString, browserUrl);
        }

        private class SeleniumSession : DefaultSelenium, ISeleniumSession
        {
            public SeleniumSession(string serverHost, int serverPort, string browserString, string browserURL)
                : base(serverHost, serverPort, browserString, browserURL)
            {
                this.Start();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                this.Close();
                this.Stop();
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using Microsoft.VisualStudio.WebHost;

namespace Pons.Services
{
    public class WebServerService : MarshalByRefObject, IDisposable
    {
        public class ServerSupport : MarshalByRefObject, IRegisteredObject
        {
            public ServerSupport()
            {
//                string name = AppDomain.CurrentDomain.FriendlyName;
                AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            }

            private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                Trace.WriteLine(string.Format("WebAppDomain Exception auf thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, e));
//                Debugger.Break();
            }

            public void Stop(bool immediate)
            {
                HttpRuntime.Close();
                return;
            }
        }

        private readonly Server server;
        private readonly ServerSupport _serverSupport;

        public int Port
        {
            get { return server.Port; }
        }

        public string PhysicalPath
        {
            get { return server.PhysicalPath; }
        }

        public string VirtualPath
        {
            get { return server.VirtualPath; }
        }

        public string ClientUrl
        {
            get
            {
                int webPort = Port;
                IPAddress ipAddr = IPAddress.Loopback; // Network.GetNonLocalIPv4Address();                 
                return "http://" + ipAddr + ":" + webPort;                
            }
        }

        public WebServerService(string physicalPath)
            :this(physicalPath, -1)
        {}

        public WebServerService(string physicalPath, int port)
        {
            if (port < 0)
            {
                port = GetPort();
            }

            DirectoryInfo webDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, physicalPath));
            physicalPath = webDirectory.FullName;
            string serverType = typeof (Server).Assembly.Location;
            DeployAssembly(physicalPath, serverType);
            string serverSideType = typeof (ServerSupport).Assembly.Location;
            DeployAssembly(physicalPath, serverSideType);

            server = new Server(port, "/", physicalPath, false);

            string appId = (server.VirtualPath + server.PhysicalPath).ToLowerInvariant().GetHashCode().ToString("x", CultureInfo.InvariantCulture);
            ApplicationManager appManager = (ApplicationManager) server.GetType().GetField("_appManager", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(server);
            _serverSupport = (ServerSupport) appManager.CreateObject(appId, typeof (ServerSupport), server.VirtualPath, server.PhysicalPath, true, true);

            server.Start();
        }

        private void DeployAssembly(string physicalPath, string serverType)
        {
            FileInfo file = new FileInfo(serverType);
            string destFileName = physicalPath + "\\bin\\" + file.Name;
            file.CopyTo(destFileName, true);
        }

        protected virtual int GetPort()
        {
            return Network.GetNextAvailableTcpPort();
        }

        public void Dispose()
        {
            try
            {
                // call HttpRuntime.Close() before shutting down Cassini prevents AppDomainUnloadedExceptions
                // see http://stackoverflow.com/questions/561402/cassini-webserver-webdev-nunit-and-appdomainunloadedexception
                _serverSupport.Stop(true);
                server.Stop();
            }
            catch (Exception e)
            {
                Trace.WriteLine("error while stopping webserver: " + e);
            }
        }
    }
}
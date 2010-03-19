using System;
using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using Spring.Core.IO;
using Spring.Data.Common;
using Spring.Data.Core;
using Spring.Testing.Ado;
using Viz.Testing.Support;

namespace Viz.Testing.Services
{
    public class DatabaseService : IDisposable, IServiceFactory
    {
        protected ILog logger = LogManager.GetLogger(typeof(DatabaseService));

        public delegate void CreateDatabase();

        private AdoTemplate _adoTemplate;
        private CreateDatabase createDatabase;

        public List<object> Create = new List<object>();
        public List<object> Load = new List<object>();
        public List<object> Delete = new List<object>();

        public AdoTemplate AdoTemplate
        {
            get { return _adoTemplate; }
        }

        public DatabaseService(IDbProvider dbProvider, CreateDatabase createDatabase)
        {
            _adoTemplate = new AdoTemplate(dbProvider);
            this.createDatabase = createDatabase;
        }

        public IDisposable GetService(object fixtureInstance)
        {
            logger.Info(string.Format("Initializing Database '{0}'", _adoTemplate.DbProvider.ConnectionString));
            if (createDatabase != null)
            {
                createDatabase();
            }

            logger.Info(string.Format("Executing Create Scripts '{0}'", _adoTemplate.DbProvider.ConnectionString));
            ExecuteScripts(Create);
            logger.Info(string.Format("Executing Load Scripts '{0}'", _adoTemplate.DbProvider.ConnectionString));
            ExecuteScripts(Load);

            return this;
        }

        private void ExecuteScripts(IEnumerable scripts)
        {
            foreach (object script in scripts)
            {
                string scriptText = null;
                try
                {
                    if (script is IResource)
                    {
                        scriptText = TestResourceLoader.GetText((IResource)script);
                    }
                    else if (script is string)
                    {
                        scriptText = (string)script;
                    }
                    else if (script is Action)
                    {
                        scriptText = null;
                        ((Action)script)();
                        continue;
                    }
                    else
                    {
                        throw new SystemException("Unknown sql script type " + script.GetType());
                    }
                }
                catch (Exception ex)
                {
                    throw new SystemException("Error loading SQL script " + script, ex);
                }

                try
                {
                    SimpleAdoTestUtils.ExecuteSqlScript(AdoTemplate, scriptText);
                }
                catch (Exception ex)
                {
                    throw new SystemException("Error executing SQL script " + script, ex);
                }
            }
        }

        public virtual void Dispose()
        {
            logger.Info(string.Format("Executing Delete Scripts '{0}'", _adoTemplate.DbProvider.ConnectionString));
            ExecuteScripts(Delete);
        }

        protected IResource Resource(string relativeUri)
        {
            return TestResourceLoader.GetResource(this, relativeUri);
        }

        protected Action Callback(Action action)
        {
            return action;
        }
    }
}
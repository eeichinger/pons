using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Common.Logging;
using NUnit.Framework;
using Spring.Core.IO;
using Viz.Testing.Services;
using Viz.Testing.Support;

namespace Viz.Testing
{
    public abstract class AbstractTest : IDisposable
    {
        protected readonly ILog Log;
        private readonly List<IDisposable> _registeredForTearDown = new List<IDisposable>();

        protected AbstractTest()
        {
            Log = LogManager.GetLogger(this.GetType());
        }

        protected virtual IEnumerable PerTestServices()
        {
            return null;
        }

        protected T Get<T>() where T : class
        {
            return Get<T>(true);
        }

        protected virtual T Get<T>(bool throwIfNull) where T : class
        {
            return GetClosestTypeMatch<T>(_registeredForTearDown, throwIfNull);
        }

        protected static T GetClosestTypeMatch<T>(IEnumerable objects, bool throwIfNull)
        {
            T closestMatch = default(T);
            bool foundMatch = false;

            foreach (IDisposable service in objects)
            {
                if (service.GetType() == typeof(T))
                {
                    return (T)service;
                }
                if (typeof(T).IsAssignableFrom(service.GetType()))
                {
                    closestMatch = (T)service;
                    foundMatch = true;
                }
            }

            if (foundMatch)
            {
                return closestMatch;
            }

            if (throwIfNull)
            {
                throw new SystemException("Service of type " + typeof(T) + " not registered");
            }
            return default(T);
        }

        [SetUp]
        public void SetUp()
        {
            try
            {
                OnRegisterPerTestServices();
                OnSetUp();
            }
            catch (Exception e)
            {
                try
                {
                    TearDown();
                }
                catch(Exception exception)
                {
                    Trace.WriteLine(exception);
                }
                throw;
            }
        }

        protected virtual void OnRegisterPerTestServices()
        {
            IEnumerable services = PerTestServices();
            if (services != null)
            {
                foreach( var service in services )
                {
                    if (service == null) continue;
                    RegisterForTearDown(service);
                }
            }
        }

        protected virtual void OnSetUp()
        { }

        [TearDown]
        public void TearDown()
        {
            try
            {
                OnTearDown();
            }
            finally
            {
                OnUnregisterPerTestServices();                
            }
        }

        protected virtual void OnTearDown()
        {}

        protected virtual void OnUnregisterPerTestServices()
        {
            _registeredForTearDown.Reverse();
            List<Exception> exceptions = new List<Exception>();
            _registeredForTearDown.ForEach((disposable) =>
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            });
            _registeredForTearDown.Clear();
            if (exceptions.Count > 0)
            {
                throw NewCompositeException("Exceptions occurred on TearDown", exceptions);
            }
        }

        protected Exception NewCompositeException(string message, List<Exception> exceptions)
        {
            StringBuilder sb = new StringBuilder();
            exceptions.ForEach((ex) =>
            {
                sb.Append(Environment.NewLine);
                sb.Append(ex);
            });
            throw new SystemException(string.Format("{0}: {1}", message, sb.ToString()));
        }

        protected void RegisterForTearDown(object service)
        {
            if (service is IServiceFactory)
            {
                service = ((IServiceFactory)service).GetService(this);
            }
            if (!(service is IDisposable))
            {
                throw new ArgumentException("Service must implement IDisposable", "service");
            }
            if(service is IEnumerable)
            {
                foreach(var element in ((IEnumerable)service))
                {
                    if (element == null) continue;
                    RegisterForTearDown(element);
                }
            }
            else
            {
                _registeredForTearDown.Add((IDisposable)service);
            }
        }

        public virtual void Dispose()
        {

        }

        protected IResource Resource(string relativeUri)
        {
            return TestResourceLoader.GetResource(this, relativeUri);
        }
    }
}
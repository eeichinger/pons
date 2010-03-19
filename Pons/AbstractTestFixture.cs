using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Pons.Services;

namespace Pons
{
    public abstract class AbstractTestFixture : AbstractTest
    {
        private readonly List<IDisposable> _registeredForFixtureTearDown = new List<IDisposable>();

        protected virtual IEnumerable PerFixtureServices()
        {
            return null;
        }

        protected override T Get<T>(bool throwIfNull)
        {
            T result = base.Get<T>(false);
            if (result != null)
            {
                return result;
            }

            return GetClosestTypeMatch<T>(_registeredForFixtureTearDown, throwIfNull);
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            try
            {
                OnRegisterPerFixtureServices();
                OnFixtureSetUp();
            }
            catch
            {
                try
                {
                    FixtureTearDown();
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }
                throw;
            }
        }

        protected virtual void OnRegisterPerFixtureServices()
        {
            IEnumerable services = PerFixtureServices();
            if (services != null)
            {
                foreach (var service in services)
                {
                    if (service == null) continue;
                    RegisterForFixtureTearDown(service);
                }
            }
        }

        protected virtual void OnFixtureSetUp()
        { }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            try
            {
                OnFixtureTearDown();
            }
            finally
            {
                OnUnregisterPerFixtureServices();                
            }
        }

        protected virtual void OnFixtureTearDown()
        {}

        protected virtual void OnUnregisterPerFixtureServices()
        {
            _registeredForFixtureTearDown.Reverse();
            List<Exception> exceptions = new List<Exception>();
            _registeredForFixtureTearDown.ForEach((disposable) =>
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
            _registeredForFixtureTearDown.Clear();
            if (exceptions.Count > 0)
            {
                throw NewCompositeException("Exceptions occurred on FixtureTearDown", exceptions);
            }
        }

        protected void RegisterForFixtureTearDown(object service)
        {
            if (service is IServiceFactory)
            {
                service = ((IServiceFactory) service).GetService(this);
            }

            if (service is IEnumerable)
            {
                foreach(var element in (IEnumerable) service)
                {
                    if (element == null) continue;
                    RegisterForFixtureTearDown(element);
                }
                return;
            }
            
            if (!(service is IDisposable))
            {
                throw new ArgumentException("Service must implement IDisposable", "service");
            }
            
            _registeredForFixtureTearDown.Add((IDisposable) service);
        }
    }
}
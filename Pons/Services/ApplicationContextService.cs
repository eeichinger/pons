using System;
using Spring.Context;
using Spring.Objects.Factory.Config;

namespace Pons.Services
{
    public class ApplicationContextService : IServiceFactory
    {
        private Func<IConfigurableApplicationContext> instantiateContext;
        private AutoWiringMode autowireMode = AutoWiringMode.ByType;
        private bool dependencyCheck = true;

        public ApplicationContextService(Func<IConfigurableApplicationContext> instantiateContext)
        {
            this.instantiateContext = instantiateContext;
        }

        /// <summary>
        /// Gets or sets the autowire mode for test properties set by Dependency Injection.
        /// </summary>
        /// <value>
        /// The autowire mode for test properties set by Dependency Injection.
        /// The default is <see cref="AutoWiringMode.ByType"/>.
        /// </value>
        public AutoWiringMode AutowireMode
        {
            get { return autowireMode; }
            set { autowireMode = value; }
        }

        /// <summary>
        /// Gets or sets a flag specifying whether or not dependency checking 
        /// should be performed for test properties set by Dependency Injection.
        /// </summary>
        /// <value>
        /// <p>A flag specifying whether or not dependency checking 
        /// should be performed for test properties set by Dependency Injection.</p>
        /// <p>The default is <b>true</b>, meaning that tests cannot be run
        /// unless all properties are populated.</p>
        /// </value>
        public bool DependencyCheck
        {
            get { return dependencyCheck; }
            set { dependencyCheck = value; }
        }

        public IDisposable GetService(object fixtureInstance)
        {
            var applicationContext = instantiateContext();
            applicationContext.Refresh();
            IConfigurableListableObjectFactory factory = (IConfigurableListableObjectFactory)applicationContext.ObjectFactory;
            factory.IgnoreDependencyType(typeof(AutoWiringMode));
            factory.AutowireObjectProperties(fixtureInstance, AutowireMode, DependencyCheck);
            return applicationContext;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Spring.Objects;
using Spring.Objects.Factory.Config;

namespace Pons
{
    public class MoqAutoMockingPostProcessor : AbstractCustomDependencyResolverPostProcessor
    {
        private MockBehavior defaultMockBehavior = MockBehavior.Default;
        private readonly Dictionary< Func<string, IObjectDefinition, IObjectWrapper, PropertyInfo, bool>, MockBehavior> mockBehaviorOverrides = new Dictionary<Func<string, IObjectDefinition, IObjectWrapper, PropertyInfo, bool>, MockBehavior>();

        public MockBehavior DefaultMockBehavior
        {
            get { return defaultMockBehavior; }
            set { defaultMockBehavior = value; }
        }

        public Dictionary<Func<string, IObjectDefinition, IObjectWrapper, PropertyInfo, bool>, MockBehavior> MockBehaviorOverrides
        {
            get { return mockBehaviorOverrides; }
        }

        protected override object ResolveDependency(string objectName, IObjectDefinition objectDefinition, IObjectWrapper objectWrapper, PropertyInfo propertyInfo)
        {
            if (!propertyInfo.PropertyType.IsInterface)
            {
                return null;
            }
            Type mockType = typeof(Mock).MakeGenericType(propertyInfo.PropertyType);
            Mock mock = (Mock)Activator.CreateInstance(mockType, defaultMockBehavior);
            return mock.Object;
        }        
    }
}

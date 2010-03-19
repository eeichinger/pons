using System.Reflection;
using Spring.Objects;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

namespace Pons
{
    public abstract class AbstractCustomDependencyResolverPostProcessor : InstantiationAwareObjectPostProcessorAdapter, IObjectFactoryAware
    {
        private IObjectDefinitionRegistry _objectFactory;

        public IObjectFactory ObjectFactory
        {
            set
            {
                _objectFactory = (IObjectDefinitionRegistry) value;
            }
        }

        public override IPropertyValues PostProcessPropertyValues(IPropertyValues propertyValues, PropertyInfo[] propertyInfos, object objectInstance, string objectName)
        {
            MutablePropertyValues mpv = new MutablePropertyValues(propertyValues);

            IConfigurableObjectDefinition objectDefinition = (IConfigurableObjectDefinition)_objectFactory.GetObjectDefinition(objectName);
            DependencyCheckingMode checkMode = objectDefinition.DependencyCheck;
            PropertyInfo[] unresolvedProperties = AutowireUtils.GetUnsatisfiedDependencies(propertyInfos, propertyValues, checkMode);
            foreach(PropertyInfo propertyInfo in unresolvedProperties)
            {
                object value = ResolveDependency(objectName, objectDefinition, new ObjectWrapper(objectInstance), propertyInfo);
                if (value != null)
                {
                    mpv.Add(propertyInfo.Name, value);
                }
            }

            return base.PostProcessPropertyValues(mpv, propertyInfos, objectInstance, objectName);
        }

        protected abstract object ResolveDependency(string objectName, IObjectDefinition objectDefinition, IObjectWrapper objectWrapper, PropertyInfo propertyInfo);
    }
}
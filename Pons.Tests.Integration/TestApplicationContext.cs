using Spring.Context;
using Spring.Context.Support;
using Spring.Data.Common;
using Spring.Data.Core;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;

namespace Pons
{
    public class TestApplicationContext : GenericApplicationContext
    {
        public TestApplicationContext(bool caseSensitive) : base(caseSensitive)
        {
            IConfigurableApplicationContext appCtx = this;
            appCtx.AddObjectFactoryPostProcessor(
                new VariablePlaceholderConfigurer(new IVariableSource
                                                      []
                                                      {
                                                          new ConnectionStringsVariableSource
                                                              (),
                                                          new ConfigSectionVariableSource
                                                              ("appSettings")
                                                      }));
            ObjectDefinitionBuilder odb = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(DbProviderFactoryObject));
            odb.AddPropertyValue("ConnectionString", "${Tests.ConnectionString}");
            odb.AddPropertyValue("Provider", "SqlServer-2.0");
            appCtx.ObjectFactory.RegisterObjectDefinition("DbProvider", odb.ObjectDefinition);

            odb = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(AdoPlatformTransactionManager));
            odb.AddConstructorArgReference("DbProvider");
            appCtx.ObjectFactory.RegisterObjectDefinition("TransactionManager", odb.ObjectDefinition);
        }
    }
}
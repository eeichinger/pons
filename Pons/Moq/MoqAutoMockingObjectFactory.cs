using Spring.Objects.Factory.Support;

namespace Pons
{
    public class MoqAutoMockingObjectFactory : DefaultListableObjectFactory
    {
        public MoqAutoMockingObjectFactory()
        {
            this.AddObjectPostProcessor(new MoqAutoMockingPostProcessor());
        }
    }
}
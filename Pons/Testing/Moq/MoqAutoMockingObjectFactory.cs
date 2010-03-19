using Spring.Objects.Factory.Support;

namespace Viz.Testing
{
    public class MoqAutoMockingObjectFactory : DefaultListableObjectFactory
    {
        public MoqAutoMockingObjectFactory()
        {
            this.AddObjectPostProcessor(new MoqAutoMockingPostProcessor());
        }
    }
}
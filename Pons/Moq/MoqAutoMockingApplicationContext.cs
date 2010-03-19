using System.Collections;
using Spring.Context.Support;
using Spring.Util;

namespace Pons
{
    public class MoqAutoMockingApplicationContext : GenericApplicationContext
    {
        public MoqAutoMockingApplicationContext()
            :base(new MoqAutoMockingObjectFactory())
        {}

        public T Get<T>() where T:class
        {
            IDictionary dictionary = base.GetObjectsOfType(typeof (T));
            if (dictionary.Count > 0)
            {
                return (T) ObjectUtils.EnumerateFirstElement(dictionary.Values);                
            }
            return null;
        }
    }
}

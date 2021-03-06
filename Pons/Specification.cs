using System;
using System.Linq;
using NUnit.Framework;

namespace Pons
{
    [Specification]
    public abstract class Specification
    {
        [SetUp]
        public void SetUp()
        {
            SetupGiven(this);
            SetupWhen(this);
        }

        [Given]
        public abstract void Given();

        [When]
        public abstract void When();

        [TearDown]
        public void TearDown()
        {
            Cleanup();
        }

        protected virtual void Cleanup()
        {   }

        public static void SetupGiven(object specification)
        {
            var methods = specification.GetType().GetMethods();
            var givens = from m in methods
                         where m.IsDefined(typeof (GivenAttribute), true)
                         orderby ((GivenAttribute) Attribute.GetCustomAttribute(m, typeof (GivenAttribute))).Priority descending
                         select (Action) Delegate.CreateDelegate(typeof (Action), specification, m);
            givens.All(m => { m(); return true; });
//            var list = givens.ToList();
//            list.ForEach(m=>m());
        }

        public static void SetupWhen(object specification)
        {
            var whens = from m in specification.GetType().GetMethods()
                        where m.IsDefined(typeof(WhenAttribute), true)
                        orderby ((WhenAttribute)Attribute.GetCustomAttribute(m, typeof(WhenAttribute))).Priority descending
                        select (Action)Delegate.CreateDelegate(typeof(Action), specification, m);
            whens.All(m => { m(); return true; });
        }
    }

    public class SpecificationAttribute : TestFixtureAttribute
    { }

    [AttributeUsage(AttributeTargets.Method)]
    public class GivenAttribute : Attribute
    {
        public int Priority { get; set; }

        public GivenAttribute():this(0)
        {
        }

        public GivenAttribute(int order)
        {
            Priority = order;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ThenAttribute : TestAttribute
    { }

    [AttributeUsage(AttributeTargets.Method)]
    public class WhenAttribute : Attribute
    {
        public int Priority { get; set; }

        public WhenAttribute():this(0)
        {
        }

        public WhenAttribute(int order)
        {
            Priority = order;
        }        
    }
}
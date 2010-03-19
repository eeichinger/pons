namespace Pons
{
    public abstract class SpecificationWithServices : AbstractTestFixture
    {
        protected override void OnSetUp()
        {
            Specification.SetupGiven(this);
            Specification.SetupWhen(this);
        }

        [Given]
        public abstract void Given();

        [When]
        public abstract void When();

        protected override void OnTearDown()
        {
            base.OnTearDown();
            Cleanup();
        }

        protected virtual void Cleanup()
        {
            
        }
    }
}
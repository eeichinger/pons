namespace Viz.Testing
{
    public abstract class SpecificationWithServices : AbstractTestFixture
    {
        protected override void OnSetUp()
        {
            Specification.SetupGiven(this);
            Specification.SetupWhen(this);
        }

        [Given]
        protected abstract void Given();

        [When]
        protected abstract void When();

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
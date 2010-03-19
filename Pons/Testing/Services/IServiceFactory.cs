using System;

namespace Viz.Testing.Services
{
    public interface IServiceFactory
    {
        IDisposable GetService(object fixtureInstance);
    }
}
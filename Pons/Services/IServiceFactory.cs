using System;

namespace Pons.Services
{
    public interface IServiceFactory
    {
        IDisposable GetService(object fixtureInstance);
    }
}
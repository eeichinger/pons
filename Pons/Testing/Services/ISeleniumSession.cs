using System;
using Selenium;

namespace Viz.Testing.Services
{
    public interface ISeleniumSession : ISelenium, IDisposable
    {}
}
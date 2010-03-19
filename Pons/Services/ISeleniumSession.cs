using System;
using Selenium;

namespace Pons.Services
{
    public interface ISeleniumSession : ISelenium, IDisposable
    {}
}
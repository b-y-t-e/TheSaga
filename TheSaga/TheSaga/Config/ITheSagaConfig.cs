using Microsoft.Extensions.DependencyInjection;
using System;

namespace TheSaga.Config
{
    public interface ITheSagaConfig
    {
        IServiceCollection Services { get; }
    }
}
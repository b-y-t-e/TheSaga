using Microsoft.Extensions.DependencyInjection;

namespace TheSaga.Config
{
    public interface ITheSagaConfig
    {
        IServiceCollection Services { get; }
    }
}
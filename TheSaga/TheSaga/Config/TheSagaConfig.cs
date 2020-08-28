using Microsoft.Extensions.DependencyInjection;

namespace TheSaga.Config
{
    public class TheSagaConfig : ITheSagaConfig
    {
        public IServiceCollection Services { get; internal set; }
    }
}
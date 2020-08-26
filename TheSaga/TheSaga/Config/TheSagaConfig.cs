using Microsoft.Extensions.DependencyInjection;

namespace TheSaga.Config
{
    public class TheSagaConfig : ITheSagaConfig
    {
        public TheSagaConfig()
        {
        }

        public IServiceCollection Services { get; internal set; }
    }
}
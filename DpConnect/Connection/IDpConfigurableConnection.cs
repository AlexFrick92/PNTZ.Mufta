

namespace DpConnect.Connection
{
    public interface IDpConfigurableConnection<TConnectionConfiguration> : IDpConnection
        where TConnectionConfiguration : IDpConnectionConfiguration
    {
        void Configure(TConnectionConfiguration configuration);
    }
}

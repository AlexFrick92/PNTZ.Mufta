
namespace DpConnect.Connection
{
    public interface IDpActionSource : IDpStatus
    {
        SourceDelegate SourceDelegate { get; set; }
    }
}

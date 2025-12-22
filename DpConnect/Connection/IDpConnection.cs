


namespace DpConnect.Connection
{
    public interface IDpConnection
    { 

        string Id { get; }

        bool Active { get; }

        void Open();
        void Close();

    }   
}

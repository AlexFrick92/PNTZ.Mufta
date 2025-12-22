using Opc.Ua;

namespace Promatis.Opc.UA.Client
{
    public class TestClient:ClientBase
    {
        public TestClient(ITransportChannel channel) : base(channel)
        {
        }
    }
}
using System.Net;

namespace MessageChannel.Abstractions
{
    public interface IApplicationReceiver
    {
        bool Receive();
    }
}

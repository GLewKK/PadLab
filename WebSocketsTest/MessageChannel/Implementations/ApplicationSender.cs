using System.Net.Sockets;
using MessageChannel.Abstractions;

namespace MessageChannel.Implementations
{
    public class ApplicationSender : IApplicationSender
    {
        private readonly IMessageBroker _messageBroker = new MessageBroker();
        public void Send(string text)
        {

        }
    }
}

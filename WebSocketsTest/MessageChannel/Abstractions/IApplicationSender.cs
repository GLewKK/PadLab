namespace MessageChannel.Abstractions
{
    public interface IApplicationSender
    {
        void Send(string text);
    }
}

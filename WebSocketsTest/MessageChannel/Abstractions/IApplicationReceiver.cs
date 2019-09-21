namespace MessageChannel.Abstractions
{
    public interface IApplicationReceiver
    {
        void Send(string text);
    }
}

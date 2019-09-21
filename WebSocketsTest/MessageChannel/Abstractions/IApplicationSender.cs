namespace MessageChannel.Abstractions
{
    public interface IApplicationSender
    {
        void Send(byte[] byteText);
    }
}

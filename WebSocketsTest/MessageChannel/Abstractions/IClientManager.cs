namespace MessageChannel.Abstractions
{
    public interface IClientManager
    {
        void ConnectToServer();
        void SetNetworkStream();
        void SetThread();
        void DisconnectFromServer();
        void SetName(string name);
        void SendMessage(string text);
    }
}

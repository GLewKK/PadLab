namespace MessageChannel.Abstractions
{
    public interface IAppUserConfigurator
    {
        string RegisterUser(string username);
        void ConnectToServer();
    }
}
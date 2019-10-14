namespace MergedServer.Entities
{
    public static class UserCommand
    {
        public const string CreateChannel = "createchannel";

        public const string Publish = "publish";

        public const string Subscribe = "subscribe";

        public const string Disconnect = "disconnectuser";

        public const string NotifyConnect = "notifyonconnect";

        public const string UnSubscribe = "unsubscribe";

        public const string PublishShortCommand = "p";

        public const string SubscribeShortCommand = "sub";

        public const string UnSubscribeShortCommand = "unsub";

        public const string NotifyReconnect = "notifyreconnect";
    }
}
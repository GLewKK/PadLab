using System.Collections.Generic;
using MergedServer.Entities;
using static MergedServer.Program;

namespace MergedServer.Extensions
{
    public static class MessageProviderNotificatorExtension
    {
        public static List<MessageTarget> NotifyConnect(this MessageProvider user)
        {
            return new List<MessageTarget>
            {
                new MessageTarget
                {
                    Message = $"{user.Name} connected!",
                    Targets = Users.GetAllUsersExceptOne(user)
                }
            };
        }
        public static List<MessageTarget> NotifyReconnect(this MessageProvider user)
        {
            return new List<MessageTarget>
            {
                new MessageTarget
                {
                    Message = $"{user.Name} reconnected!",
                    Targets = Users.GetAllUsersExceptOne(user)
                }
            };
        }

        public static List<MessageTarget> NotifySubscribe(this MessageProvider user, MessageProvider publisher, string channelName)
        {
            var targets = publisher.Subscribers.GetAllUsersExceptOne(user);

            var result = new List<MessageTarget>
            {
                new MessageTarget
                {
                    Message = $"{user.Name} subscribed to {channelName}", 
                    Targets = targets
                },
                new MessageTarget
                {
                    Message = $"{user.Name} subscribed to your channel",
                    Targets = new List<MessageProvider> {publisher}
                }
            };

            return result;
        }
        public static List<MessageTarget> NotifyUnsubscribe(this MessageProvider user, MessageProvider publisher, string channelName)
        {
            var targets = publisher.Subscribers.GetAllUsersExceptOne(user);

            var result = new List<MessageTarget>
            {
                new MessageTarget
                {
                    Message = $"{user.Name} unsubscribed to {channelName}", 
                    Targets = targets
                },
                new MessageTarget
                {
                    Message = $"{user.Name} unsubscribed to your channel",
                    Targets = new List<MessageProvider> {publisher}
                }
            };

            return result;
        }

    }
}

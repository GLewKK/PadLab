using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MergedServer.Entities;
using static MergedServer.Program;

namespace MergedServer.Extensions
{
    public static class MessageProviderExtension
    {
        public static List<MessageTarget> CreateChannel(this MessageProvider user, string channelName)
        {
            if(!string.IsNullOrEmpty(channelName))
                user.Channel = channelName;

            return new List<MessageTarget>
            {
                new MessageTarget
                {
                    Targets = new List<MessageProvider> { user },
                    Message = string.IsNullOrEmpty(channelName) ? "Invalid channel name" : "Channel created"
                }
            };
        }

        public static List<MessageTarget> Disconnect(this MessageProvider user)
        {
            user.IsActive = false;

            return new List<MessageTarget>
            {
                new MessageTarget
                {
                    Targets = new List<MessageProvider> { user },
                    Message = "Successfully disconnected"
                },
                new MessageTarget
                {
                    Targets = Users.GetAllUsersExceptOne(user),
                    //Targets = Users.Where(x => x.Id != user.Id).ToList(),
                    Message = $"{user.Name} disconnected"
                }
            };
        }

        public static List<MessageTarget> Publish(this MessageProvider user, string message)
        {
            if (string.IsNullOrEmpty(user.Channel))
            {
                return new List<MessageTarget>
                {
                    new MessageTarget
                    {
                        Targets = new List<MessageProvider> {user},
                        Message = "Channel is not existing!"
                    }
                };
            }

            if (string.IsNullOrEmpty(message))
            {
                return new List<MessageTarget>
                {
                    new MessageTarget
                    {
                        Targets = new List<MessageProvider>(),
                        Message = string.Empty
                    }
                };
            }

            var filePath = @"C:\Users\glewk\source\repos\PadLab\WebSocketsTest\logs\";
            StringBuilder sb = new StringBuilder();
            sb.Append($"{user.Name} -> {user.Channel}: {message}" + Environment.NewLine);

            File.AppendAllText(filePath + $"{DateTime.Now:dd_MM_yyyy}.txt", sb.ToString());
            sb.Clear();
            return new List<MessageTarget>
            {
                new MessageTarget
                {
                    Targets = user.Subscribers,
                    Message = $"{user.Name} -> {user.Channel}: {message}"
                }
            };
        }

        public static List<MessageTarget> Subscribe(this MessageProvider user, string channelName)
        {
            var returnMessage = string.Empty;
            var publisher = Users.FirstOrDefault(x => x.Channel.Equals(channelName));
            var targets = new List<MessageTarget>();

            if (publisher is null)
            {
                returnMessage = "Channel doesn't not exist!";
            }
            else
            {
                if (publisher.Id.Equals(user.Id))
                {
                    returnMessage = "You cannot subscribe to your channel";
                }
                else
                {
                    publisher.Subscribers.Add(user);
                    targets = user.NotifySubscribe(publisher,channelName);
                }
            }

            targets.Add(new MessageTarget
            {
                Message = string.IsNullOrEmpty(returnMessage) ? "Successfully subscribed!" : returnMessage,
                Targets = new List<MessageProvider> { user }
            });

            return targets;
        }

        public static List<MessageTarget> Unsubscribe(this MessageProvider user, string channelName)
        {
            var returnMessage = string.Empty;
            var publisher = Users.FirstOrDefault(x => x.Channel.Equals(channelName));
            var targets = new List<MessageTarget>();

            if (publisher is null)
            {
                returnMessage = "Channel doesn't not exist!";
            }
            else
            {
                if (publisher.Id.Equals(user.Id))
                {
                    returnMessage = "You cannot unsubscribe to your channel";
                }
                else
                {
                    publisher.Subscribers.Remove(user);
                    targets = user.NotifyUnsubscribe(publisher, channelName);
                }
            }
            targets.Add(new MessageTarget
            {
                Message = string.IsNullOrEmpty(returnMessage) ? "Successfully unsubscribed!" : returnMessage,
                Targets = new List<MessageProvider> { user }
            });

            return targets;
        }

        public static List<MessageTarget> NotFound(this MessageProvider user)
        {
            return new List<MessageTarget>
            {
                new MessageTarget
                {
                    Targets = new List<MessageProvider> {user},
                    Message = "Command not recognized"
                }
            };
        }

        public static List<MessageTarget> SendMessage(this MessageProvider user, string message)
        {
            return new List<MessageTarget>
            {
                new MessageTarget
                {
                    Targets = Users.GetAllUsersExceptOne(user),
                    Message = message
                }
            };
        }

        public static void AddLostMessage(this MessageProvider user, string message)
        {
            user.LostMessages.Add(new LostMessage
            {
                Message = message,
                ReceivedDate = DateTime.Now
            });
        }

    }
}
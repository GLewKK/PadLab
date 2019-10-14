using System.Collections.Generic;
using System.Linq;
using MergedServer.Entities;

namespace MergedServer.Extensions
{
    public static class ListMessageProvidersExtension
    {
        public static List<MessageProvider> GetAllUsersExceptOne(this List<MessageProvider> usersList, MessageProvider user)
        {
            return usersList.Where(x => x.Id != user.Id).ToList();
        }
    }
}

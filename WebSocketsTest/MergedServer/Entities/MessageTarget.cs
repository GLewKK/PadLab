using System.Collections.Generic;

namespace MergedServer.Entities
{
    public class MessageTarget
    {
        public List<MessageProvider> Targets { get; set; }
        public string Message { get; set; }
    }
}
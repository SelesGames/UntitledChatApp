using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UntitledChatApp
{
    public class ChatUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string CurrentRoom { get; set; }
    }
}
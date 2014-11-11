using System;

namespace UntitledChatApp.Core.Graph
{
    public class UserNode : Node
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }

        public UserNode()
        {
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return UserName;
        }
    }
}
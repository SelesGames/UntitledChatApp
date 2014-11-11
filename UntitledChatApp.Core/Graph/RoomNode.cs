using System;

namespace UntitledChatApp.Core.Graph
{
    public class RoomNode : Node
    {
        public string Id { get; set; }

        public RoomNode()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public override string ToString()
        {
            return string.Format("{0}: users: {1}, mid: {2}", Id, Children.Count, MidPoint.midpoint);
        }

        public static RoomNode DefaultRoom = new RoomNode { MidPoint = WeightedCartesianCoordinates.Empty };
    }
}
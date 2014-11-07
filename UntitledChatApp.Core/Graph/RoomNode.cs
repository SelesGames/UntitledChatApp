using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UntitledChatApp.Core.Graph
{
    public class RoomNode : Node
    {
        public static RoomNode DefaultRoom = new RoomNode { MidPoint = WeightedCartesianCoordinates.Empty };
    }
}
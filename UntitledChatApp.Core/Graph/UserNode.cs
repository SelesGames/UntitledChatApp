using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UntitledChatApp.Core.Graph
{
    public class UserNode : Node
    {
        public string Address { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}

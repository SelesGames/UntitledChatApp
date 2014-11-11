using System;
using UntitledChatApp.Core.Graph;

namespace UntitledChatApp.Core
{
    public class RoomTree
    {
        public static RoomTree Instance = new RoomTree();

        public Node root;

        public RoomTree()
        {
            root = new Node();
            root.AddChild(RoomNode.DefaultRoom);
        }




        #region Public Add User function

        /// <summary>
        /// Add a user by traversing the node tree until you find a RoomNode, then 
        /// add user to the room.
        /// </summary>
        public void AddUser(UserNode user)
        {
            var node = FindClosestRoom(user);
            node.AddChild(user);
        }

        public RoomNode FindClosestRoom(UserNode user)
        {
            if (user == null) throw new ArgumentNullException("user");

            var node = root;
            // traverse the tree until you find a RoomNode
            while (!(node is RoomNode))
            {
                node = node.Children.FindNearestTo(user);
            }
            return (RoomNode)node;
        }

        #endregion
    }
}
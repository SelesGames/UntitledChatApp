using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core
{
    public class NodeCollection
    {
        readonly List<Node> nodes;

        public WeightedCartesianCoordinatesAggregate MidPoint { get; private set; }

        public NodeCollection()
        {
            this.nodes = new List<Node>();
        }

        public void Add(Node node)
        {
            nodes.Add(node);
            MidPoint = MidPoint.Add(node.MidPoint.midpoint);
        }

        public void Remove(Node node)
        {
            nodes.Remove(node);
            MidPoint = MidPoint.Subtract(node.MidPoint.midpoint);
        }
    }

    public class UserNode : Node
    {
    }

    public class RoomNode : Node
    {
    }

    public class Node
    {
        readonly protected int MAX = 60;
        readonly protected int TARGET = 30;
        readonly protected int MIN = 15;

        public WeightedCartesianCoordinatesAggregate MidPoint { get; set; }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }




        #region Public Add User function

        /// <summary>
        /// Add a user by traversing the node tree until you find a RoomNode, then 
        /// add user to the room.
        /// </summary>
        public void AddUser(UserNode user)
        {
            if (user == null) throw new ArgumentNullException("user");

            var node = this;
            // traverse the tree until you find a RoomNode
            while (!(node is RoomNode))
            {
                node = node.Children.FindNearestTo(user);
            }
            node.AddChild(user);
            node.EvaluateChildCount();
        }

        #endregion




        #region Helper functions

        IEnumerable<Node> GetSiblings()
        {
            if (Parent == null)
                return new List<Node>();

            return Parent.Children.Except(new[] { this });
        }

        /// <summary>
        /// Evaluate the number of child nodes, reacting if over MAX or under MIN
        /// </summary>
        void EvaluateChildCount()
        {
            if (Children.Count >= MAX)
                SplitThisNode();

            else if (Children.Count <= MIN)
                Dissolve();
        }

        /// <summary>
        /// Distribute Children to this node's siblings, according to distance
        /// </summary>
        static void DistributeNodesToNearestNodes(IEnumerable<Node> nodesToDistribute, IEnumerable<Node> targets)
        {
            if (nodesToDistribute == null) throw new ArgumentNullException("nodesToDistribute");
            if (targets == null) throw new ArgumentNullException("targets");

            var nodeAndNearestTargetTuples = nodesToDistribute
                .AsParallel()                           // operation parallelizes well
                .Select(node => new
                {
                    node,
                    nearestTarget = targets.FindNearestTo(node)
                });

            foreach (var tuple in nodeAndNearestTargetTuples)
            {
                var nearestTarget = tuple.nearestTarget;
                var node = tuple.node;
                nearestTarget.AddChild(node);
            }
        }

        #endregion




        #region Add a node to Children, recalculating midpoint, and splitting this node in two if there are too many children

        /// <summary>
        /// Add a node to Children and re-calculate the midpoint.  If there are 
        /// too many children after adding, then split this node in twain.
        /// </summary>
        void AddChild(Node node)
        {
            Children.Add(node);
            node.Parent = this;
            MidPoint = MidPoint.Add(node.MidPoint.midpoint);

            if (Children.Count >= MAX)
                SplitThisNode();
        }

        void SplitThisNode()
        {
            var furthestNodes = RemoveFurthestChildren();

            // 1: create a temporary node (if this is a RoomNode, make a new RoomNode)
            // 2: assign temp node's midpoint to the midpoint of the furthest nodes
            var tempNode = (this is RoomNode) ? new RoomNode() : new Node();
            tempNode.MidPoint = furthestNodes.GetGeographicMidpoint();

            // 3: assign each furthest child node to one of this node's siblings, 
            //    or to temp node, based on distance
            DistributeNodesToNearestNodes(
                nodesToDistribute: furthestNodes, 
                targets: GetSiblings().Union(new[] { tempNode }).ToList());

            // 4: reset tempNode's midpoint
            tempNode.MidPoint = tempNode.Children.GetGeographicMidpoint();

            // 5: if tempNode is less than MIN, move it's children to siblings, 
            //    otherwise, add tempNode to the Parent's children
            if (tempNode.Children.Count <= TARGET)
            {
                DistributeNodesToNearestNodes(
                    nodesToDistribute: tempNode.Children,
                    targets: GetSiblings().ToList());
            }
            else
            {
                Parent.AddChild(tempNode);
            }
        }

        /// <summary>
        /// Remove the "bottom" or furthest N children, where N is the ideal "Target"
        /// size of the room.
        /// </summary>
        /// <returns>The child nodes that were removed.</returns>
        IEnumerable<Node> RemoveFurthestChildren()
        {
            var currentMidpoint = MidPoint.midpoint;

            var furthestNodes = Children
                .OrderyByDistanceTo(currentMidpoint, NodeDistanceOrderType.Furthest)
                .Take(TARGET)
                .ToList();

            // remove the furthest child nodes
            foreach (var node in furthestNodes)
                InnerRemoveChild(node);

            return furthestNodes;
        }

        #endregion




        #region Remove a Node

        /// <summary>
        /// Remove a Node from the Children collection.  Distribute the Nodes's child nodes
        /// amongst the remaining sibling Nodes, according to distance
        /// </summary>
        /// <param name="room">The ChatRoom to remove</param>
        public void RemoveChild(Node node)
        {
            InnerRemoveChild(node);

            // distribute all node's child nodes to the sibling nodes
            DistributeNodesToNearestNodes(
                nodesToDistribute: node.Children, 
                targets: Children);

            // if we've hit the min threshold, remove this node from it's parent,
            // which will in turn distribute this node's children to it's siblings
            if (Children.Count <= MIN)
                Parent.Remove(this);// Dissolve();
        }

        /// <summary>
        /// Remove a node from Children and re-calculate the midpoint
        /// </summary>
        void InnerRemoveChild(Node node)
        {
            Children.Remove(node);
            MidPoint = MidPoint.Subtract(node.MidPoint.midpoint);
        }

        /// <summary>
        /// Remove this room from the parent node - which will in turn distribute every 
        /// child node to one of the sibling nodes, based on distance
        /// </summary>
        void Dissolve()
        {
            // if you remove this room and there are no other rooms, this would be a bug
            Parent.RemoveChild(this);
        }

        #endregion
    }
}
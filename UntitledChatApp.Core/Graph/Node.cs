using System;
using System.Collections.Generic;
using System.Linq;

namespace UntitledChatApp.Core.Graph
{
    public class Node
    {
        readonly protected int MAX = 400;
        readonly protected int TARGET = 2;
        readonly protected int MIN = 1;

        public WeightedCartesianCoordinates MidPoint { get; set; }
        public List<Node> Children { get; private set; }
        public Node Parent { get; private set; }

        public Node()
        {
            Children = new List<Node>();
        }




        //#region Public Add User function

        ///// <summary>
        ///// Add a user by traversing the node tree until you find a RoomNode, then 
        ///// add user to the room.
        ///// </summary>
        //public void AddUser(UserNode user)
        //{
        //    if (user == null) throw new ArgumentNullException("user");

        //    var node = this;
        //    // traverse the tree until you find a RoomNode
        //    while (!(node is RoomNode))
        //    {
        //        node = node.Children.FindNearestTo(user);
        //    }
        //    node.AddChild(user);
        //}

        //#endregion




        #region Helper functions

        IEnumerable<Node> GetSiblings()
        {
            if (Parent == null)
                return new List<Node>();

            return Parent.Children.Except(new[] { this });
        }

        /// <summary>
        /// Distribute Children to this node's siblings, according to distance
        /// </summary>
        static void DistributeNodesToNearestNodes(IEnumerable<Node> nodesToDistribute, IEnumerable<Node> targets)
        {
            if (nodesToDistribute == null) throw new ArgumentNullException("nodesToDistribute");
            if (targets == null) throw new ArgumentNullException("targets");

            var targetsArray = targets.ToArray();

            var nodeAndNearestTargetTuples = nodesToDistribute
                .AsParallel()                           // operation parallelizes well
                .Select(node => new
                {
                    node,
                    nearestTarget = targetsArray.FindNearestTo(node)
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
        internal void AddChild(Node node)
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
                targets: GetSiblings().Union(new[] { tempNode }));

            // 4: reset tempNode's midpoint
            tempNode.MidPoint = tempNode.Children.GetGeographicMidpoint();

            // 5: if tempNode is less than MIN, move it's children to siblings, 
            //    otherwise, add tempNode to the Parent's children
            if (tempNode.Children.Count < TARGET)
            {
                DistributeNodesToNearestNodes(
                    nodesToDistribute: tempNode.Children,
                    targets: GetSiblings());
            }
            else
            {
                // check to see if the parent is the root node
                if (Parent == null)
                {
                    // create a new root node.  The root will never be a RoomNode
                    var newRoot = new Node();
                    Parent = newRoot;
                }
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
                RemoveChild(node);

            return furthestNodes;
        }

        #endregion




        #region Remove a Node

        /// <summary>
        /// Remove a node from Children and re-calculate the midpoint
        /// </summary>
        void RemoveChild(Node node)
        {
            Children.Remove(node);
            MidPoint = MidPoint.Subtract(node.MidPoint.midpoint);

            if (Children.Count <= MIN)
                DestroyThisNode();
        }

        /// <summary>
        /// Destroy this node - first remove it from its parent's children, then 
        /// distribute all of this node's children amongst its siblings.
        /// </summary>
        void DestroyThisNode()
        {
            // never destroy the root node of the default room node
            if (Parent == null || this == RoomNode.DefaultRoom)
                return;

            DistributeNodesToNearestNodes(
                nodesToDistribute: Children,
                targets: GetSiblings());
            Parent.RemoveChild(this);
        }

        #endregion
    }
}
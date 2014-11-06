using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public static class NodeExtensions
    {
        public static IEnumerable<Node> OrderyByDistanceTo(this IEnumerable<Node> nodes, CartesianCoordinates coords, NodeDistanceOrderType orderType)
        {
            if (nodes == null) throw new ArgumentNullException("nodes");

            var tuples = nodes
                .Select(node =>
                    new
                    {
                        node,
                        dist = GPSMath.DistanceFastCalc(coords, node.MidPoint.midpoint),
                    });

            if (orderType == NodeDistanceOrderType.Nearest)
                return tuples.OrderBy(o => o.dist).Select(o => o.node);

            else if (orderType == NodeDistanceOrderType.Furthest)
                return tuples.OrderByDescending(o => o.dist).Select(o => o.node);

            else throw new ArgumentException(string.Format("unsupported OrderType: {0}", orderType), "orderType");
        }

        public static Node FindNearestTo(this IEnumerable<Node> nodes, CartesianCoordinates coords)
        {
            if (nodes == null || !nodes.Any())
                throw new ArgumentException("can't find nearest if rooms is empty or null", "rooms");

            return OrderyByDistanceTo(nodes, coords, NodeDistanceOrderType.Nearest).First();
        }

        public static Node FindNearestTo(this IEnumerable<Node> nodes, Node node)
        {
            return FindNearestTo(nodes, node.MidPoint.midpoint);
        }

        public static WeightedCartesianCoordinatesAggregate GetGeographicMidpoint(this IEnumerable<Node> nodes)
        {
            return nodes.Select(o => o.MidPoint.midpoint).ToArray().GetGeographicMidpoint();
        }
    }

    public enum NodeDistanceOrderType
    {
        Nearest,
        Furthest
    }
}
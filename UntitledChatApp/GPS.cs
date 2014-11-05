using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core
{
    struct DecimalCoordinates
    {
        public double decimalLatitude;
        public double decimalLongitude;
    }

    struct RadiansCoordinates
    {
        public double radiansLatitude;
        public double radiansLongitude;
    }

    struct CartesianCoordinates
    {
        public double x;
        public double y;
        public double z;

        public static implicit operator WeightedCartesianCoordinates(CartesianCoordinates o)
        {
            WeightedCartesianCoordinates result;
            result.coordinates = o;
            result.weight = 1d;
            return result;
        }
    }

    struct WeightedCartesianCoordinates
    {
        public CartesianCoordinates coordinates;
        public double weight;
    }

    struct WeightedCartesianCoordinatesAggregate
    {
        public static WeightedCartesianCoordinatesAggregate Empty = new WeightedCartesianCoordinatesAggregate { midpoint = new CartesianCoordinates(), totalWeight = 1 };
        
        public CartesianCoordinates midpoint;
        public double totalWeight;
    }

    static class GPSMath
    {
        static readonly double RADIANS = Math.PI / 180d;
        static readonly double DECIMAL = 180d / Math.PI;

        public static RadiansCoordinates ToRadians(this DecimalCoordinates o)
        {
            RadiansCoordinates result;
            result.radiansLatitude = RADIANS * o.decimalLatitude;
            result.radiansLongitude = RADIANS * o.decimalLongitude;
            return result;
        }

        public static CartesianCoordinates ToCartesian(this RadiansCoordinates o)
        {
            CartesianCoordinates result;
            result.x = Math.Cos(o.radiansLatitude) * Math.Cos(o.radiansLongitude);
            result.y = Math.Cos(o.radiansLatitude) * Math.Sin(o.radiansLongitude);
            result.z = Math.Sin(o.radiansLatitude);
            return result;
        }

        public static WeightedCartesianCoordinatesAggregate Add(this WeightedCartesianCoordinatesAggregate o, WeightedCartesianCoordinates tuple)
        {
            var totalWeight = o.totalWeight;
            var xAgg = o.midpoint.x * totalWeight;
            var yAgg = o.midpoint.y * totalWeight;
            var zAgg = o.midpoint.z * totalWeight;

            xAgg += tuple.coordinates.x * tuple.weight;
            yAgg += tuple.coordinates.y * tuple.weight;
            zAgg += tuple.coordinates.z * tuple.weight;
            totalWeight += tuple.weight;

            WeightedCartesianCoordinatesAggregate result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.totalWeight = totalWeight;
            return result;
        }

        public static WeightedCartesianCoordinatesAggregate Subtract(this WeightedCartesianCoordinatesAggregate o, WeightedCartesianCoordinates tuple)
        {
            var totalWeight = o.totalWeight;
            var xAgg = o.midpoint.x * totalWeight;
            var yAgg = o.midpoint.y * totalWeight;
            var zAgg = o.midpoint.z * totalWeight;

            xAgg -= tuple.coordinates.x * tuple.weight;
            yAgg -= tuple.coordinates.y * tuple.weight;
            zAgg -= tuple.coordinates.z * tuple.weight;
            totalWeight -= tuple.weight;

            WeightedCartesianCoordinatesAggregate result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.totalWeight = totalWeight;
            return result;
        }

        public static WeightedCartesianCoordinatesAggregate GetWeightedGeographicMidpoint(this WeightedCartesianCoordinates[] coords)
        {
            double totalWeight = 0, xAgg = 0, yAgg = 0, zAgg = 0;

            foreach (var tuple in coords)
            {
                xAgg += tuple.coordinates.x * tuple.weight;
                yAgg += tuple.coordinates.y * tuple.weight;
                zAgg += tuple.coordinates.z * tuple.weight;

                totalWeight += tuple.weight;
            }

            WeightedCartesianCoordinatesAggregate result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.totalWeight = totalWeight;
            return result;
        }

        public static WeightedCartesianCoordinatesAggregate GetGeographicMidpoint(this CartesianCoordinates[] coords)
        {
            double totalWeight = 0, xAgg = 0, yAgg = 0, zAgg = 0;

            foreach (var tuple in coords)
            {
                xAgg += tuple.x;
                yAgg += tuple.y;
                zAgg += tuple.z;

                totalWeight++;
            }

            WeightedCartesianCoordinatesAggregate result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.totalWeight = totalWeight;
            return result;
        }

        public static RadiansCoordinates ToRadians(this CartesianCoordinates o)
        {
            RadiansCoordinates result;
            result.radiansLongitude = Math.Atan2(o.y, o.x);

            var hypotenuse = Math.Sqrt(o.x * o.x + o.y * o.y);

            result.radiansLatitude = Math.Atan2(o.z, hypotenuse);
            return result;
        }

        public static DecimalCoordinates ToDecimal(this RadiansCoordinates o)
        {
            DecimalCoordinates result;
            result.decimalLatitude = DECIMAL * o.radiansLatitude;
            result.decimalLongitude = DECIMAL * o.radiansLongitude;
            return result;
        }

        public static double Distance(CartesianCoordinates a, CartesianCoordinates b)
        {
            var xd = a.x - b.x;
            var yd = a.y - b.y;
            var zd = a.z - b.z;
            return Math.Sqrt(xd * xd + yd * yd + zd * zd);
        }

        /// <summary>
        /// Return 1/10 the squared distance of 2 points in 3D.  Eliminates the expensive square root calculation
        /// </summary>
        public static double DistanceFastCalc(CartesianCoordinates a, CartesianCoordinates b)
        {
            var xd = a.x - b.x;
            var yd = a.y - b.y;
            var zd = a.z - b.z;

            var xd2 = xd * xd;
            var yd2 = yd * yd;
            var zd2 = zd * zd;

            var xten = xd2 * 0.1d;
            var yten = yd2 * 0.1d;
            var zten = zd2 * 0.1d;

            return xten + yten + zten;
        }
    }
}
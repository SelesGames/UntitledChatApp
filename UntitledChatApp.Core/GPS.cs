using System;
using System.Linq;

namespace UntitledChatApp.Core
{
    public struct DecimalCoordinates
    {
        public double decimalLatitude;
        public double decimalLongitude;

        public override string ToString()
        {
            return string.Format("lat: {0}, long: {1}", decimalLatitude, decimalLongitude);
        }

        public static implicit operator DecimalCoordinates(string coords)
        {
            if (string.IsNullOrWhiteSpace(coords))
                ThrowInvalidCastException();

            var split = coords.Split(',').ToArray();
            if (split.Length != 2)
                ThrowInvalidCastException();

            DecimalCoordinates result;
            double lat = 0d, longi = 0d;
            if (!(
                double.TryParse(split[0], out lat) && 
                double.TryParse(split[1], out longi)
            ))
                ThrowInvalidCastException();

            result.decimalLatitude = lat;
            result.decimalLongitude = longi;
            return result;
        }

        static void ThrowInvalidCastException()
        {
            throw new InvalidCastException("these coords are not valid");
        }
    }

    public struct RadiansCoordinates
    {
        public double radiansLatitude;
        public double radiansLongitude;

        public override string ToString()
        {
            return string.Format("rlat: {0}, rlong: {1}", radiansLatitude, radiansLongitude);
        }
    }

    public struct CartesianCoordinates
    {
        public double x;
        public double y;
        public double z;

        public static implicit operator WeightedCartesianCoordinates(CartesianCoordinates o)
        {
            WeightedCartesianCoordinates result;
            result.midpoint = o;
            result.weight = 1d;
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", x, y, z);
        }
    }

    public struct WeightedCartesianCoordinates
    {
        public static WeightedCartesianCoordinates Empty;

        public CartesianCoordinates midpoint;
        public double weight;

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}    weight: {3}", 
                midpoint.x, 
                midpoint.y, 
                midpoint.z, 
                weight);
        }
    }

    //public struct WeightedCartesianCoordinatesAggregate
    //{
    //    public static WeightedCartesianCoordinatesAggregate Empty = new WeightedCartesianCoordinatesAggregate { midpoint = new CartesianCoordinates(), totalWeight = 1 };
        
    //    public CartesianCoordinates midpoint;
    //    public double totalWeight;
    //}

    public static class GPSMath
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

        public static WeightedCartesianCoordinates Add(this WeightedCartesianCoordinates o, WeightedCartesianCoordinates tuple)
        {
            var totalWeight = o.weight;
            var xAgg = o.midpoint.x * totalWeight;
            var yAgg = o.midpoint.y * totalWeight;
            var zAgg = o.midpoint.z * totalWeight;

            xAgg += tuple.midpoint.x * tuple.weight;
            yAgg += tuple.midpoint.y * tuple.weight;
            zAgg += tuple.midpoint.z * tuple.weight;
            totalWeight += tuple.weight;

            WeightedCartesianCoordinates result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.weight = totalWeight;
            return result;
        }

        public static WeightedCartesianCoordinates Subtract(this WeightedCartesianCoordinates o, WeightedCartesianCoordinates tuple)
        {
            var totalWeight = o.weight;
            var xAgg = o.midpoint.x * totalWeight;
            var yAgg = o.midpoint.y * totalWeight;
            var zAgg = o.midpoint.z * totalWeight;

            xAgg -= tuple.midpoint.x * tuple.weight;
            yAgg -= tuple.midpoint.y * tuple.weight;
            zAgg -= tuple.midpoint.z * tuple.weight;
            totalWeight -= tuple.weight;

            WeightedCartesianCoordinates result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.weight = totalWeight;
            return result;
        }

        public static WeightedCartesianCoordinates GetWeightedGeographicMidpoint(this WeightedCartesianCoordinates[] coords)
        {
            double totalWeight = 0, xAgg = 0, yAgg = 0, zAgg = 0;

            foreach (var tuple in coords)
            {
                xAgg += tuple.midpoint.x * tuple.weight;
                yAgg += tuple.midpoint.y * tuple.weight;
                zAgg += tuple.midpoint.z * tuple.weight;

                totalWeight += tuple.weight;
            }

            WeightedCartesianCoordinates result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.weight = totalWeight;
            return result;
        }

        public static WeightedCartesianCoordinates GetGeographicMidpoint(this CartesianCoordinates[] coords)
        {
            double totalWeight = 0, xAgg = 0, yAgg = 0, zAgg = 0;

            foreach (var tuple in coords)
            {
                xAgg += tuple.x;
                yAgg += tuple.y;
                zAgg += tuple.z;

                totalWeight++;
            }

            WeightedCartesianCoordinates result;
            result.midpoint.x = xAgg / totalWeight;
            result.midpoint.y = yAgg / totalWeight;
            result.midpoint.z = zAgg / totalWeight;
            result.weight = totalWeight;
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
        /// Return the squared distance of 2 points in 3D.  Eliminates the expensive square root calculation
        /// </summary>
        public static double DistanceSquared(CartesianCoordinates a, CartesianCoordinates b)
        {
            var xd = a.x - b.x;
            var yd = a.y - b.y;
            var zd = a.z - b.z;

            var xd2 = xd * xd;
            var yd2 = yd * yd;
            var zd2 = zd * zd;

            return xd2 + yd2 + zd2;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledChatApp.Core;
using UntitledChatApp.Core.Clustering;
using UntitledChatApp.Core.Graph;

namespace UntitledChatApp.Sandbox.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RunTest();

            while (true)
                System.Console.Read();
        }


        void RunTest()
        {
            var geocoder = new Geocoding.Google.GoogleGeocoder();

            var tree = RoomTree.Instance;

            var addresses = new[] {
                "2728 Stevens St, La Crescenta CA 91214",
                "1766 Sand Hill Road, Palo Alto CA 94304", 
                "3645 Broadway New York, NY 10031",
                "Paseo del Prado, 26 28014 Madrid Spain",
                "3710 U.S. 9 Freehold, NJ 07728",
                "101 Forest Ave Palo Alto, CA 94301",
                "Heerengracht St Cape Town City Centre, Cape Town 8000, South Africa",
            };

            var users = addresses
                .SelectMany(o => geocoder.Geocode(o)
                    .Select(x => new { address = o, geo = x }))
                .Select(o =>
                    new
                    {
                        coords =
                            new DecimalCoordinates
                            {
                                decimalLatitude = o.geo.Coordinates.Latitude,
                                decimalLongitude = o.geo.Coordinates.Longitude
                            },
                        address = o.address
                    })
                .Select(o => 
                    new UserNode 
                    { 
                        MidPoint = o.coords.ToRadians().ToCartesian(),
                        Address = o.address,
                    })
                .ToList();

            System.Console.WriteLine(users);

            foreach (var user in users)
            {
                tree.AddUser(user);
                System.Console.WriteLine(string.Format(
                    "Num users: {0}, midpoint: {1}", 
                    user.Parent.Children.Count, 
                    user.Parent.MidPoint.midpoint.ToRadians().ToDecimal()));
            }

            System.Console.WriteLine("now users are in:\r\n******************");

            foreach (var user in users)
            {
                System.Console.WriteLine(string.Format(
                    "Num users: {0}, midpoint: {1}",
                    user.Parent.Children.Count,
                    user.Parent.MidPoint.midpoint.ToRadians().ToDecimal()));
            }

            var algorithms = new Algorithms();
            var clusterItems = users.Select(o => new ClusterItem<UserNode> { Mean = o.MidPoint.midpoint, Value = o }).ToList();
            var clusters = algorithms.ComputeKMeans(clusterItems, 2, 5).ToList();
            
            int i = 0;
            foreach (var cluster in clusters)
            {
                System.Console.WriteLine(string.Format("Cluster {0}", i++));
                foreach (var item in cluster.Values)
                {
                    System.Console.WriteLine(string.Format(
                        "Address: {0}",
                        item.Value.ToString()));
                }
                System.Console.WriteLine();
            }
            //var user1 = new UserNode { MidPoint = }
        }
    }
}

using Geocoding.Google;
using Newtonsoft.Json;
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
                "6254 Lexington Ave Los Angeles, CA 90038",
                "2729 Stevens St, La Crescenta CA 91214",
                "2730 Stevens St, La Crescenta CA 91214",
                "2732 Stevens St, La Crescenta CA 91214",
                "2733 Stevens St, La Crescenta CA 91214",
                "2734 Stevens St, La Crescenta CA 91214",
                "2735 Stevens St, La Crescenta CA 91214",
            };

            var coords = new DecimalCoordinates[] {
                "34.228678, -118.238516",
                "37.430674, -122.187788", 
                "40.830045, -73.948349",
                "40.412344, -3.693364",
                "40.252929, -74.300446",
                "37.442422, -122.161644",
                "-33.916492, 18.428951",
                "34.092568, -118.326251",
                "34.229087, -118.238338",
                "34.228922, -118.238469",
                "34.228916, -118.238510",
                "34.228972, -118.238600",
                "34.228768, -118.238705",
                "34.229237, -118.238535",
            };

            var users = addresses.Zip(coords, (address, coord) => 
                new UserNode
                { 
                    Address = address,
                    MidPoint = coord.ToRadians().ToCartesian(),
                })
                //.SelectMany(o => geocoder.Geocode(o)
                //    .Select(x => new { address = o, geo = x }))
                //.Select(o =>
                //    new
                //    {
                //        coords =
                //            new DecimalCoordinates
                //            {
                //                decimalLatitude = o.geo.Coordinates.Latitude,
                //                decimalLongitude = o.geo.Coordinates.Longitude
                //            },
                //        address = o.address
                //    })
                //.Select(o => 
                //    new UserNode 
                //    { 
                //        MidPoint = o.coords.ToRadians().ToCartesian(),
                //        Address = o.address,
                //    })
                .ToList();

            System.Console.WriteLine(users);

            foreach (var user in users)
            {
                tree.AddUser(user);
                //System.Console.WriteLine(string.Format(
                //    "Num users: {0}, midpoint: {1}", 
                //    user.Parent.Children.Count, 
                //    user.Parent.MidPoint.midpoint.ToRadians().ToDecimal()));
            }

            //Output(tree);

            //var lastUser = users.Last();
            //tree.AddUser(lastUser);

            System.Console.WriteLine("\r\n\r\nnow users are in:\r\n******************");

            var rooms = users.Select(o => o.Parent).OfType<RoomNode>().Distinct();
            foreach (var room in rooms)
            {
                System.Console.WriteLine(string.Format(
                    "Room ID: {0} \r\nMID: {1}",
                    room.Id,
                    room.MidPoint.midpoint.ToRadians().ToDecimal()));

                foreach (var user in room.Children.OfType<UserNode>())
                {
                    System.Console.WriteLine(string.Format(
                        "    {0}",
                        user.Address));
                }

                System.Console.WriteLine();
            }

            var treeOutput = Newtonsoft.Json.JsonConvert.SerializeObject(tree.root,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                });
            //System.Console.WriteLine(string.Format("tree:\r\n{0}", treeOutput));

            //var algorithms = new Algorithms();
            //var clusterItems = users.Select(o => new ClusterItem<UserNode> { Mean = o.MidPoint.midpoint, Value = o }).ToList();
            //var clusters = algorithms.ComputeKMeans(clusterItems, 2, 5).ToList();
            
            //int i = 0;
            //foreach (var cluster in clusters)
            //{
            //    System.Console.WriteLine(string.Format("Cluster {0}", i++));
            //    foreach (var item in cluster.Values)
            //    {
            //        System.Console.WriteLine(string.Format(
            //            "Address: {0}",
            //            item.Value.ToString()));
            //    }
            //    System.Console.WriteLine();
            //}
        }

        //static void Output(RoomTree tree)
        //{
        //    System.Console.WriteLine("root: " + tree.ToString());
        //    Output(tree.root);
        //}

    }
}

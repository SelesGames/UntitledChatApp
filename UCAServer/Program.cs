using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;

namespace UCA.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            //string url = "http://localhost:8080";
            string url = "http://*";//:8080";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }

        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.UseCors(CorsOptions.AllowAll);
                app.MapSignalR();


                /*app.Map("/signalr", map =>
                {
                    // Setup the cors middleware to run before SignalR.
                    // By default this will allow all origins. You can 
                    // configure the set of origins and/or http verbs by
                    // providing a cors options with a different policy.
                    map.UseCors(CorsOptions.AllowAll);

                    var hubConfiguration = new HubConfiguration
                    {
                        // You can enable JSONP by uncommenting line below.
                        // JSONP requests are insecure but some older browsers (and some
                        // versions of IE) require JSONP to work cross domain
                        // EnableJSONP = true
                    };

                    // Run the SignalR pipeline. We're not using MapSignalR
                    // since this branch is already runs under the "/signalr"
                    // path.
                    map.RunSignalR(hubConfiguration);
                });*/
            }
        }
    }
}
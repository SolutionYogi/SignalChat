using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Autofac;
using Autofac.Integration.SignalR;
using ChatServerCS;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace ChatServerHosted
{
    public class TownCrier
    {
        private readonly Timer _timer;

        public TownCrier()
        {
            _timer = new Timer(1000) {AutoReset = true};
            _timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
        }

        public void Start()
        {
            _timer.Start();
            

            Task.Factory.StartNew(StartHub);

        }

        private void StartHub()
        {
            var url = "http://localhost:8080/";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine($"Server running at {url}");
                Console.ReadLine();
            }
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            var builder = new ContainerBuilder();

            // STANDARD SIGNALR SETUP:

            // Get your HubConfiguration. In OWIN, you'll create one
            // rather than using GlobalHost.
            var config = new HubConfiguration();

            // Register your SignalR hubs.
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterType<ChatHub>().ExternallyOwned();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.Resolver = new AutofacDependencyResolver(container);

            // OWIN SIGNALR SETUP:

            var hubConfig = new HubConfiguration();
            app.MapSignalR("/signalchat", hubConfig);

            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
        }
    }
}
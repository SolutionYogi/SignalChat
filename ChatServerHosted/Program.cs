using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.SignalR;
using ChatServerCS;
using Microsoft.AspNet.SignalR;
using Topshelf;

namespace ChatServerHosted
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>                                   //1
            {
                x.Service<TownCrier>(s =>                                   //2
                {
                    s.ConstructUsing(name=> new TownCrier());                //3
                    s.WhenStarted(tc => tc.Start());                         //4
                    s.WhenStopped(tc => tc.Stop());                          //5
                });
                x.RunAsLocalSystem();                                       //6

                x.SetDescription("SignalR.ChatServer");                   //7
                x.SetDisplayName("SignalR.ChatServer");                                  //8
                x.SetServiceName("SignalR.ChatServer");                                  //9
            });                                                             //10

            var exitCode = (int) Convert.ChangeType(rc, rc.GetTypeCode());  //11
            Environment.ExitCode = exitCode;
        }
    }

    public static class App
    {
        private static IContainer _container;

        public static void Initialize()
        {
            var builder = new ContainerBuilder();

            // Register your SignalR hubs.
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            builder.RegisterType<ChatHub>().ExternallyOwned();

            // Set the dependency resolver to be Autofac.
            _container = builder.Build();
            GlobalHost.DependencyResolver = new AutofacDependencyResolver(_container);
        }
    }
}

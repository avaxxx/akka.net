﻿using Akka.Actor;
using Akka.DI.Ninject;
using Akka.Routing;
using System;
using System.Threading.Tasks;

namespace BasicNinjectUses
{
    class Program
    {
        static void Main(string[] args)
        {
            WithHashPool();
        }

        private static void WithHashPool()
        {
            Ninject.IKernel container = new Ninject.StandardKernel();
            container.Bind<TypedWorker>().To(typeof(TypedWorker));


            using (var system = ActorSystem.Create("MySystem"))
            {
                var propsResolver =
                    new NinjectDependencyResolver(container, system);

                var router = system.ActorOf(propsResolver.Create<TypedWorker>().WithRouter(FromConfig.Instance), "router1");

                Task.Delay(500).Wait();
                Console.WriteLine("Sending Messages");

                for (var i = 0; i < 5; i++)
                {
                    for (var j = 0; j < 7; j++)
                    {

                        var msg = new TypedActorMessage { Id = j, Name = Guid.NewGuid().ToString() };
                        var ms = new AnotherMessage { Id = j, Name = msg.Name };

                        var envelope = new ConsistentHashableEnvelope(ms, msg.Id);

                        router.Tell(msg);
                        router.Tell(envelope);

                    }
                }
            }


            Console.ReadLine();
        }
    }
}

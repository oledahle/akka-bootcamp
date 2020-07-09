using System;
using Akka.Actor;

namespace HelloWorld
{
    class Program
    {
        public static ActorSystem MyActorSystem;
        static void Main(string[] args)
        {
	    Console.WriteLine("Hello World starting up");
	    
	    MyActorSystem = ActorSystem.Create("MyActorSystem");
	    var greeter = MyActorSystem.ActorOf<GreetingActor>("greeter");
            greeter.Tell(new Greet("Akka World!"));
	    
            // prevent the application from exiting before message is handled
            Console.ReadLine();
        }
    }
}

using System;
using Akka.Actor;

public class GreetingActor : ReceiveActor
{
   public GreetingActor()
   {
      Receive<Greet>(greet => Console.WriteLine("Greeting {0}", greet.Who));
   }
}
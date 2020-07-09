using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for serializing message writes to the console.
    /// (write one message at a time, champ :)
    /// </summary>
    class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is Messages.InputError)
            {
                Console.ForegroundColor = ConsoleColor.Red ;
                Console.WriteLine((message as Messages.InputError).Reason);  
            }
            else if (message is Messages.InputSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green ;
                Console.WriteLine((message as Messages.InputSuccess).Reason);  
            }
            else if (message is string)
            {
                Console.Write(message as string);
            }
            Console.ResetColor();
        }
    }
}

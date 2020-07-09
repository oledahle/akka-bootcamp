using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";
        private readonly IActorRef _validationActor;

        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                PrintInstructions();
            }

            GetAndValidateInput();
        }
        
        private static void PrintInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Please provide a URI of a log file on disk.\n");
        }

        private void GetAndValidateInput()
        {
            var read = Console.ReadLine();
            if (String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the system (acquire handle to system via
                // this actors context)
                Context.System.Terminate();
            }
            else
            {
                // Hand off to FileValidationActor
                _validationActor.Tell(read);
            }
        }

    }
}
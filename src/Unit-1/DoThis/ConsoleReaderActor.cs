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
        private IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                PrintInstructions();
            }
            else if (message is Messages.InputError || message is Messages.InputSuccess)
            {
                // send input to the console writer
                _consoleWriterActor.Tell(message);
            }

            GetAndValidateInput();
        }
        
        private static void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }

        private void GetAndValidateInput()
        {
            var read = Console.ReadLine();
            if (String.IsNullOrEmpty(read))
            {
               // Signal that input was blank
               Self.Tell(new Messages.NullInputError("No input received"));
            }
            else if (String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the system (acquire handle to system via
                // this actors context)
                Context.System.Terminate();
            }
            else
            {
                var valid = IsValid(read);
                if (valid)
                {
                    Self.Tell(new Messages.InputSuccess("Thanks, message was valid!"));
                }
                else
                {
                    Self.Tell(new Messages.ValidationError("Sorry, message had odd number of characters."));
                }
            }
 
        }

        private static bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}
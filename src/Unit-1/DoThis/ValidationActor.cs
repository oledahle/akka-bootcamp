using Akka.Actor;

namespace WinTail
{
    /**
     * Actor that validates user input and signals result to others
     */
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                // Signal that input was blank
                _consoleWriterActor.Tell(new Messages.NullInputError("No input received"));
            }
            else
            {
                var valid = IsValid(msg);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thanks, message was valid!"));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError("Sorry, message had odd number of characters."));
                }
            }
            // Tell sender to continue doing its thing
            Sender.Tell(new Messages.ContinueProcessing());
        }

        private static bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}
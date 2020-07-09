using Akka.Actor;

namespace WinTail
{
    /**
     * Actor that validates user input and signals result to others
     */
    public class FileValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        private readonly IActorRef _tailCoordinatorActor;

        public FileValidationActor(IActorRef consoleWriterActor, IActorRef tailCoordinatorActor)
        {
            _consoleWriterActor = consoleWriterActor;
            _tailCoordinatorActor = tailCoordinatorActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                // Signal that input was blank
                _consoleWriterActor.Tell(new Messages.NullInputError("No input received"));
                // Tell sender to continue doing its thing
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                var valid = IsFileUri(msg);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess(string.Format("Start processing for {0}", msg)));
                    // Start coordinator
                    _tailCoordinatorActor.Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError(string.Format("{0} is not a file on disk", msg)));
                    // Tell sender to continue doing its thing
                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }
  
        }

        private static bool IsFileUri(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}
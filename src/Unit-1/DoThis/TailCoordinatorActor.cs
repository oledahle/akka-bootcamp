using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor  : UntypedActor
    {
        #region messages
        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }
            
            public string FilePath { get; private set; }
            public IActorRef ReporterActor { get; private set; }
        }
        
        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }
            
            public string FilePath { get; private set; }
        }
        
        #endregion

        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;
                Context.ActorOf(Props.Create(
                    () => new TailActor(msg.ReporterActor, msg.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // retries
                    TimeSpan.FromSeconds(10), // Within time range
                x =>
                {
                    // Resume immediately on math errors
                    if (x is ArithmeticException) return Directive.Resume;
                    // Stop everything on really serious failures
                    else if (x is NotSupportedException) return Directive.Stop;
                    // In more normal cases, restart the failed actor
                    else return Directive.Restart;
                });
        }
    }
}
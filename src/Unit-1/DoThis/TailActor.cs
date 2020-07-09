using System;
using Akka.Actor;
using System.IO;
using System.Text;

namespace WinTail
{
    // Uses a FileObserver to monitor a file and sends updates to the console
    public class TailActor : UntypedActor
    {
       #region messages

       public class FileWrite
       {
           public FileWrite(string filename)
           {
               FileName = filename;
           }
           public string FileName { get; private set; }
       }

       public class FileError
       {
           public FileError(string filename, string reason)
           {
               FileName = filename;
               Reason = reason;
           }
           public string FileName { get; private set; }
           public string Reason { get; private set; }
       }
       
       public class InitialRead
       {
           public InitialRead(string filename, string text)
           {
               FileName = filename;
               Text = text;
           }
           public string FileName { get; private set; }
           public string Text { get; private set; }
       }
       #endregion

       private readonly string _filePath;
       private readonly IActorRef _reporterActor;
       private readonly FileObserver _observer;
       private readonly Stream _fileStream;
       private readonly StreamReader _fileStreamReader;

       public TailActor(IActorRef reporterActor, string filePath)
       {
           _reporterActor = reporterActor;
           _filePath = filePath;
           
           // Starting to watch immediately
           _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
           _observer.Start();
           
           // Open the file stream
           _fileStream = new FileStream(Path.GetFullPath(_filePath),
               FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
           _fileStreamReader = new StreamReader(_fileStream, Encoding.UTF8);
           
           Console.WriteLine("TailActor doing initial read\n");
           // Do initial read
           var text = _fileStreamReader.ReadToEnd();
           Self.Tell(new InitialRead(_filePath, text));
       }

       protected override void OnReceive(object message)
       {
           if (message is FileWrite)
           {
               // Read any appended(!) content
               var text = _fileStreamReader.ReadToEnd();
               if (!string.IsNullOrEmpty(text))
               {
                   _reporterActor.Tell(text);
               }
           }
           else if (message is FileError)
           {
               var fe = message as FileError;
               _reporterActor.Tell(string.Format("Tail error: {0}", fe.Reason));
           }
           else if (message is InitialRead)
           {
               var ir = message as InitialRead;
               _reporterActor.Tell(ir.Text);
           }
       }
    }
}
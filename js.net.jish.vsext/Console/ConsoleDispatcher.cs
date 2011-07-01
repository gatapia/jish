using System;
using System.Threading.Tasks;
using js.net.jish.vsext.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Task = System.Threading.Tasks.Task;

namespace js.net.jish.vsext.Console
{
  internal interface IPrivateConsoleDispatcher : IConsoleDispatcher
  {
    void PostInputLine(InputLine inputLine);
  }

  internal class ConsoleDispatcher : IPrivateConsoleDispatcher
  {
    private IPrivateWpfConsole WpfConsole { get; set; }

    private Dispatcher dispatcher;

    public event EventHandler StartCompleted;

    public ConsoleDispatcher(IPrivateWpfConsole wpfConsole)
    {
      if (wpfConsole == null)
      {
        throw new ArgumentNullException("wpfConsole");
      }

      WpfConsole = wpfConsole;
    }

    public bool IsExecutingCommand { get { return (dispatcher != null) && dispatcher.IsExecuting; } }

    private void RaiseEventSafe(EventHandler handler)
    {
      if (handler != null)
      {
        ThreadHelper.Generic.Invoke(() => handler(this, EventArgs.Empty));
      }
    }

    public bool IsStartCompleted { get; private set; }

    public void Start()
    {
      // Only Start once
      if (dispatcher == null)
      {
        IHost host = WpfConsole.Host;

        if (host == null)
        {
          throw new InvalidOperationException("Can't start Console dispatcher. Host is null.");
        }

        dispatcher = new SyncHostConsoleDispatcher(this);

        Task.Factory.StartNew( // gives the host a chance to do initialization works before the console starts accepting user inputs
          () => host.Initialize(WpfConsole)).ContinueWith(task =>
          {
            if (task.IsFaulted)
            {
              var exception = ExceptionUtility.Unwrap(task.Exception);
              WriteError(exception.Message);
            }

            if (host.IsCommandEnabled)
            {
              ThreadHelper.Generic.Invoke(dispatcher.Start);
            }

            RaiseEventSafe(StartCompleted);
            IsStartCompleted = true;
          }, TaskContinuationOptions.NotOnCanceled);
      }
    }

    private void WriteError(string message)
    {
      if (WpfConsole != null)
      {
        WpfConsole.Write(message + Environment.NewLine);
      }
    }

    public void ClearConsole()
    {
      if (dispatcher != null)
      {
        dispatcher.ClearConsole();
      }
    }

    public void PostInputLine(InputLine inputLine)
    {
      if (dispatcher != null)
      {
        dispatcher.PostInputLine(inputLine);
      }
    }

    private abstract class Dispatcher
    {
      private IPrivateWpfConsole WpfConsole { get; set; }

      private bool isExecuting;

      public bool IsExecuting
      {
        get { return isExecuting; }
        protected set
        {
          isExecuting = value;
          WpfConsole.SetExecutionMode(isExecuting);
        }
      }

      protected Dispatcher(ConsoleDispatcher parentDispatcher)
      {
        WpfConsole = parentDispatcher.WpfConsole;
      }

      protected Tuple<bool, bool> Process(InputLine inputLine)
      {
        if (inputLine.Flags.HasFlag(InputLineFlag.Echo))
        {
          WpfConsole.BeginInputLine();

          if (inputLine.Flags.HasFlag(InputLineFlag.Execute))
          {
            WpfConsole.WriteLine(inputLine.Text);
          } else
          {
            WpfConsole.Write(inputLine.Text);
          }
        }

        if (inputLine.Flags.HasFlag(InputLineFlag.Execute))
        {
          string command = inputLine.Text;
          bool isExecuted = WpfConsole.Host.Execute(WpfConsole, command);
          WpfConsole.InputHistory.Add(command);
          return Tuple.Create(true, isExecuted);
        }
        return Tuple.Create(false, false);
      }

      protected void PromptNewLine()
      {
        WpfConsole.Write(WpfConsole.Host.Prompt + (char) 32); // 32 is the space
        WpfConsole.BeginInputLine();
      }

      public void ClearConsole()
      {
        // When inputting commands
        if (WpfConsole.InputLineStart != null)
        {
          WpfConsole.Host.Abort(); // Clear constructing multi-line command
          WpfConsole.Clear();
          PromptNewLine();
        } else
        {
          WpfConsole.Clear();
        }
      }

      public abstract void Start();
      public abstract void PostInputLine(InputLine inputLine);
    }

    private class SyncHostConsoleDispatcher : Dispatcher
    {
      public SyncHostConsoleDispatcher(ConsoleDispatcher parentDispatcher) : base(parentDispatcher) { }

      public override void Start() { PromptNewLine(); }

      public override void PostInputLine(InputLine inputLine)
      {
        IsExecuting = true;
        try
        {
          if (Process(inputLine).Item1)
          {
            PromptNewLine();
          }
        } finally
        {
          IsExecuting = false;
        }
      }
    }
  }

  [Flags] internal enum InputLineFlag
  {
    Echo = 1,
    Execute = 2
  }

  internal class InputLine
  {
    public string Text { get; private set; }
    public InputLineFlag Flags { get; private set; }

    public InputLine(string text, bool execute)
    {
      Text = text;
      Flags = InputLineFlag.Echo;

      if (execute)
      {
        Flags |= InputLineFlag.Execute;
      }
    }

    public InputLine(SnapshotSpan snapshotSpan)
    {
      Text = snapshotSpan.GetText();
      Flags = InputLineFlag.Execute;
    }
  }
}
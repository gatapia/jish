using System;
using System.Diagnostics;
using js.net.Engine;
using js.net.Util;

namespace js.net.repl
{
  public class CommandLineInterpreter
  {    
    private readonly IEngine engine;
    private readonly JSConsole console;

    public CommandLineInterpreter(IEngine engine, JSConsole console)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.console = console;

      InitialiseConsole();

      Console.WriteLine("Type '.help' for options.");
    }

    private void InitialiseConsole()
    {
      Console.TreatControlCAsInput = false;
      Console.CancelKeyPress += (s, e) => Environment.Exit(0);
    }

    public bool ReadAndExecuteCommand()
    {
      Console.Write("> ");
      string input = Console.ReadLine().Trim();
      if (String.IsNullOrWhiteSpace(input) || InterceptSpecialCommands(input)) { return true;  }
      
      try
      {
        object val = engine.Run(input);
        if (val != null && val != String.Empty) console.log(val);
        if (val != null) engine.SetGlobal("_", val);
      }
      catch (Exception e)
      {        
        PrintExceptionMessage(e);
      }
      return true;
    }

    private void PrintExceptionMessage(Exception e)
    {
      string msg = e.Message;
      if (msg.IndexOf(": ") > 0) msg = msg.Substring(msg.IndexOf(": ") + 2);
      if (msg.IndexOf('(') > 0) msg = msg.Substring(0, msg.IndexOf('('));
      
      Console.WriteLine(msg);
    }

    private bool InterceptSpecialCommands(string input)
    {
      if (input.Equals(".exit"))
      {
        Environment.Exit(0);        
      } else if (input.Equals(".break"))
      {
        Console.WriteLine("Not implemented...");
      } else if (input.Equals(".help"))
      {
        Console.WriteLine(new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents("js.net.repl.resources.help.txt", GetType().Assembly));
      } else if (input.Equals(".clear"))
      {
        Console.WriteLine("Clearing context...");
        engine.Run(
@"
for (var i in this) {
  if (i === 'console' || i === 'global') continue;
  delete this[i];
};
"
);        
      } else
      {
        return false;
      }
      return true;
    }
  }
}
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

      Console.WriteLine("Type '.help' for options.");
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
        Console.WriteLine(e.Message);
      }
      return true;
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
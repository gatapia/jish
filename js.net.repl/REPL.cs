using System;
using System.IO;

namespace js.net.repl
{
  public class REPL
  {
    private readonly ICommandLineInterpreter interpretter;

    public REPL(ICommandLineInterpreter interpretter)
    {
      this.interpretter = interpretter;
    }

    public void StartREPL()
    {   
      while (true)
      {
        string command = interpretter.ReadCommand();
        interpretter.ExecuteCommand(command);
      }
    }

    public void ExecuteArgs(string[] args)
    {
      string file = args[0];
      if (!File.Exists(file))
      {
        Console.WriteLine("Could not find " + file);
        return;
      }
      interpretter.RunFile(file);      
    }    
  }
}
using System;
using System.IO;

namespace js.net.jish
{
  public class Jish
  {
    private readonly ICommandLineInterpreter interpretter;

    public Jish(ICommandLineInterpreter interpretter)
    {
      this.interpretter = interpretter;
    }

    public void StartJish()
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
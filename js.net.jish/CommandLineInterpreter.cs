using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using js.net.Engine;
using js.net.jish.Command;
using Noesis.Javascript;

namespace js.net.jish
{
  public class CommandLineInterpreter : ICommandLineInterpreter
  {
    private readonly IList<ICommand> commands = new List<ICommand>();
    protected IDictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

    private readonly JSConsole console;
    private readonly IEngine engine;
    private string bufferedCommand = String.Empty;

    public CommandLineInterpreter(IEngine engine, JSConsole console)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.console = console;
      
      Initialise();

      console.log("Type '.help' for options.");
    }

    private void Initialise()
    {
      foreach (Type t in GetType().Assembly.GetTypes().Where(t => t != typeof (ICommand) && typeof (ICommand).IsAssignableFrom(t)))
      {
        commands.Add((ICommand) Activator.CreateInstance(t, this, console));
      }
    }

    public virtual string ReadCommand()
    {
      console.log("> ", false);
      string input = Console.ReadLine().Trim();
      if (String.IsNullOrWhiteSpace(input))
      {
        return null;
      }
      return input;      
    }
    
    public void ExecuteCommand(string input)
    {  
      if (input.IndexOf('.') == 0)
      {
        InterceptSpecialCommands(input);
        return;
      }

      try
      {
        bufferedCommand += input + '\n';
        object returnValue;
        if (!AttemptToRunCommand(out returnValue)) { return; } // Is multi-line
        if (returnValue != null && (!(returnValue is string) || (string) returnValue != String.Empty))
        {
          console.log(returnValue);
        }
        if (returnValue != null) engine.SetGlobal("_", returnValue);
      }
      catch (Exception e)
      {
        bufferedCommand = String.Empty;
        PrintExceptionMessage(e);
      }
    }

    private bool AttemptToRunCommand(out object returnValue)
    {
      returnValue = null;
      try
      {
        returnValue = engine.Run("(" + bufferedCommand + ")", "jish");
        bufferedCommand = String.Empty;
      }  catch (JavascriptException)
      {
        try
        {
          returnValue = engine.Run(bufferedCommand, "jish");
          bufferedCommand = String.Empty;
        } catch (JavascriptException ex2)
        {
          string msg = ex2.Message;
          if (msg.IndexOf("Unexpected token ILLEGAL") >= 0 || msg.IndexOf("SyntaxError") < 0)
          {
            throw;
          }
          return false;
        }
      }
      return true;
    }

    public void RunFile(string file)
    {
      engine.Run(File.ReadAllText(file), new FileInfo(file).Name);
    }

    public void ClearBufferedCommand()
    {
      bufferedCommand = String.Empty;      
    }

    public IEnumerable<ICommand> GetCommands()
    {
      return commands;
    }

    public IDictionary<string, Assembly> GetLoadedAssemblies()
    {
      return loadedAssemblies;
    } 

    public void SetGlobal(string name, object valud)
    {
      engine.SetGlobal(name, valud);
    }

    public void InitialiseConsole()
    {
      Console.TreatControlCAsInput = false;
      Console.CancelKeyPress += (s, e) => Environment.Exit(0);
    }    

    private void PrintExceptionMessage(Exception e)
    {
      string msg = e.Message;
      if (msg.IndexOf(": ") > 0) msg = msg.Substring(msg.IndexOf(": ") + 2);
      if (msg.IndexOf('(') > 0) msg = msg.Substring(0, msg.IndexOf('('));

      console.log(msg);
    }

    private void InterceptSpecialCommands(string input)
    {

      string commandName = new Regex(@"\.([A-z0-9])+").Match(input).Captures[0].Value.Substring(1).Trim();
      foreach (ICommand command in commands)
      {
        if (!command.GetName().Equals(commandName)) continue;
        command.Execute(input);
        return;
      }
      console.log("Could not find command: " + input);                  
    }      
  }
}
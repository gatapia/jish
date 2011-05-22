using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using js.net.Engine;
using js.net.Util;
using Noesis.Javascript;

namespace js.net.jish
{
  public class CommandLineInterpreter : ICommandLineInterpreter
  {
    
    private readonly IDictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

    private readonly JSConsole console;
    private readonly IEngine engine;

    public CommandLineInterpreter(IEngine engine, JSConsole console)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.console = console;
      
      console.log("Type '.help' for options.");
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

    private string bufferedCommand = String.Empty;
    public void ExecuteCommand(string input)
    {  
      if (InterceptSpecialCommands(input)) { return; }
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
        returnValue = engine.Run("(" + bufferedCommand + ")");
        bufferedCommand = String.Empty;
      }  catch (JavascriptException)
      {
        try
        {
          returnValue = engine.Run(bufferedCommand);
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
      engine.Run(File.ReadAllText(file));
    }

    public void InitialiseConsole()
    {
      Console.TreatControlCAsInput = false;
      Console.CancelKeyPress += (s, e) => Exit();
    }    

    private void PrintExceptionMessage(Exception e)
    {
      string msg = e.Message;
      if (msg.IndexOf(": ") > 0) msg = msg.Substring(msg.IndexOf(": ") + 2);
      if (msg.IndexOf('(') > 0) msg = msg.Substring(0, msg.IndexOf('('));

      console.log(msg);
    }

    private bool InterceptSpecialCommands(string input)
    {
      if (input.Equals(".exit"))
      {
        Exit();
      }
      else if (input.Equals(".break"))
      {
        Break();
      }
      else if (input.Equals(".help"))
      {
        Help();
      }
      else if (input.Equals(".clear"))
      {
        ClearGlobalContext();
      }
      else if (input.StartsWith(".using"))
      {
        ImportClassIntoGlobalNamespace(input.Substring(input.IndexOf('(')));
      }
      else if (input.StartsWith(".load"))
      {
        LoadAssembly(input.Substring(input.IndexOf('(')));
      }
      else
      {
        return false;
      }
      return true;
    }

    private void Exit()
    {
      Environment.Exit(0);
    }

    private void Break()
    {
      bufferedCommand = String.Empty;
    }

    private void Help()
    {
      console.log(new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents(
        "js.net.jish.resources.help.txt", GetType().Assembly));
    }

    private void ClearGlobalContext()
    {
      console.log("Clearing context...");
      engine.Run(
        @"
for (var i in this) {
  if (i === 'console' || i === 'global') continue;
  delete this[i];
};
"
        );
    }

    private void LoadAssembly(string input)
    {
      string assemblyFileName = ParseFileOrTypeName(input);
      Assembly assembly = Assembly.LoadFrom(assemblyFileName);
      loadedAssemblies[assembly.GetName().Name] = assembly;
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
    }    

    private void ImportClassIntoGlobalNamespace(string input)
    {
      string nameSpaceAndClass = ParseFileOrTypeName(input);
      new TypeImporter(loadedAssemblies, engine, nameSpaceAndClass, console).ImportType();
    }
    
    private string ParseFileOrTypeName(string input)
    {
      return new Regex(@"([A-z0-9\., ])+").Match(input).Captures[0].Value.Trim();
    }
  }
}
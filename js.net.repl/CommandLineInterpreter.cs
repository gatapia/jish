using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using js.net.Engine;
using js.net.Util;

namespace js.net.repl
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

    public void ExecuteCommand(string input)
    {  
      if (InterceptSpecialCommands(input)) { return; }
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
      console.log("Not implemented...");
    }

    private void Help()
    {
      console.log(new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents(
        "js.net.repl.resources.help.txt", GetType().Assembly));
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
      new REPLTypeImporter(loadedAssemblies, engine, nameSpaceAndClass, console).ImportType();
    }
    
    private string ParseFileOrTypeName(string input)
    {
      return new Regex(@"([A-z0-9\., ])+").Match(input).Captures[0].Value.Trim();
    }
  }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using js.net.jish.Util;
using js.net.Util;

namespace js.net.jish.Command.InlineCommand
{
  public class AssemblyCommand : IInlineCommand
  {
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly JSConsole console;

    public AssemblyCommand(LoadedAssembliesBucket loadedAssemblies, JSConsole console)
    {
      Trace.Assert(loadedAssemblies != null);
      Trace.Assert(console != null);

      this.loadedAssemblies = loadedAssemblies;
      this.console = console;
    }
    
    public string GetNameSpace()
    {
      return "jish";
    }

    /// <summary>
    /// Note: This is not supposed to be called manually, it should be called 
    /// through the jish.assembly command which is defined in jish.js.  This
    /// is also why the help in this file is misleading (says name = assembly).
    //  This is intentional as jish.js assembly delegates to this.
    /// </summary>
    /// <param name="assemblyFileNameOrAssemblyName"></param>
    /// <returns>Returns a dictionary of all commands added (by namespace.commandName).</returns>
    public IDictionary<string, IInlineCommand> loadAssemblyImpl(string assemblyFileNameOrAssemblyName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(assemblyFileNameOrAssemblyName));      

      Assembly assembly = File.Exists(assemblyFileNameOrAssemblyName) 
        ? Assembly.LoadFrom(assemblyFileNameOrAssemblyName)
        : Assembly.Load(assemblyFileNameOrAssemblyName);

      if (loadedAssemblies.ContainsAssembly(assembly.GetName().Name))
      {
        console.log("Assembly '" + assembly.GetName().Name + "' is already loaded. Ignoring.");
        return null;
      }

      IEnumerable<IInlineCommand> loadedCommands = loadedAssemblies.AddAssembly(assembly);
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
      return ConvertCommandsToFullyQualifiedDictionary(loadedCommands);
    }

    private IDictionary<string, IInlineCommand> ConvertCommandsToFullyQualifiedDictionary(IEnumerable<IInlineCommand> loadedCommands)
    {
      Trace.Assert(loadedCommands != null);

      IDictionary<string, IInlineCommand> fullyQualified = new Dictionary<string, IInlineCommand>();
      foreach (IInlineCommand command in loadedCommands)
      {
        string ns = command.GetNameSpace();
        IEnumerable<string> methods = command.GetType().GetMethods().Select(mi => mi.Name).Where(n => Char.IsLower(n[0]));
        foreach (var method in methods)
        {
          fullyQualified.Add(ns + '.' + method, command);
        }
      }
      return fullyQualified;
    }

    public string GetName()
    {
      return "assembly";
    }

    public string GetHelpDescription()
    {
      return "Loads an assembly into the Jish 'context'. You can now\n\t\tjish.create(<typeNames>) types from this assembly.";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      return new[] { new CommandParam {Name = "assemblyFileNameOrAssemblyName"} };
    }
  }
}

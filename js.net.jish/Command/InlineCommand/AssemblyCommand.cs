using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using js.net.jish.Util;
using js.net.Util;

namespace js.net.jish.Command.InlineCommand
{
  public class AssemblyCommand : IInlineCommand
  {
    private readonly AssemblyCommandLoader loader;
    private readonly JSConsole console;

    public AssemblyCommand(AssemblyCommandLoader loader, JSConsole console)
    {
      Trace.Assert(loader != null);
      Trace.Assert(console != null);

      this.loader = loader;
      this.console = console;
    }

    /// <summary>
    /// Note: This is not supposed to be called manually, it should be called 
    /// through the jish.assembly command which is defined in jish.js.  This
    /// is also why the help in this file is misleading (says name = assembly).
    //  This is intentional as jish.js assembly delegates to this.
    /// </summary>
    /// <param name="assemblyFileNameOrAssemblyName"></param>
    /// <returns>Returns a dictionary of all commands added (by namespace.commandName).</returns>
    public IDictionary<string, object> loadAssemblyImpl(string assemblyFileNameOrAssemblyName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(assemblyFileNameOrAssemblyName));      

      Assembly assembly = File.Exists(assemblyFileNameOrAssemblyName) 
        ? Assembly.LoadFrom(assemblyFileNameOrAssemblyName)
        : Assembly.Load(assemblyFileNameOrAssemblyName);
      
      IDictionary<string, object> namespaceCommands = loader.GetCommandsMapFromAssembly(assembly);

      if (namespaceCommands == null)
      {
        console.log("Assembly '" + assembly.GetName().Name + "' is already loaded. Ignoring.");
        return null;
      }      
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
      return namespaceCommands;
    }    
    
    public string GetNameSpace()
    {
      return "jish";
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

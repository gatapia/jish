using System.Collections.Generic;
using System.Reflection;
using js.net.jish.Util;

namespace js.net.jish.InlineCommand
{
  public class AssemblyCommand : IInlineCommand
  {
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly JSConsole console;

    public AssemblyCommand(LoadedAssembliesBucket loadedAssemblies, JSConsole console)
    {
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
    //  This is intentioanl as jish.js assembly delegates to this.
    /// </summary>
    /// <param name="assemblyFileName"></param>
    /// <returns>Returns a dictionary of all commands added (by namespace.commandName).</returns>
    public IDictionary<string, object> loadAssemblyImpl(string assemblyFileName)
    {
      Assembly assembly = Assembly.LoadFrom(assemblyFileName);
      IDictionary<string, object> loadedCommands = loadedAssemblies.AddAssembly(assembly, true);
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
      return loadedCommands;
    }

    public string GetName()
    {
      return "assembly";
    }

    public string GetHelpDescription()
    {
      return "Loads an assembly into the Jish 'context'. You can not\n\t\tjish.create(<typeNames>) types from this assembly.";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      return new[] { new CommandParam {Name = "assemblyFileName"} };
    }
  }
}

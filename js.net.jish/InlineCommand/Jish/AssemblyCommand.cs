using System.Collections.Generic;
using System.Reflection;
using js.net.jish.Util;

namespace js.net.jish.InlineCommand.Jish
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
    /// Returns a dictionary of all commands added (by namespace).
    /// </summary>
    /// <param name="assemblyFileName"></param>
    /// <returns></returns>
    public IDictionary<string, object> assembly(string assemblyFileName)
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

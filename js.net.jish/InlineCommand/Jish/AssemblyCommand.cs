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

    public void assembly(string assemblyFileName)
    {
      Assembly assembly = Assembly.LoadFrom(assemblyFileName);
      loadedAssemblies.AddAssembly(assembly);
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
    }

    public string GetName()
    {
      return "assembly";
    }

    public string GetHelpDescription()
    {
      return "Loads an assembly into the Jish 'context'. You can not jish.create(<typeNames>) types from this assembly.";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      return new[] { new CommandParam {Name = "assemblyFileName"} };
    }
  }
}

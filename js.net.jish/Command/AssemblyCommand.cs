using System.Reflection;
using js.net.jish.Util;

namespace js.net.jish.Command
{
  public class AssemblyCommand : EmptyCommand
  {
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly JSConsole console;

    public AssemblyCommand(LoadedAssembliesBucket loadedAssemblies, JSConsole console)
    {
      this.loadedAssemblies = loadedAssemblies;
      this.console = console;
    }

    public override string GetName()
    {
      return "assembly";
    }

    public override string GetHelpDescription()
    {
      return "Loads a .Net assembly in preparation for .create calls.";
    }

    public override void Execute(params string[] args)
    {
      string assemblyFileName = args[0];      
      Assembly assembly = Assembly.LoadFrom(assemblyFileName);
      loadedAssemblies.AddAssembly(assembly);
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
    }
  }
}

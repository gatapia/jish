using System.Reflection;

namespace js.net.jish.Command
{
  public class AssemblyCommand : ParseInputCommand
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

    public override void Execute(string input)
    {
      string assemblyFileName = ParseFileOrTypeName(input);      
      Assembly assembly = Assembly.LoadFrom(assemblyFileName);
      loadedAssemblies.AddAssembly(assembly);
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
    }
  }
}

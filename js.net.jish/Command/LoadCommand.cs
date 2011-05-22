using System.Reflection;

namespace js.net.jish.Command
{
  public class LoadCommand : ParseInputCommand, ICommand
  {    

    private readonly ICommandLineInterpreter cli;
    private readonly JSConsole console;

    public LoadCommand(ICommandLineInterpreter cli, JSConsole console)
    {
      this.cli = cli;
      this.console = console;
    }

    public string GetName()
    {
      return "load";
    }

    public string GetHelpDescription()
    {
      return "Loads a .Net assembly in preparation for .using calls.";
    }

    public void Execute(string input)
    {
      string assemblyFileName = ParseFileOrTypeName(input);
      Assembly assembly = Assembly.LoadFrom(assemblyFileName);
      cli.GetLoadedAssemblies()[assembly.GetName().Name] = assembly;
      console.log("Assembly '" + assembly.GetName().Name + "' loaded.");
    }
  }
}

namespace js.net.jish.Command
{
  public class StaticCommand : ParseInputCommand, ICommand
  {
    private readonly ICommandLineInterpreter cli;
    private readonly JSConsole console;

    public StaticCommand(ICommandLineInterpreter cli, JSConsole console)
    {
      this.cli = cli;
      this.console = console;
    }

    public string GetName()
    {
      return "static";
    }

    public string GetHelpDescription()
    {
      return "Loads all static members of a .Net utility class and makes them available to you in Jish.";
    }

    public void Execute(string input)
    {
      string nameSpaceAndClass = ParseFileOrTypeName(input).Replace("\"", "").Replace("'", "").Trim();
      new TypeImporter(cli, nameSpaceAndClass, console).ImportType();
    }
  }
}

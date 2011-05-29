namespace js.net.jish.Command
{
  public class StaticCommand : ParseInputCommand
  {
    public override string GetName()
    {
      return "static";
    }

    public override string GetHelpDescription()
    {
      return "Loads all static members of a .Net utility class and makes them available to you in Jish.";
    }

    public override void Execute(string input)
    {
      string nameSpaceAndClass = ParseFileOrTypeName(input).Replace("\"", "").Replace("'", "").Trim();
      new TypeImporter(JishEngine).ImportType(nameSpaceAndClass);
    }
  }
}

using System.Collections.Generic;

namespace js.net.jish.Command.InlineCommand
{
  /// <summary>
  /// This command is actually implemented in jish.js.  This class is only
  /// here for help purposes.
  /// </summary>
  public class ClosureLibraryCommand : IInlineCommand
  {    
    
    public string GetNameSpace()
    {
      return "jish";
    }       

    public string GetName()
    {
      return "closure";
    }

    public string GetHelpDescription()
    {
      return "Loads google closure library environment.";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      return new[]
      {
        new CommandParam {Name = "baseJsPath"},        
      };
    }
  }
}

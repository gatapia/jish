using System.Collections.Generic;

namespace js.net.jish.Command
{
  public interface ICommandBase
  {
    string GetName();
    string GetHelpDescription();
    IEnumerable<CommandParam> GetParameters();
  }
}
using System.Collections.Generic;

namespace js.net.jish.Command
{
  public abstract class EmptyCommand : ICommand
  {
    public abstract string GetName();
    public abstract string GetHelpDescription();
    public abstract IEnumerable<CommandParam> GetParameters();
    public abstract void Execute(params string[] args);    
  }
}

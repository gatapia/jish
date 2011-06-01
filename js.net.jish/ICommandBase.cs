using System;
using System.Collections.Generic;

namespace js.net.jish
{
  public interface ICommandBase
  {
    string GetName();
    string GetHelpDescription();
    IEnumerable<CommandParam> GetParameters();
  }
}
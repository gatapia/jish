using System;
using System.Collections.Generic;
using System.Linq;

namespace js.net.jish
{
  public class CommandArgumentValidator
  {
    private readonly ICommandBase command;

    public CommandArgumentValidator(ICommandBase command)
    {
      this.command = command;
    }

    public string ValidateArguments(object[] actualArguments)
    {
      IEnumerable<CommandParam> expected = command.GetParameters();
      if (expected == null || expected.Count() == 0)
      {
        if (actualArguments != null && actualArguments.Length > 0)
        {
          return "Command [." + command.GetName() + "] should not be invoked with any arguments.";
        }
        return String.Empty;
      }            
      if (actualArguments == null || actualArguments.Length == 0)
      {
        return "Expected " + expected.Count() + " Names [" +
                       String.Join(", ", expected) + "].  Got no arguments.";
      }
      if (actualArguments.Length == expected.Count()) return String.Empty;

      string message = "Expected " + expected.Count() + " Names [" +
                       String.Join(", ", expected) + "].  Only got " + actualArguments.Length + " [" +
                       String.Join(", ", actualArguments) + "]";

      CommandParam last = expected.Last();
      if (!last.IsParams) { return message; }
      int diff = expected.Count() - actualArguments.Length;
      return (diff == 1 || diff < 0) ? String.Empty : message;
    }
  }
}
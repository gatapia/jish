using System;

namespace js.net.jish.Command
{
  public abstract class EmptyCommand : ICommand
  {
    public IJishInterpreter JishEngine { get; set; }    

    public abstract string GetName();
    public abstract string GetHelpDescription();
    public abstract string ValidateArgumentsBeforeExecute(params string[] args);

    public abstract void Execute(params string[] args);

    protected string AssertExpectedArguments(string[] expectedArgumentNames, string[] actualArguments, bool lastArgOptional = false, bool lastArgParams = false)
    {
      if (expectedArgumentNames == null || expectedArgumentNames.Length == 0)
      {
        if (actualArguments != null && actualArguments.Length > 0)
        {
          return "Command [." + GetName() + "] should not be invoked with any arguments.";
        }
        return String.Empty;
      }            
      if (actualArguments == null || actualArguments.Length == 0)
      {
        return "Expected " + expectedArgumentNames.Length + " Names [" +
                       String.Join(", ", expectedArgumentNames) + "].  Got no arguments.";
      }
      if (actualArguments.Length == expectedArgumentNames.Length) return String.Empty;

      string message = "Expected " + expectedArgumentNames.Length + " Names [" +
                       String.Join(", ", expectedArgumentNames) + "].  Only got " + actualArguments.Length + " [" +
                       String.Join(", ", actualArguments) + "]";

      if (!lastArgOptional && !lastArgParams) return message;

      if (actualArguments.Length > expectedArgumentNames.Length)
      {
        return lastArgParams ? String.Empty : message;
      }
      return lastArgOptional && expectedArgumentNames.Length - actualArguments.Length == 1 ? String.Empty : message;
    }
  }
}

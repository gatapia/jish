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

    protected string AssertExpectedArguments(string[] expectedArgumentNames, params string[] actualArguments, bool lastArgOptional = false, bool lastArgParams = false;)
    {
      if (expectedArgumentNames == null || expectedArgumentNames.Length == 0)
      {
        if (actualArguments != null && actualArguments.Length > 0)
        {
          return "Command [." + GetName() + "] should not be invoked with any arguments.";
        }
        return String.Empty;
      } else if (actualArguments == null || actualArguments.Length == 0 || actualArguments.Length != actualArguments.Length)
      {
        return "Expected " + expectedArgumentNames.Length + " arguments [" + String.Join(", ", expectedArgumentNames) + "].  Only got " + actualArguments.Length + " [" + String.Join(", ", actualArguments) + "]";
      }
      return String.Empty;
    }
  }
}

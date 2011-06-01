using System;
using System.Diagnostics;

namespace js.net.jish.Command
{
  public class ProcessCommand : EmptyCommand
  {
    private readonly JSConsole console;

    public ProcessCommand(JSConsole console)
    {
      this.console = console;
    }

    public override string GetName()
    {
      return "process";
    }

    public override string GetHelpDescription()
    {
      return "Executes the command in a separate Process.";
    }

    public override string ValidateArgumentsBeforeExecute(params string[] args)
    {
      return AssertExpectedArguments(new [] {"command", "arguments"});
    }

    public override void Execute(params string[] args)
    {
      string command = args[0];
      string arguments = args[1];
      using (var process = new Process
                      {
                        StartInfo =
                          {
                            FileName = command,
                            Arguments = arguments,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                          }
                      })
      {
        process.Start();
        string err = process.StandardError.ReadToEnd();
        string output = process.StandardOutput.ReadToEnd();
        if (!String.IsNullOrWhiteSpace(err)) console.log(err);
        if (!String.IsNullOrWhiteSpace(output)) console.log(output);
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
          throw new ApplicationException("Process [" + command + "] args [" + arguments+ "] exited with code: " + process.ExitCode);
        }
      }
    }
  }
}
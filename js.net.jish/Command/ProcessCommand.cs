using System;
using System.Collections.Generic;
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

    public override IEnumerable<CommandParm> GetParameters()
    {
      CommandParm a1 = new CommandParm { Name = "command" };
      CommandParm a2 = new CommandParm { Name = "arguments", Null = true};
      return new[] { a1, a2 };
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
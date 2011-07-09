using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using js.net.Util;

namespace js.net.jish.Command.InlineCommand
{
  public class LoadCommand : IInlineCommand
  {
    private readonly JSConsole console;

    public LoadCommand(JSConsole console)
    {
      Trace.Assert(console != null);

      this.console = console;
    }

    public string GetName()
    {
      return "load";
    }

    public string GetHelpDescription()
    {
      return "Load and executes another Jish or plain JavaScript file.";
    }

    public IEnumerable<CommandParam> GetParameters()
    {
      CommandParam a1 = new CommandParam { Name = "file" };
      return new[] { a1 };
    }

    public string GetNameSpace()
    {
      return "jish";
    }

    public string loadFileImpl(string file) 
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      Trace.Assert(File.Exists(file));

      return File.ReadAllText(file);
    }
  }
}
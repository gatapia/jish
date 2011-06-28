using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using js.net.jish.Command.ConsoleCommand;
using js.net.jish.Command.InlineCommand;

namespace js.net.jish.Command
{
  public class HelpMgr
  {
    private readonly IDictionary<string, string> specialCommandHelps = new Dictionary<string, string>();
    private readonly IDictionary<string, string> inlineCommandHelps = new Dictionary<string, string>();
    private string helpCached;

    public void AddHelpForConsoleCommand(IConsoleCommand command)
    {
      Trace.Assert(command != null);

      AddHelpImpl(specialCommandHelps, command);
    }

    public void AddHelpForInlineCommand(IInlineCommand command)
    { 
      Trace.Assert(command != null);

      AddHelpImpl(inlineCommandHelps, command);      
    }

    public string GetHelpString()
    {
      return helpCached ?? (helpCached = RenderHelp());
    }    

    private void AddHelpImpl(IDictionary<string, string> dictionary, ICommandBase command)
    {
      Trace.Assert(command != null);
      Trace.Assert(dictionary != null);
      Trace.Assert(!dictionary.ContainsKey(command.GetName())); 

      dictionary.Add(command.GetName(), GetHelpRowForCommand(command));
    }

    private string GetHelpRowForCommand(ICommandBase command)
    {
      Trace.Assert(command != null);

      string commandName = GetCommandName(command);
      StringBuilder sb = new StringBuilder(commandName);
      sb.Append(":\n");
      sb.Append('\t').Append(command.GetHelpDescription()).Append('\n');
      sb.Append("\tArguments: ").Append(GetArgumentDescriptionFor(command)).Append('\n');
      return sb.ToString();
    }

    private string GetCommandName(ICommandBase command)
    {
      Trace.Assert(command != null);

      if (command is IConsoleCommand) { return '.' + command.GetName(); }

      IInlineCommand icommand = (IInlineCommand) command;
      return icommand.GetNameSpace() + '.' + icommand.GetName();
    }

    private string GetArgumentDescriptionFor(ICommandBase command)
    {
      Trace.Assert(command != null);

      IEnumerable<CommandParam> args = command.GetParameters();
      return '(' + (args == null ? "" : String.Join(", ", args.Select(GetArgumentName).ToArray())) + ')';
    }

    private string GetArgumentName(CommandParam param)
    {
      Trace.Assert(param != null);

      string name = param.Name;
      if (param.IsParams)
      {
        name = "param object[] " + name;
      } else if (param.Null)
      {
        name += "?";
      }
      return name;
    }

    private string RenderHelp()
    {
      StringBuilder sb = new StringBuilder("Jish Help\n");
      sb.Append(new String('=', sb.Length - 1)).Append("\n\n");
      sb.Append("Console Commands\n\n");
      AddCommandsToBuilder(specialCommandHelps, sb);
      sb.Append("\nInline Commands\n\n");
      AddCommandsToBuilder(inlineCommandHelps, sb);
      return sb.ToString();
    }

    private void AddCommandsToBuilder(IDictionary<string, string> commands, StringBuilder sb)
    {
      Trace.Assert(commands != null);
      Trace.Assert(sb != null);

      IEnumerable<string> keysSorted = commands.Keys.OrderBy(k => k);
      foreach (string key in keysSorted)
      {
        sb.Append(commands[key]).Append('\n');
      }
    }
  }
}
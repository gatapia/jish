using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using js.net.jish.Command;
using js.net.jish.InlineCommand;
using System.Linq;

namespace js.net.jish
{
  public class HelpMgr
  {
    private readonly IDictionary<string, string> specialCommandHelps = new Dictionary<string, string>();
    private readonly IDictionary<string, string> inlineCommandHelps = new Dictionary<string, string>();
    private string helpCached;

    public void AddHelpForSpecialCommand(ICommand command)
    {
      AddHelpImpl(specialCommandHelps, command);
    }

    public void AddHelpForInlineCommand(IInlineCommand command)
    {      
      AddHelpImpl(inlineCommandHelps, command);      
    }

    public string GetHelpString()
    {
      return helpCached ?? (helpCached = RenderHelp());
    }    

    private void AddHelpImpl(IDictionary<string, string> dictionary, ICommandBase command)
    {
      Trace.Assert(!dictionary.ContainsKey(command.GetName())); 
      dictionary.Add(command.GetName(), GetHelpRowForCommand(command));
    }

    private string GetHelpRowForCommand(ICommandBase command)
    {
      string commandName = GetCommandName(command);
      StringBuilder sb = new StringBuilder(commandName);
      sb.Append(":\n");
      sb.Append('\t').Append(command.GetHelpDescription()).Append('\n');
      sb.Append("\tArguments: ").Append(GetArgumentDescriptionFor(command)).Append('\n');
      return sb.ToString();
    }

    private string GetCommandName(ICommandBase command)
    {
      if (command is ICommand) { return '.' + command.GetName(); }

      IInlineCommand icommand = (IInlineCommand) command;
      return icommand.GetNameSpace() + '.' + icommand.GetName();
    }

    private string GetArgumentDescriptionFor(ICommandBase command)
    {
      IEnumerable<CommandParam> args = command.GetParameters();
      return '(' + String.Join(", ", args.Select(GetArgumentName).ToArray()) + ')';
    }

    private string GetArgumentName(CommandParam param)
    {
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
      IEnumerable<string> keysSorted = commands.Keys.OrderBy(k => k);
      foreach (string key in keysSorted)
      {
        sb.Append(commands[key]).Append('\n');
      }
    }
  }
}
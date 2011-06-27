using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using js.net.jish.Command;
using js.net.jish.Command.ConsoleCommand;
using js.net.jish.Command.InlineCommand;
using js.net.Util;
using Ninject;

namespace js.net.jish.Util
{
  public class LoadedAssembliesBucket
  {
    private readonly IDictionary<string, Type> commands = new Dictionary<string, Type>();    

    private readonly IDictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
    private readonly IDictionary<string, IEnumerable<IInlineCommand>> loadedInlineCommands = new Dictionary<string, IEnumerable<IInlineCommand>>();

    private readonly IKernel kernel;    
    private readonly HelpMgr helpManager;
    private readonly JSConsole console;

    public LoadedAssembliesBucket(HelpMgr helpManager, IKernel kernel, JSConsole console)
    {      
      Trace.Assert(helpManager != null);
      Trace.Assert(kernel != null);
      Trace.Assert(console != null);

      this.console = console;
      this.kernel = kernel;
      this.helpManager = helpManager;
    }

    public IEnumerable<IInlineCommand> AddAssembly(Assembly a)
    {
      Trace.Assert(a != null);
      string name = a.GetName().Name;
      Trace.Assert(!ContainsAssembly(name), "Assembly: " + name + " is already loaded.");
      //Console.WriteLine("Loading: " + name);
      assemblies.Add(name, a);
      
      LoadAllCommandsFromAssembly(a);
      IEnumerable<IInlineCommand> assemblyCommands = LoadAllInlineCommandsFromAssembly(a);
      loadedInlineCommands.Add(name, assemblyCommands);
      return assemblyCommands;
    }

    public bool ContainsAssembly(string assemblyName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(assemblyName));

      return assemblies.ContainsKey(GetShortNameFrom(assemblyName));
    }

    public Assembly GetAssembly(string assemblyName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(assemblyName));

      return assemblies[GetShortNameFrom(assemblyName)];
    }

    public void InterceptSpecialCommands(string input)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(input));
      Trace.Assert(input.StartsWith("."));

      string commandName = new Regex(@"\.([A-z0-9])+").Match(input).Captures[0].Value.Substring(1).Trim();
      if (commandName.Equals("help"))
      {
        console.log(helpManager.GetHelpString());
        return;
      }
      if (!commands.ContainsKey(commandName))
      {
        console.log("Could not find command: " + input);
        return;
      }
      IConsoleCommand command = (IConsoleCommand) kernel.Get(commands[commandName]);
      command.Execute(ParseSpecialCommandInputs(input));
    }

    private string[] ParseSpecialCommandInputs(string input)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(input));

      if (input.IndexOf('(') < 0) return null;
      input = input.Substring(input.IndexOf('(') + 1); 
      input = input.Substring(0, input.LastIndexOf(')')); 
      return Regex.Split(input, ",(?=(?:[^']*'[^']*')*[^']*$)").Select(s => s.Trim().Replace("\"", "").Replace("'", "")).ToArray();
    }

    private string GetShortNameFrom(string assemblyName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(assemblyName));

      if (assemblyName.IndexOf(',') < 0 ) return assemblyName;
      return assemblyName.Substring(0, assemblyName.IndexOf(','));
    }

    public IEnumerable<Assembly> GetAllAssemblies()
    {
      Trace.Assert(assemblies != null);

      return assemblies.Values.ToArray();
    }

    private void LoadAllCommandsFromAssembly(Assembly assembly)
    {
      Trace.Assert(assembly != null);

      foreach (Type t in GetAllTypesThatImplement(assembly, typeof(IConsoleCommand)))
      {
        IConsoleCommand command = (IConsoleCommand) kernel.Get(t);
        commands.Add(command.GetName(), t);
        if (command.GetType().Assembly.FullName.IndexOf("js.net.test.module") < 0)
        {
          helpManager.AddHelpForConsoleCommand(command);
        }
      }
    }

    private IEnumerable<IInlineCommand> LoadAllInlineCommandsFromAssembly(Assembly assembly)
    {
      IDictionary<string, IList<IInlineCommand>> icommands = new Dictionary<string, IList<IInlineCommand>>();
      foreach (Type t in GetAllTypesThatImplement(assembly, typeof(IInlineCommand)))
      {
        IInlineCommand icommand = (IInlineCommand) kernel.Get(t);        
        string ns = icommand.GetNameSpace();        
        if (String.IsNullOrWhiteSpace(ns)) { throw new ApplicationException("Could not load inline command from type[" + t.FullName + "].  No namespace specified.");}
        if (ns.IndexOf('.') > 0) { throw new ApplicationException("Nested namespaces (namespaces with '.' in them) are not supported."); }
        if (!icommands.ContainsKey(ns)) icommands.Add(ns, new List<IInlineCommand>());
        if (icommand.GetType().Assembly.FullName.IndexOf("js.net.test.module") < 0)
        {
          helpManager.AddHelpForInlineCommand(icommand);
        }      
        icommands[ns].Add(icommand);
      }
      return icommands.Values.SelectMany(c => c);      
    }    

    private IEnumerable<Type> GetAllTypesThatImplement(Assembly assembly, Type iface)
    {
      try
      {
        Type[] types = assembly.GetTypes();
        return types.Where(t => !t.IsAbstract && iface.IsAssignableFrom(t));
      } catch (ReflectionTypeLoadException ex) {
        foreach(Exception inner in ex.LoaderExceptions) {
            Console.WriteLine(inner);
        }
        throw;
      }
    }

  }
}
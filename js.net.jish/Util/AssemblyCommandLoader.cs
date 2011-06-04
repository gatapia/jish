﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using js.net.Engine;
using js.net.jish.IL;
using js.net.jish.InlineCommand;

namespace js.net.jish.Util
{
  public class AssemblyCommandLoader
  {
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly IEngine engine;

    public AssemblyCommandLoader(LoadedAssembliesBucket loadedAssemblies, IEngine engine)
    {
      this.loadedAssemblies = loadedAssemblies;
      this.engine = engine;
    }

    public void LoadCommandsFromAssembly(Assembly assembly)
    {
      IEnumerable<IInlineCommand> commands = loadedAssemblies.AddAssembly(assembly);
      if (commands.Any()) InjectCommandsIntoGlobalScope(commands);
    }

    private void InjectCommandsIntoGlobalScope(IEnumerable<IInlineCommand> commands)
    {
            const string jsBinder = 
@"
global['{0}']['{1}'] = function() {{
  return global['{2}']['{1}'].apply(global, arguments)
}};
";
      StringBuilder js = new StringBuilder();

      var nsCommands = commands.GroupBy(c => c.GetNameSpace());
      foreach (var commandsInNamespace in nsCommands)
      {
        string ns = commandsInNamespace.Key;
        object namespaceCommand = GetNamespaceCommandProxy(commandsInNamespace);

        js.Append(String.Format("\nif (!global['{0}']) global['{0}'] = {{}};\n", ns));

        string tmpClassName = "__" + Guid.NewGuid();
        engine.SetGlobal(tmpClassName, namespaceCommand);

        foreach (string method in namespaceCommand.GetType().GetMethods().Select(mi => mi.Name).Distinct().Where(m => Char.IsLower(m[0])))
        {
          js.Append(String.Format(jsBinder, commandsInNamespace.Key, method, tmpClassName));
        }
      }
      engine.Run(js.ToString(), "JishInterpreter.InjectCommandsIntoGlobalScope");
    }

    private object GetNamespaceCommandProxy(IEnumerable<IInlineCommand> commands)
    {      
      var methods = new List<ProxyMethod>();
      foreach (var command in commands)
      {
        var thisMethods = command.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => Char.IsLower(m.Name[0]));
        foreach (MethodInfo mi in thisMethods)
        {
          methods.Add(new ProxyMethod(mi, command));
        }
      }
      var nsWrapper = new TypeILWrapper().CreateWrapper(commands.First().GetType(), methods.ToArray());
      return nsWrapper;
    }
  }
}
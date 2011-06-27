using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using js.net.Engine;
using js.net.jish.Command.InlineCommand;
using js.net.jish.IL;

namespace js.net.jish.Util
{
  public class AssemblyCommandLoader
  {
    private readonly LoadedAssembliesBucket loadedAssemblies;
    private readonly IEngine engine;
    private const string jsBinder = 
@"
global['{0}']['{1}'] = function() {{
  return global['{2}']['{1}'].apply(global, arguments)
}};
";

    public AssemblyCommandLoader(LoadedAssembliesBucket loadedAssemblies, IEngine engine)
    {
      Trace.Assert(loadedAssemblies != null);
      Trace.Assert(engine != null);

      this.loadedAssemblies = loadedAssemblies;
      this.engine = engine;
    }

    public void LoadCommandsFromAssembly(Assembly assembly)
    {
      Trace.Assert(assembly != null);
      if (loadedAssemblies.ContainsAssembly(assembly.GetName().Name)) { return; }

      IEnumerable<IInlineCommand> commands = loadedAssemblies.AddAssembly(assembly);
      if (commands.Any()) InjectCommandsIntoGlobalScope(commands);
    }

    private void InjectCommandsIntoGlobalScope(IEnumerable<IInlineCommand> commands)
    {
      Trace.Assert(commands != null);
      
      StringBuilder js = new StringBuilder();
      foreach (var commandsInNamespace in commands.GroupBy(c => c.GetNameSpace()))
      {
        CreateAndInjectNamespaceProxy(commandsInNamespace, js);
      }
      engine.Run(js.ToString(), "JishInterpreter.InjectCommandsIntoGlobalScope");
    }

    private void CreateAndInjectNamespaceProxy(IGrouping<string, IInlineCommand> commandsInNamespace, StringBuilder jsBuilder)
    {
      Trace.Assert(commandsInNamespace != null);
      Trace.Assert(jsBuilder != null);

      string ns = commandsInNamespace.Key;
      object namespaceCommand = GetNamespaceCommandProxy(commandsInNamespace);

      jsBuilder.Append(String.Format("\nif (!global['{0}']) global['{0}'] = {{}};\n", ns));

      string tmpClassName = "__" + Guid.NewGuid();
      engine.SetGlobal(tmpClassName, namespaceCommand);

      foreach (string method in namespaceCommand.GetType().GetMethods().Select(mi => mi.Name).Distinct().Where(m => Char.IsLower(m[0])))
      {
        jsBuilder.Append(String.Format(jsBinder, commandsInNamespace.Key, method, tmpClassName));
      }
    }

    private object GetNamespaceCommandProxy(IEnumerable<IInlineCommand> commands)
    {      
      Trace.Assert(commands != null);

      var methods = new List<MethodToProxify>();
      foreach (var command in commands)
      {
        var thisMethods = command.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => Char.IsLower(m.Name[0]));
        foreach (MethodInfo mi in thisMethods)
        {
          methods.Add(new MethodToProxify(mi, command));
        }
      }
      var nsWrapper = new TypeILWrapper().CreateWrapper(commands.First().GetType(), methods.ToArray());
      return nsWrapper;
    }
  }
}

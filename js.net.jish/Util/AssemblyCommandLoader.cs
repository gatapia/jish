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

    public IDictionary<string, object> GetCommandsMapFromAssembly(Assembly assembly)
    {
      Trace.Assert(assembly != null);

      IEnumerable<IInlineCommand> commands = GetInlineCommandsInAssembly(assembly);
      if (commands == null) return null;      
      IDictionary<string, object> namespaceProxies = new Dictionary<string, object>();
      foreach (var commandsInNamespace in commands.GroupBy(c => c.GetNameSpace()))
      {
        object proxy = GetNamespaceCommandProxy(commandsInNamespace);
        namespaceProxies.Add(commandsInNamespace.Key, proxy);
      }
      return namespaceProxies;
    }

    public void LoadCommandsFromAssemblyAndInjectIntoGlobalScope(Assembly assembly)
    {
      Trace.Assert(assembly != null);

      IDictionary<string, object> commandsMap = GetCommandsMapFromAssembly(assembly);
      if (commandsMap == null || !commandsMap.Any()) return;

      StringBuilder js = new StringBuilder();
      foreach (KeyValuePair<string, object> nsAndProxy in commandsMap)
      {
         CreateInjectProxyIntoGlobalScopeJS(nsAndProxy.Value, nsAndProxy.Key, js);
      }
      engine.Run(js.ToString(), "JishInterpreter.InjectCommandsIntoGlobalScope");
    }

    private IEnumerable<IInlineCommand> GetInlineCommandsInAssembly(Assembly assembly) {
      Trace.Assert(assembly != null);
      if (loadedAssemblies.ContainsAssembly(assembly.GetName().Name)) { return null; }

      return loadedAssemblies.AddAssembly(assembly);
    }

    private void CreateInjectProxyIntoGlobalScopeJS(object namespaceCommand, string ns, StringBuilder jsBuilder)
    {
      jsBuilder.Append(String.Format("\nif (!global['{0}']) global['{0}'] = {{}};\n", ns));

      string tmpClassName = "__" + Guid.NewGuid();
      engine.SetGlobal(tmpClassName, namespaceCommand);

      foreach (string method in namespaceCommand.GetType().GetMethods().Select(mi => mi.Name).Distinct().Where(m => Char.IsLower(m[0])))
      {
        jsBuilder.Append(String.Format(jsBinder, ns, method, tmpClassName));
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
      var nsWrapper = new TypeILWrapper().CreateWrapperFromType(commands.First().GetType(), methods.ToArray());
      return nsWrapper;
    }
  }
}

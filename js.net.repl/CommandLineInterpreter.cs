using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using js.net.Engine;
using js.net.Util;

namespace js.net.repl
{
  public class CommandLineInterpreter
  {
    private readonly JSConsole console;
    private readonly IEngine engine;

    public CommandLineInterpreter(IEngine engine, JSConsole console)
    {
      Trace.Assert(engine != null);
      Trace.Assert(console != null);

      this.engine = engine;
      this.console = console;

      InitialiseConsole();

      Console.WriteLine("Type '.help' for options.");
    }

    private void InitialiseConsole()
    {
      Console.TreatControlCAsInput = false;
      Console.CancelKeyPress += (s, e) => Environment.Exit(0);
    }

    public bool ReadAndExecuteCommand()
    {
      Console.Write("> ");
      string input = Console.ReadLine().Trim();
      if (String.IsNullOrWhiteSpace(input) || InterceptSpecialCommands(input))
      {
        return true;
      }

      try
      {
        object val = engine.Run(input);
        if (val != null && val != String.Empty) console.log(val);
        if (val != null) engine.SetGlobal("_", val);
      }
      catch (Exception e)
      {
        PrintExceptionMessage(e);
      }
      return true;
    }

    private void PrintExceptionMessage(Exception e)
    {
      string msg = e.Message;
      if (msg.IndexOf(": ") > 0) msg = msg.Substring(msg.IndexOf(": ") + 2);
      if (msg.IndexOf('(') > 0) msg = msg.Substring(0, msg.IndexOf('('));

      Console.WriteLine(msg);
    }

    private bool InterceptSpecialCommands(string input)
    {
      if (input.Equals(".exit"))
      {
        Environment.Exit(0);
      }
      else if (input.Equals(".break"))
      {
        Console.WriteLine("Not implemented...");
      }
      else if (input.Equals(".help"))
      {
        Console.WriteLine(new EmbeddedResourcesUtils().ReadEmbeddedResourceTextContents(
          "js.net.repl.resources.help.txt", GetType().Assembly));
      }
      else if (input.Equals(".clear"))
      {
        Console.WriteLine("Clearing context...");
        engine.Run(
          @"
for (var i in this) {
  if (i === 'console' || i === 'global') continue;
  delete this[i];
};
"
          );
      }
      else if (input.StartsWith(".using"))
      {
        ImportClassIntoGlobalNamespace(input.Substring(6));
      }
      else
      {
        return false;
      }
      return true;
    }

    private void ImportClassIntoGlobalNamespace(string input)
    {
      string nameSpaceAndClass = new Regex(@"([A-z\.])+").Match(input).Captures[0].Value;
      string className = nameSpaceAndClass.Substring(nameSpaceAndClass.LastIndexOf('.') + 1);
      Type t = Type.GetType(nameSpaceAndClass);
      if (t == null)
      {
        Console.WriteLine("Could not find type: " + nameSpaceAndClass);
        return;
      }
      var dyn = new Dictionary<string, object>();
      ScrapeMethods(t, dyn);
      engine.SetGlobal(className, dyn);
      Console.WriteLine(nameSpaceAndClass + " imported.  Use like: " + className + ".Method(args);");
    }

    private void ScrapeMethods(Type targetType, IDictionary<string, object> methods)
    {
      foreach (MethodInfo mi in targetType.GetMethods(BindingFlags.Public | BindingFlags.Static))
      {
        // Note: Only adding one instance of each method (no overrides)
        if (methods.ContainsKey(mi.Name)) { continue; }
        methods.Add(mi.Name, ToDelegate(mi, null));
      }
    }

    public Delegate ToDelegate(MethodInfo mi, object target)
    {
      if (mi == null) throw new ArgumentNullException("mi");

      Type delegateType;

      List<Type> typeArgs = mi.GetParameters()
        .Select(p => p.ParameterType)
        .ToList();

      // builds a delegate type
      if (mi.ReturnType == typeof (void))
      {
        delegateType = Expression.GetActionType(typeArgs.ToArray());
      }
      else
      {
        typeArgs.Add(mi.ReturnType);
        delegateType = Expression.GetFuncType(typeArgs.ToArray());
      }

      // creates a binded delegate if target is supplied
      Delegate result = (target == null)
                          ? Delegate.CreateDelegate(delegateType, mi)
                          : Delegate.CreateDelegate(delegateType, target, mi);

      return result;
    }
  }
}
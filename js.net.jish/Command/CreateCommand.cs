using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace js.net.jish.Command
{
  public class CreateCommand : ParseInputCommand
  {
    public override string GetName()
    {
      return "create";
    }

    public override string GetHelpDescription()
    {
      return "Creates an instance of an object and stores it in the specified global name.";
    }

    public override void Execute(string input)
    {
      string typeArgsAndGlobalName = ParseFileOrTypeName(input);
      string[] split = Regex.Split(typeArgsAndGlobalName, ",(?=(?:[^']*'[^']*')*[^']*$)").Select(s => s.Trim().Replace("\"", "").Replace("'", "")).ToArray();
      string[] args = split.Skip(1).Take(split.Length - 2).ToArray();
      string typeStr = split[0];
      Type t = new TypeLoader().LoadType(typeStr, JishEngine.GetLoadedAssemblies());
      if (t == null)
      {
        JishEngine.JavaScriptConsole.log("Could not find a matching type: " + typeStr);
        return;
      }
      object[] realArgs = ConvertToActualArgs(args, t);
      if (realArgs == null)
      {
        JishEngine.JavaScriptConsole.log("Could not find a matching constructor.");
        return;
      }
      object instance;
      if (t.GetConstructors(BindingFlags.Public).Length == 0)
      {

      } else
      {
        instance = Activator.CreateInstance(t, realArgs);
      }
      JishEngine.SetGlobal(split.Last(), instance);
    }

    private object[] ConvertToActualArgs(string[] args, Type type)
    {      
      if (args.Length == 0) return new object[] {};
      IEnumerable<ConstructorInfo> possibleConstructors = type.GetConstructors().Where(c => c.GetParameters().Length == args.Length);      
      foreach (ConstructorInfo ci in possibleConstructors)
      {
        IEnumerable<object> realArgs = ConvertToActualArgs(args, ci);
        if (realArgs != null) return realArgs.ToArray();
      }
      return null;
    }

    private IEnumerable<object> ConvertToActualArgs(string[] args, ConstructorInfo ci)
    {
      IList<object> realArgs =new List<object>();
      for (int i = 0; i < args.Length; i++)
      {
        Type exp = ci.GetParameters()[i].ParameterType;
        string strArgs = args[i];
        try
        {
          object actual = Convert.ChangeType(strArgs, exp);
          if (actual == null && !String.IsNullOrWhiteSpace(strArgs)) return null; // Not matching        
          realArgs.Add(actual);        
        } catch (FormatException)
        {
          return null; // Not matching
        }
      }
      return realArgs;
    }
  }
}

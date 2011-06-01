using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using js.net.jish.Util;

namespace js.net.jish.Command
{
  public class CreateCommand : EmptyCommand
  {
    private readonly TypeLoader typeLoader;
    private readonly JSConsole console;

    public CreateCommand(TypeLoader typeLoader, JSConsole console)
    {
      this.typeLoader = typeLoader;
      this.console = console;
    }

    public override string GetName()
    {
      return "create";
    }

    public override string GetHelpDescription()
    {
      return "Creates an instance of an object and stores it in the specified global name.";
    }

    public override string ValidateArgumentsBeforeExecute(params string[] args)
    {
      return AssertExpectedArguments(new [] {"typeName", "globalVariableName", "constructor args (if any)"}, true, true);
    }

    public override void Execute(params string[] args)
    {            
      string typeStr = args[0];
      string globalName = args[1];
      string[] constructorArgsAsString = args.Skip(2).ToArray();

      Type t = typeLoader.LoadType(typeStr);
      if (t == null)
      {
        console.log("Could not find a matching type: " + typeStr);
        return;
      }
      object[] realArgs = ConvertToActualArgs(constructorArgsAsString, t);
      if (realArgs == null)
      {
        console.log("Could not find a matching constructor.");
        return;
      }
      object instance = t.GetConstructors().Length == 0 
        ? new StaticTypeWrapper(t).CreateWrapper() 
        : Activator.CreateInstance(t, realArgs);
      
      JishEngine.SetGlobal(globalName, instance);
      console.log("Created instance of " + typeStr + ". Saved in global '" + globalName + "'");
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

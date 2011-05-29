using System;
using System.IO;
using System.Reflection;

namespace js.net.jish.Command
{
  public class AssemblyCommand : ParseInputCommand
  {    
    public override string GetName()
    {
      return "assembly";
    }

    public override string GetHelpDescription()
    {
      return "Loads a .Net assembly in preparation for .static calls.";
    }

    public override void Execute(string input)
    {
      string assemblyFileName = ParseFileOrTypeName(input);      
      Assembly assembly = Assembly.LoadFrom(assemblyFileName);
      JishEngine.GetLoadedAssemblies()[assembly.GetName().Name] = assembly;
      JavaScriptConsole.log("Assembly '" + assembly.GetName().Name + "' loaded.");
    }
  }
}

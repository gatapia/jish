using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace js.net.jish.Util
{
  public class EmbeddedAssemblyLoader
  {    
    public static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
      if (args.Name.StartsWith("Noesis.Javascript")) return null;
      return new EmbeddedAssemblyLoader("js.net.jish.resources." + args.Name).LoadAssembly();
    }

    private readonly string assemblyResourceShortName;

    public EmbeddedAssemblyLoader(string assemblyResourceName)
    {
      assemblyResourceShortName = assemblyResourceName.Substring(0, assemblyResourceName.IndexOf(',')) + ".dll";
    }

    public Assembly LoadAssembly()
    {
      Assembly a = Assembly.GetExecutingAssembly();
      
      Trace.Assert(Array.IndexOf(a.GetManifestResourceNames(), assemblyResourceShortName) >= 0, "Assembly '" + a.FullName + "' does not contain resource '" + assemblyResourceShortName + "' - " + String.Join(", ", a.GetManifestResourceNames()));
      using(Stream s = a.GetManifestResourceStream(assemblyResourceShortName))
      {        
        byte[] assemblyBytes = new byte[s.Length];
        s.Read(assemblyBytes, 0, assemblyBytes.Length);
        return Assembly.Load(assemblyBytes);
      }
    }
  }
}
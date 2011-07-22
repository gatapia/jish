using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using js.net.Util;
using Jurassic;

namespace js.net.Engine
{
  public class JurassicEngine : AbstractEngine
  { 
    static JurassicEngine()
    {            
      new EmbeddedResourcesUtils().InjectJavaScriptNetAssemblyIntoRunningDir();
      AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
    }

    // This code is duplicated in js.net.jish.Util.EmbeddedAssemblyLoader but
    // not much that can be done.
    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
      string assemblyResourceShortName = args.Name;
      if (assemblyResourceShortName.IndexOf(',') > 0)
      {
        assemblyResourceShortName = assemblyResourceShortName.Substring(0, args.Name.IndexOf(','));
      }
      assemblyResourceShortName += ".dll";
      string dir = Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location);      
      string path = dir + "\\" + assemblyResourceShortName;
      if (File.Exists(path)) return Assembly.LoadFrom(path);

      Assembly a = Assembly.GetExecutingAssembly();
      
      Trace.Assert(Array.IndexOf(a.GetManifestResourceNames(), assemblyResourceShortName) >= 0, "Assembly '" + a.FullName + "' does not contain resource '" + assemblyResourceShortName + "' - " + String.Join(", ", a.GetManifestResourceNames()));
      using(Stream s = a.GetManifestResourceStream(assemblyResourceShortName))
      {
        byte[] assemblyBytes = new byte[s.Length];
        s.Read(assemblyBytes, 0, assemblyBytes.Length);
        return Assembly.Load(assemblyBytes);
      }
    }

    private ScriptEngine ctx;

    public JurassicEngine()
    {      
      ctx = new ScriptEngine();
    }

    public override object Run(string script, string fileName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(script));
      Trace.Assert(ctx != null);

      return ctx.Evaluate(new StringScriptSource(script, fileName));
    }

    public override void SetGlobal(string name, object value)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(ctx != null);

      ctx.SetGlobalValue(name, value);
    }

    public override object GetGlobal(string name)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(name));
      Trace.Assert(ctx != null);

      return ctx.GetGlobalValue(name);
    }

    public override void Dispose()
    {
      Trace.Assert(ctx != null);
      ctx = null;
    }
  }
}

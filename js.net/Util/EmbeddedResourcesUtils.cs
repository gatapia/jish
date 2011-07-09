using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace js.net.Util
{
  public class EmbeddedResourcesUtils
  {
    public string ReadEmbeddedResourceTextContents(string resourceName, Assembly assembly = null)
    {      
      Trace.Assert(!String.IsNullOrWhiteSpace(resourceName));
      if (assembly == null) assembly = Assembly.GetExecutingAssembly();
      Trace.Assert(Array.IndexOf(assembly.GetManifestResourceNames(), resourceName) >= 0, String.Join(", ", assembly.GetManifestResourceNames()));

      using(Stream s = assembly.GetManifestResourceStream(resourceName))
      {        
        using (StreamReader sr = new StreamReader(s))
        {
          return sr.ReadToEnd();
        }
      }
    } 
   
    public void InjectJavaScriptNetAssemblyIntoRunningDir()
    {
      Assembly ass = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
      string dir = Path.GetDirectoryName(ass.Location);
      string path = dir + "\\Noesis.Javascript.dll";

      if (File.Exists(path)) return;
      using(Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("js.net.resources.Noesis.Javascript.dll"))
      {
        byte[] assemblyBytes = new byte[s.Length];
        s.Read(assemblyBytes, 0, assemblyBytes.Length);
        File.WriteAllBytes(path, assemblyBytes);
      }
    }
  }
}

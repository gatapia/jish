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
      if (assembly == null) assembly = Assembly.GetExecutingAssembly();
      Trace.Assert(Array.IndexOf(assembly.GetManifestResourceNames(), resourceName) >= 0, 
        String.Join(", ", assembly.GetManifestResourceNames()));

      using(Stream s = assembly.GetManifestResourceStream(resourceName))
      {        
        using (StreamReader sr = new StreamReader(s))
        {
          return sr.ReadToEnd();
        }
      }
    }

    public void CopyAssemblyToExecutable(string fileName, string resourceName, Assembly assembly = null)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(fileName));
      Trace.Assert(!String.IsNullOrWhiteSpace(resourceName));
      if (assembly == null) assembly = Assembly.GetExecutingAssembly();

      Trace.Assert(Array.IndexOf(assembly.GetManifestResourceNames(), resourceName) >= 0, 
        String.Join(", ", assembly.GetManifestResourceNames()));

      if (File.Exists(fileName))
      {
        return;
      }

      using (Stream s = assembly.GetManifestResourceStream(resourceName))
      {
        using (FileStream fs = new FileStream(fileName, FileMode.Create))
        {
          byte[] b = new byte[s.Length];
          s.Read(b, 0, b.Length);
          fs.Write(b, 0, b.Length);
        }
      }
    }
  }
}

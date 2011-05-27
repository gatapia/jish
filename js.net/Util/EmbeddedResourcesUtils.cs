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
  }
}

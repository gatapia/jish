using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace js.net.Util
{
  public class EmbeddedResourcesUtils
  {
    public string ReadEmbeddedResourceTextContents(string resourceName)
    {
      Console.WriteLine(String.Join(", ", Assembly.GetExecutingAssembly().GetManifestResourceNames()));
      Trace.Assert(Array.IndexOf(Assembly.GetExecutingAssembly().GetManifestResourceNames(), resourceName) >= 0, 
        String.Join(", ", Assembly.GetExecutingAssembly().GetManifestResourceNames()));

      using(Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {        
        using (StreamReader sr = new StreamReader(s))
        {
          return sr.ReadToEnd();
        }
      }
    }

    public void CopyAssemblyToExecutable(string fileName, string resourceName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(fileName));
      Trace.Assert(!String.IsNullOrWhiteSpace(resourceName));
      Trace.Assert(Array.IndexOf(Assembly.GetExecutingAssembly().GetManifestResourceNames(), resourceName) >= 0, 
        String.Join(", ", Assembly.GetExecutingAssembly().GetManifestResourceNames()));

      if (File.Exists(fileName))
      {
        return;
      }

      using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
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

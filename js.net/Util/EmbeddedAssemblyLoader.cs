using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace js.net.Util
{
  public class EmbeddedAssemblyLoader
  {
    private readonly string fileName;
    private readonly string resourceName;

    public EmbeddedAssemblyLoader(string fileName, string resourceName)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(fileName));
      Trace.Assert(!String.IsNullOrWhiteSpace(resourceName));
      Trace.Assert(Array.IndexOf(Assembly.GetExecutingAssembly().GetManifestResourceNames(), resourceName) >= 0, 
        String.Join(", ", Assembly.GetExecutingAssembly().GetManifestResourceNames()));

      this.fileName = fileName;
      this.resourceName = resourceName;
    }

    public void CopyAssemblyToExecutable()
    {
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

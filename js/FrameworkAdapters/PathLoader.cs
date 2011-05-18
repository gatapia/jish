using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace js.net.FrameworkAdapters
{
  public class PathLoader
  {
    private readonly IList<string> loaded = new List<string>();

    public string LoadScriptContent(string path)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(path));
      Trace.Assert(File.Exists(path), "Could not find file: " + path);

      string key = path.ToLower();
      if (loaded.Contains(key)) return null;
      loaded.Add(key);

      return File.ReadAllText(path);
    }
  }
}
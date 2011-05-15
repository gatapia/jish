using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace js.Closure
{
  public class ClosureInterceptor
  {
    private readonly string basedir;
    private readonly PathLoader loader;

    public ClosureInterceptor(string basedir, PathLoader loader)
    {
      this.basedir = basedir;
      this.loader = loader;
    }

    public string GetScriptContentIfNotLoaded(string directory, string filename)
    {
      if (String.IsNullOrWhiteSpace(directory)) directory = basedir;
      string path = Path.Combine(directory, filename);
      return loader.LoadScriptContent(path);
    }
  }

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
using System;
using System.IO;

namespace js.net.FrameworkAdapters.Closure
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
}
using System;
using System.Diagnostics;
using System.IO;

namespace js.net.FrameworkAdapters.Closure
{
  public class ClosureWriteScriptTagImpl
  {
    private readonly string basedir;
    private readonly OneTimeFileLoader loader;

    public ClosureWriteScriptTagImpl(string basedir, OneTimeFileLoader loader)
    {
      Trace.Assert(loader != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(basedir));
      Trace.Assert(Directory.Exists(basedir));

      this.basedir = basedir;
      this.loader = loader;
    }

    public string GetScriptContentIfNotLoaded(string directory, string filename)
    {
      Trace.Assert(loader != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(filename));
      Trace.Assert(String.IsNullOrWhiteSpace(directory) || Directory.Exists(directory));

      if (String.IsNullOrWhiteSpace(directory)) directory = basedir;
      string path = Path.Combine(directory, filename);
      if (!File.Exists(path)) { throw new FileNotFoundException("Could not find JavaScript directory/file: " + directory + "/" + filename, path); }
      return loader.LoadScriptContent(path);
    }
  }
}
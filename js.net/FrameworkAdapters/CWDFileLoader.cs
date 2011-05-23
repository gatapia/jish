using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using js.net.Engine;

namespace js.net.FrameworkAdapters
{
  public class CWDFileLoader
  {
    private string currentWorkingDirectory = String.Empty;
    private readonly Stack<string> oldWorkingDirectories = new Stack<string>();
    private readonly IEngine engine;

    public CWDFileLoader(IEngine engine)
    {
      this.engine = engine;
    }

    public string LoadJSFile(string file)
    {
      return LoadJSFile(file, true); 
    }

    public string LoadJSFile(string file, bool setCwd)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      if (!String.IsNullOrWhiteSpace(currentWorkingDirectory))
      {
        file = Path.Combine(currentWorkingDirectory, file);
      }
      Trace.Assert(File.Exists(file), "Could not find file: " + file);
      FileInfo fi = new FileInfo(file);
      if (setCwd)
      {
        oldWorkingDirectories.Push(currentWorkingDirectory);
        currentWorkingDirectory = fi.Directory.FullName;
      }
      string scriptContent = File.ReadAllText(file);
      if (String.IsNullOrWhiteSpace(scriptContent))
      {
        return null;
      }

      return scriptContent;      
    }

    public void ScriptFinnished()
    {
      currentWorkingDirectory = oldWorkingDirectories.Pop();
    }
  }
}
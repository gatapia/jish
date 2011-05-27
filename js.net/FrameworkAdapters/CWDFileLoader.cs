using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace js.net.FrameworkAdapters
{
  public class CWDFileLoader
  {
    private string currentWorkingDirectory = String.Empty;

    private readonly Stack<string> oldWorkingDirectories = new Stack<string>();

    public string GetFilePathFromCwdIfRequired(string file)
    {
      return GetFilePathFromCwdIfRequired(file, true); 
    }

    public string GetFilePathFromCwdIfRequired(string file, bool setCwd)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      string currentFile = GetFilePath(file);
      Trace.Assert(File.Exists(currentFile), "Could not find file: " + currentFile);

      FileInfo fi = new FileInfo(currentFile);
      if (setCwd)
      {
        oldWorkingDirectories.Push(currentWorkingDirectory);
        currentWorkingDirectory = fi.Directory.FullName;
      }
      string scriptContent = File.ReadAllText(currentFile);
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

    public string GetFilePath(string file)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      
      if (file.Equals("cssom"))
      {
        file = new FileInfo(@"resources\dom\CSSOM\lib\index.js").FullName;
      } else if (file.Equals("node-htmlparser/lib/node-htmlparser"))
      {
        file = new FileInfo(@"resources\dom\node-htmlparser\node-htmlparser.js").FullName;
      }
      if (String.IsNullOrWhiteSpace(new FileInfo(file).Extension)) { file += ".js"; }

      string currentFile = file;
      if (!String.IsNullOrWhiteSpace(currentWorkingDirectory) && file.IndexOf(':') < 0)
      {
        currentFile = Path.Combine(currentWorkingDirectory, file);
      }
      return currentFile;
    }
  }
}
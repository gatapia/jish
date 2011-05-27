using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using js.net.Util;

namespace js.net.FrameworkAdapters
{
  public class CWDFileLoader
  {
    private EmbeddedResourcesUtils resources = new EmbeddedResourcesUtils();
    private string currentWorkingDirectory = String.Empty;

    private readonly Stack<string> oldWorkingDirectories = new Stack<string>();

    public string GetFileContentFromCwdIfRequired(string file)
    {
      return GetFileContentFromCwdIfRequired(file, true); 
    }

    public string GetFileContentFromCwdIfRequired(string file, bool setCwd)
    {      
      Trace.Assert(!String.IsNullOrWhiteSpace(file));
      string currentFile = GetFilePath(file);      

      return currentFile.StartsWith("js.net.") 
        ? GetEmbeddedResourceContentFromCwdIfRequired(currentFile, setCwd) 
        : GetFileContentFromCwdIfRequiredImpl(currentFile, setCwd);
    }    

    private string GetEmbeddedResourceContentFromCwdIfRequired(string currentFile, bool setCwd)
    {
      if (setCwd)
      {        
        oldWorkingDirectories.Push(currentWorkingDirectory);
        currentWorkingDirectory = currentFile.Substring(0, 1 + currentFile.LastIndexOf('.', currentFile.LastIndexOf('.') - 1));        
      }
      string scriptContent = resources.ReadEmbeddedResourceTextContents(currentFile, GetType().Assembly);
      return String.IsNullOrWhiteSpace(scriptContent) ? null : scriptContent;
    }

    private string GetFileContentFromCwdIfRequiredImpl(string currentFile, bool setCwd)
    {
      Trace.Assert(File.Exists(currentFile), "Could not find file: " + currentFile);

      FileInfo fi = new FileInfo(currentFile);
      if (setCwd)
      {
        oldWorkingDirectories.Push(currentWorkingDirectory);
        currentWorkingDirectory = fi.Directory.FullName;
      }
      string scriptContent = File.ReadAllText(currentFile);

      return String.IsNullOrWhiteSpace(scriptContent) ? null : scriptContent;
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
        file = "js.net.resources.dom.CSSOM.lib.index.js";
      } else if (file.Equals("node-htmlparser/lib/node-htmlparser"))
      {
        file = "js.net.resources.dom.node-htmlparser.node-htmlparser.js";
      }
      if (String.IsNullOrWhiteSpace(new FileInfo(file).Extension)) { file += ".js"; }

      string currentFile = file;
      if (!String.IsNullOrWhiteSpace(currentWorkingDirectory) && file.IndexOf(':') < 0)
      {        
        if (currentWorkingDirectory.StartsWith("js.net.")) // Is Resouce
        {
          Console.WriteLine("currentWorkingDirectory: " + currentWorkingDirectory + " file: " + file);
          string tmpCurrentWorkingDirectory = currentWorkingDirectory.Replace('.', '\\');
          file = file.Replace("../", "{BACK}").Replace("./", "").Replace('/', '\\').Replace('.', '\\').Replace("{BACK}", "..\\");
          Console.WriteLine("2 - tmpCurrentWorkingDirectory: " + tmpCurrentWorkingDirectory + " file: " + file);
          currentFile = Path.Combine(tmpCurrentWorkingDirectory, file);
          Console.WriteLine("currentFile: " + currentFile);          
          currentFile = Path.GetFullPath(currentFile).Replace(Path.GetFullPath("."), "").Replace('\\', '.').Substring(1);
          currentFile = currentFile.Replace('\\', '.');
          Console.WriteLine("done: " + currentFile);
        } else
        {
          currentFile = Path.Combine(currentWorkingDirectory, file);
        }
      }
      return currentFile;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using js.net.Util;

namespace js.net.FrameworkAdapters
{
  public class CWDFileLoader
  {
    private readonly EmbeddedResourcesUtils embeddedResourceLoader;
    private string currentWorkingDirectory = String.Empty;
    private readonly Stack<string> oldWorkingDirectories = new Stack<string>();

    public CWDFileLoader(EmbeddedResourcesUtils embeddedResourceLoader)
    {
      Trace.Assert(embeddedResourceLoader != null);

      this.embeddedResourceLoader = embeddedResourceLoader;
    }

    public string GetFileContentFromCwdIfRequired(string file)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(file));

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
      string scriptContent = embeddedResourceLoader.ReadEmbeddedResourceTextContents(currentFile, GetType().Assembly);
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
      
      file = HandleSpecialFileNames(file);      

      string currentFile = file;
      if (String.IsNullOrWhiteSpace(currentWorkingDirectory) || file.IndexOf(':') >= 0) return currentFile;
      return currentWorkingDirectory.StartsWith("js.net.") ? GetResourcePath(file) : Path.Combine(currentWorkingDirectory, file);
    }

    private string GetResourcePath(string file) { 
      string tmpCurrentWorkingDirectory = currentWorkingDirectory.Replace('.', '\\');

      // TODO: Use a regex or something to clean this up
      file = file.Replace("../", "{BACK}").Replace("./", "").Replace('/', '\\').Replace('.', '\\').Replace("{BACK}", "..\\");
      string path = Path.Combine(tmpCurrentWorkingDirectory, file);
      path = Path.GetFullPath(path).Replace(Path.GetFullPath("."), "").Replace('\\', '.').Substring(1);
      path = path.Replace('\\', '.');

      if (Array.IndexOf(GetType().Assembly.GetManifestResourceNames(), path) < 0) { throw new ApplicationException("Could not find resource: " + path); }

      return path;
    }

    private string HandleSpecialFileNames(string file) {
      if (file.Equals("cssom"))
      {
        file = "js.net.resources.dom.CSSOM.lib.index.js";
      } else if (file.Equals("node-htmlparser/lib/node-htmlparser"))
      {
        file = "js.net.resources.dom.node_htmlparser.node_htmlparser.js";
      }
      if (file.StartsWith("js.net.resources."))
      {
        currentWorkingDirectory = "js.net.resources.";
        file = file.Replace(currentWorkingDirectory, "");
      }
      if (String.IsNullOrWhiteSpace(new FileInfo(file).Extension)) { file += ".js"; }
      return file;
    }

    public void ResetCwd()
    {
      currentWorkingDirectory = String.Empty;
    }
  }
}
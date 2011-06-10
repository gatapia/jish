using System;
using System.Diagnostics;
using js.net.Engine;
using js.net.FrameworkAdapters;
using js.net.Util;

namespace js.net
{
  public class JSGlobal
  {
    private readonly CWDFileLoader fileLoader;
    private readonly JSConsole console;
    private readonly EmbeddedResourcesUtils embeddedResourceLoader;

    public JSGlobal(CWDFileLoader fileLoader, JSConsole console, EmbeddedResourcesUtils embeddedResourceLoader)
    {
      Trace.Assert(fileLoader != null);
      Trace.Assert(console != null);
      Trace.Assert(embeddedResourceLoader != null);

      this.embeddedResourceLoader = embeddedResourceLoader;
      this.console = console;
      this.fileLoader = fileLoader;
    }

    public void BindToGlobalScope(IEngine engine)
    {      
      Trace.Assert(engine != null);
      Trace.Assert(embeddedResourceLoader != null);

      engine.SetGlobal("console", console);
      engine.SetGlobal("global", this);

      engine.Run(embeddedResourceLoader.ReadEmbeddedResourceTextContents("js.net.resources.jsglobal.js"), "js.net.resources.jsglobal.js");
    }
    
    public string LoadFileContents(string file, bool setCwd)
    {            
      Trace.Assert(fileLoader != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(file));

      return fileLoader.GetFileContentFromCwdIfRequired(file, setCwd);
    }

    public string GetFilePath(string file)
    {
      Trace.Assert(fileLoader != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(file));

      return fileLoader.GetFilePath(file);
    }

    public void ScriptFinnished()
    {
      Trace.Assert(fileLoader != null);

      fileLoader.ScriptFinnished(); 
    }
  }
}
using System;
using js.net.Engine;
using js.net.FrameworkAdapters;
using js.net.Util;

namespace js.net
{
  public class JSGlobal
  {
    private readonly IEngine engine;
    private readonly CWDFileLoader fileLoader;
    private readonly JSConsole console;

    public JSGlobal(IEngine engine, CWDFileLoader fileLoader, JSConsole console)
    {
      this.engine = engine;
      this.console = console;
      this.fileLoader = fileLoader;
    }

    public void BindToGlobalScope()
    {
      EmbeddedResourcesUtils embedded = new EmbeddedResourcesUtils();  
      
      engine.SetGlobal("console", console);
      engine.SetGlobal("global", this);

      engine.Run(embedded.ReadEmbeddedResourceTextContents("js.net.resources.jsglobal.js"), "js.net.resources.jsglobal.js");
    }
    
    public string LoadFileContents(string file, bool setCwd)
    {            
      return fileLoader.GetFilePathFromCwdIfRequired(file, setCwd);
    }

    public string GetFilePath(string file)
    {
      return fileLoader.GetFilePath(file);
    }

    public void ScriptFinnished()
    {
      fileLoader.ScriptFinnished(); 
    }
  }
}
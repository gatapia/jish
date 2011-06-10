using System;
using System.Diagnostics;
using System.IO;
using js.net.Engine;

namespace js.net.FrameworkAdapters.Closure
{
  public class ClosureFrameworkAdapter : JSDomAdapter
  {
    private readonly OneTimeFileLoader oneTimeFileLoader;
    private readonly string baseJsFile;    
    private readonly string basedir;

    public ClosureFrameworkAdapter(IEngine engine, CWDFileLoader fileLoader, OneTimeFileLoader oneTimeFileLoader, JSGlobal jsGlobal, string baseJsFile) : base(engine, fileLoader, jsGlobal)
    {
      Trace.Assert(oneTimeFileLoader != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(baseJsFile));
      Trace.Assert(File.Exists(baseJsFile));
      
      this.oneTimeFileLoader = oneTimeFileLoader;      
      this.baseJsFile = baseJsFile;
      basedir = new FileInfo(baseJsFile).Directory.FullName;
    }

    public override void Initialise()
    {      
      Trace.Assert(!String.IsNullOrWhiteSpace(baseJsFile));
      Trace.Assert(File.Exists(baseJsFile));

      base.Initialise();
      
      LoadClosureBaseFileAndSetBaseDirectory();
      InterceptWriteScript();
      LoadGoogDeps();
    }

    private void LoadClosureBaseFileAndSetBaseDirectory()
    {            
      LoadJSFile(baseJsFile, false);
    }

    private void InterceptWriteScript()
    {      
      Trace.Assert(oneTimeFileLoader != null);      

      SetGlobal("writeScriptTagImpl", new ClosureWriteScriptTagImpl(basedir, oneTimeFileLoader));
      Run(
@"
goog.writeScriptTag_ = function(filename) {  
  var src = writeScriptTagImpl.GetScriptContentIfNotLoaded(null, filename);
  // null 'src' means already loaded, ignore
  if (src) { eval(src); } 
  return false;
};", "ClosureAdapterAugmenter.InterceptWriteScript");
    }

    private void LoadGoogDeps()
    {      
      Trace.Assert(!String.IsNullOrWhiteSpace(basedir));
      Trace.Assert(Directory.Exists(basedir));
      Trace.Assert(File.Exists(Path.Combine(basedir, "deps.js")));

      LoadJSFile(Path.Combine(basedir, "deps.js"), false);
    }
  }
}
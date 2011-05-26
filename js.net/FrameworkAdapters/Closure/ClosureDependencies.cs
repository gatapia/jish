using System;
using System.Diagnostics;
using System.IO;

namespace js.net.FrameworkAdapters.Closure
{
  public class ClosureDependencies
  {
    private readonly ClosureAdapter ctx;
    private readonly string baseJsFile;
    private readonly PathLoader loader;

    private string basedir;
    private ClosureInterceptor interceptor;    

    public ClosureDependencies(ClosureAdapter ctx, string baseJsFile, PathLoader loader)
    {
      Trace.Assert(ctx != null);
      Trace.Assert(loader != null);
      Trace.Assert(!String.IsNullOrWhiteSpace(baseJsFile));
      Trace.Assert(File.Exists(baseJsFile));

      this.ctx = ctx;
      this.loader = loader;
      this.baseJsFile = baseJsFile;      
    }

    public  void Initialise()
    {
      InitialiseInterceptor();
      LoadClosureBaseFile();
      InterceptWriteScript();
      LoadGoogDeps();
    }

    private void InitialiseInterceptor()
    {
      basedir = new FileInfo(baseJsFile).Directory.FullName;
      interceptor = new ClosureInterceptor(basedir, loader);
    }

    private void LoadClosureBaseFile()
    {
      ctx.LoadJSFile(baseJsFile, false);
    }

    private void InterceptWriteScript()
    {      
      ctx.SetGlobal("interceptor", interceptor);
      ctx.Run(
@"
goog.writeScriptTag_ = function(filename) {  
  var src = interceptor.GetScriptContentIfNotLoaded(null, filename);
  // null 'src' means already loaded, ignore
  if (src) { eval(src); } 
  return false;
};", "ClosureDependencies.InterceptWriteScript");
    }

    private void LoadGoogDeps()
    {      
      ctx.LoadJSFile(Path.Combine(basedir, "deps.js"), false);
    }
  }
}
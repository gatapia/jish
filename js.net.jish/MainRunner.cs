using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net.jish
{
  /// <summary>
  /// Note: This Jish is a straight clone from Node.js's REPL, cloned without
  /// premission or ceremony.  I'm sure they won't mind.
  /// 
  /// From the node.js (http://nodejs.org/docs/v0.4.8/api/repl.html) docs.
  /// 
  /// --------------------------------------------------------------------------
  /// 
  /// A Read-Eval-Print-Loop (Jish) is available both as a standalone program 
  /// and easily includable in other programs. Jish provides a way to 
  /// interactively run JavaScript and see the results. It can be used for 
  /// debugging, testing, or just trying things out.
  /// 
  /// By executing node without any arguments from the command-line you will be 
  /// dropped into the Jish. It has simplistic emacs line-editing.
  /// 
  /// ...
  /// 
  /// Inside the Jish, Control+D will exit. Multi-line expressions can be input.
  /// 
  /// The special variable _ (underscore) contains the result of the last expression.
  /// 
  /// ...
  /// There are a few special Jish commands:
  /// 
  /// .break - While inputting a multi-line expression, sometimes you get lost or just don't care about completing it. .break will start over.
  /// .clear - Resets the context object to an empty object and clears any multi-line expression.
  /// .exit - Close the I/O stream, which will cause the Jish to exit.
  /// .help - Show this list of special commands.
  /// 
  /// Running Jish with command line arguments will attempt to run the 
  /// file specified in the first argument. 
  /// </summary>
  public class MainRunner
  {
    static MainRunner()
    {                          
      // No need to include the js.net.dll, just load the embedded resource.
      CopyAssemblyToExecutable("js.net.dll", "js.net.jish.resources.js.net.dll", typeof(MainRunner).Assembly); 
    }

    // This is copied from js.net.Util.EmbeddedResourcesUtils as at this stage 
    // it is actually not available (as it lives in the js.net.dll)
    private static void CopyAssemblyToExecutable(string fileName, string resourceName, Assembly assembly = null)
    {
      Trace.Assert(!String.IsNullOrWhiteSpace(fileName));
      Trace.Assert(!String.IsNullOrWhiteSpace(resourceName));
      if (assembly == null) assembly = Assembly.GetExecutingAssembly();

      Trace.Assert(Array.IndexOf(assembly.GetManifestResourceNames(), resourceName) >= 0, 
        String.Join(", ", assembly.GetManifestResourceNames()));

      if (File.Exists(fileName))
      {
        return;
      }

      using (Stream s = assembly.GetManifestResourceStream(resourceName))
      {        
        using (FileStream fs = new FileStream(fileName, FileMode.Create))
        {
          byte[] b = new byte[s.Length];
          s.Read(b, 0, b.Length);
          fs.Write(b, 0, b.Length);
        }
      }            
    }

    [STAThread] private static void Main(string[] args)
    {
      using (IEngine engine = new JSNetEngine())
      {
        JSConsole console = new JSConsole();
        new JSGlobal(engine, new CWDFileLoader(), console).BindToGlobalScope();        

        CommandLineInterpreter cli = new CommandLineInterpreter(engine, console);
        cli.InitialiseConsole();

        var jish = new Jish(cli);
        if (args == null || args.Length == 0)
        {
          jish.StartJish();
        }
        else
        {
          jish.ExecuteArgs(args);
        }
      }
    }
  }
}
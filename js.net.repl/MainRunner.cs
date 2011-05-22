using System.Diagnostics;
using js.net.Engine;

namespace js.net.repl
{
  /// <summary>
  /// Note: This REPL is a straight clone from Node.js's REPL, cloned without
  /// premission or ceremony.  I'm sure they won't mind.
  /// 
  /// From the node.js (http://nodejs.org/docs/v0.4.8/api/repl.html) docs.
  /// 
  /// --------------------------------------------------------------------------
  /// 
  /// A Read-Eval-Print-Loop (REPL) is available both as a standalone program 
  /// and easily includable in other programs. REPL provides a way to 
  /// interactively run JavaScript and see the results. It can be used for 
  /// debugging, testing, or just trying things out.
  /// 
  /// By executing node without any arguments from the command-line you will be 
  /// dropped into the REPL. It has simplistic emacs line-editing.
  /// 
  /// ...
  /// 
  /// Inside the REPL, Control+D will exit. Multi-line expressions can be input.
  /// 
  /// The special variable _ (underscore) contains the result of the last expression.
  /// 
  /// ...
  /// There are a few special REPL commands:
  /// 
  /// .break - While inputting a multi-line expression, sometimes you get lost or just don't care about completing it. .break will start over.
  /// .clear - Resets the context object to an empty object and clears any multi-line expression.
  /// .exit - Close the I/O stream, which will cause the REPL to exit.
  /// .help - Show this list of special commands.
  /// 
  /// Running REPL with command line arguments will attempt to run the 
  /// file specified in the first argument. 
  /// </summary>
  public class MainRunner
  {
    static MainRunner()
    {            
      DefaultTraceListener def = (DefaultTraceListener) Trace.Listeners[0];
      def.AssertUiEnabled = false; // No silly dialogs
      Trace.Listeners.Clear();
      Trace.Listeners.Add(def);
    }

    private static void Main(string[] args)
    {
      using (IEngine engine = new JSNetEngine())
      {
        JSConsole console = new JSConsole();
        engine.SetGlobal("console", console);
        engine.SetGlobal("global", new JSGlobal());

        var repl = new REPL(engine, console);
        if (args == null || args.Length == 0)
        {
          repl.StartREPL();
        }
        else
        {
          repl.ExecuteArgs(args);
        }
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using js.net.Engine;

namespace js.net.NodeFacade
{
  public class NodeProcess
  {
    private IEngine engine;
    private static readonly string dllPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
    private static string currentWorkingDirectory = new FileInfo(dllPath).Directory.FullName;
    private readonly IDictionary<string, object> modules = new Dictionary<string, object>();

    internal void SetEngine(IEngine engine)
    {
      this.engine = engine;
    }

    public IEngine GetEngine() { return engine; }

    public string title = "";
    public string version = "0.1a";
    
    // Where package managers look for installed modules
    public string installPrefix = currentWorkingDirectory;
    
    // This is the absolute pathname of the executable that started the process.
    public string execPath = dllPath;
        
    public object binding(string module)
    {
      if (!modules.ContainsKey(module))
      {
        modules.Add(module, InitialiseModule(module));        
      }
      return modules[module];      
    }

    private object InitialiseModule(string module)
    {
      char[] moduleChars = module.ToCharArray();
      moduleChars[0] = Char.ToUpper(moduleChars[0]);
      module = new String(moduleChars);
      string typeName = GetType().FullName.Replace(GetType().Name, "Modules." + module);
      Type moduleType = GetType().Assembly.GetType(typeName);
      if (moduleType == null) throw new NotSupportedException("Typename " + typeName + " could not be found");
      return Activator.CreateInstance(moduleType, this);
    }

    public void chdir(string directory)
    {
      if (!Directory.Exists(directory)) throw new DirectoryNotFoundException(directory);
      currentWorkingDirectory = directory;
    }

    public string cwd()
    {
      return currentWorkingDirectory; 
    }

    public void exit(int code = 0)
    {
      engine.Dispose();
      Environment.Exit(code);
    }

    public int getgid() { throw new NotSupportedException("setgid not supported."); }
    public void setgid(object id) { throw new NotSupportedException("setgid not supported."); }
    public void getuid() { throw new NotSupportedException("setgid not supported."); }
    public void setuid(object id) { throw new NotSupportedException("setgid not supported."); }

    public void kill(int pid, string signal = "ignored")
    {
      Process.GetProcessById(pid).Kill();
    }

    public int pid = Process.GetCurrentProcess().Id;

    public string platform = Environment.OSVersion.Platform.ToString();

    public object memoryUsage()
    {      
      // TODO: heap values should come from V8
      return new
               {
                 rss = Process.GetCurrentProcess().PrivateMemorySize64,
                 vsize = Process.GetCurrentProcess().VirtualMemorySize64,
                 heapTotal = Process.GetCurrentProcess().VirtualMemorySize64,
                 heapUsed = Process.GetCurrentProcess().PrivateMemorySize64
               };
    }

    public string umask(string mask = null) { throw new NotSupportedException("umask not supported.");  }

    // TODO:
    // - procees is also an EventEmitter and events exit, uncaughtException 
    // must be fired
    // - SigEvents, like SIGINT, SIGUSR1
    // - process stdout, stderr are WritableStream
    // - process.stdin = readable stream
    // - process.argv = The args array
    // - process.env = The user environment
    // -     
  }
}
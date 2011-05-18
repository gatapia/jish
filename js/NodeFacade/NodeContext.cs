using System.IO;
using js.net.Engine;

namespace js.net.NodeFacade
{
  public class NodeContext
  {
    private const string NODE_LIB_DIR = @"C:\dev\projects\labs\js.net\lib\Nodelib\";    

    private readonly IEngine engine;
    private readonly string workingDirectory;

    public NodeContext(IEngine engine, string workingDirectory)
    {
      this.engine = engine;
      this.workingDirectory = workingDirectory;
    }

    public void Initialise()
    {
      InitialiseGlobalObjects();
      // InitialiseGlobalFunctions();      
      LoadInitialNodeCore(new NodeProcess());
    }    

    private void InitialiseGlobalObjects()
    {
      engine.SetGlobal("console", new JSConsole());
      // engine.SetGlobal("exports", new Exports());
    }

    private void InitialiseGlobalFunctions()
    {
      engine.SetGlobal("globalFunctions", new NodeGlobalFunctions(workingDirectory));
      engine.Run(
@"
var require = function(name) { 
  return eval(globalFunctions.Require(name)); 
};

");
    }

    private void LoadInitialNodeCore(NodeProcess process)
    {
      engine.SetGlobal("__process", process);
      process.SetEngine(engine);
      engine.Run("var f = " + File.ReadAllText(NODE_LIB_DIR + @"Node.js") + "; f(__process);");
    }

    public IEngine GetEngine()
    {
      return engine;
    }
  }
}

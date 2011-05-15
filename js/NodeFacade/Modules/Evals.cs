using System;

namespace js.NodeFacade.Modules
{
  public class Evals : AbstractNodeModule
  {
    public Evals(NodeProcess process) : base(process) {}

    private NodeScript script;
    public NodeScript NodeScript
    {
      get { return script ?? (script = new NodeScript(process));  }
    }
  }

  public class NodeScript : AbstractNodeModule
  {
    public NodeScript(NodeProcess process) : base(process) {}

    public object runInThisContext(string script, object mockFileNameOrSandbox = null, string mockFileName = null)
    {
      return process.GetEngine().Run(script);
    }
  }
}
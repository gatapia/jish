namespace js.net.NodeFacade.Modules
{
  public class Natives : AbstractNodeModule
  {
    public Natives(NodeProcess process) : base(process) {}

    private NodeBuffer nodeBuffer;
    public NodeBuffer buffer { 
      get
      {
        return nodeBuffer ?? (nodeBuffer = new NodeBuffer());
      }
    }
  }

  public class NodeBuffer
  {
  }
}
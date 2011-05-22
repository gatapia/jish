namespace js.net.NodeFacade.Modules
{
  public class Buffer : AbstractNodeModule
  {
    public Buffer(NodeProcess process) : base(process) {}

    private SlowBuffer slowBuffer;
    public SlowBuffer SlowBuffer { 
      get
      {
        return slowBuffer ?? (slowBuffer = new SlowBuffer());
      }
    }
  }

  public class SlowBuffer
  {
  }
}
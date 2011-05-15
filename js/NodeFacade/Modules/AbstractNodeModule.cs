namespace js.NodeFacade.Modules
{
  public abstract class AbstractNodeModule
  {
    protected readonly NodeProcess process;

    protected AbstractNodeModule(NodeProcess process)
    {
      this.process = process;
    }
  }
}
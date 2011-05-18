namespace js.net.Engine
{
  public class DefaultEngineFactory : IEngineFactory
  {
    public IEngine CreateEngine()
    {
      return new JSNetEngine();
    }
  }
}

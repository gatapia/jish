using js.net.Engine;
using js.net.FrameworkAdapters.Closure;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestAdapterFactory : ITestAdapterFactory
  {
    private readonly string baseJsFile;
    private readonly IEngineFactory engineFactory;

    public ClosureTestAdapterFactory(string baseJsFile, IEngineFactory engineFactory)
    {
      this.baseJsFile = baseJsFile;
      this.engineFactory = engineFactory;
    }

    public bool Silent { get; set; }

    public ITestAdapter CreateAdapter()
    {
      ClosureAdapter closure = new ClosureAdapter(baseJsFile, engineFactory.CreateEngine());
      closure.Initialise();
      ClosureTestAdapter adapter = new ClosureTestAdapter(closure) { Silent = Silent };
      return adapter;
    }
  }
}

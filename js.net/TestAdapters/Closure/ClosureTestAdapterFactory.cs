using js.net.Engine;
using js.net.FrameworkAdapters.Closure;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestAdapterFactory : AbstractTestAdapterFactory
  {
    public ClosureTestAdapterFactory(string baseJsFile, IEngineFactory engineFactory) : base(baseJsFile, engineFactory) {}

    protected override ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile)
    {
      ClosureAdapter closure = new ClosureAdapter(frameworkJsFile, engine);
      closure.Initialise();
      return new ClosureTestAdapter(closure);
    }
  }
}

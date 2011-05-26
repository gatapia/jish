using js.net.Engine;
using js.net.FrameworkAdapters.Closure;

namespace js.net.TestAdapters.Closure
{
  public class ClosureTestAdapterFactory : AbstractTestAdapterFactory
  {
    private readonly string jsDomSourceFile;
    public ClosureTestAdapterFactory(string baseJsFile, string jsDomSourceFile, IEngineFactory engineFactory) : base(baseJsFile, engineFactory)
    {
      this.jsDomSourceFile = jsDomSourceFile;
    }

    protected override ITestAdapter CreateTestAdapter(IEngine engine, string frameworkJsFile)
    {
      ClosureAdapter closure = new ClosureAdapter(frameworkJsFile, jsDomSourceFile, engine);
      closure.Initialise();
      return new ClosureTestAdapter(closure);
    }
  }
}

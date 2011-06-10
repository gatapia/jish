namespace js.net.TestAdapters.Coverage
{
  public interface IFileCoverageResults
  {
    string FileName { get; }
    int Statements { get; }
    int Executed { get; }
    decimal CoveragePercentage { get; }
  }
}
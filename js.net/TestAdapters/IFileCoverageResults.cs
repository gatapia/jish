namespace js.net.TestAdapters
{
  public interface IFileCoverageResults
  {
    string FileName { get; }
    int Statements { get; }
    int Executed { get; }
    decimal CoveragePercentage { get; }
  }
}
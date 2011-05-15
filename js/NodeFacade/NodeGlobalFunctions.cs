using System;
using System.IO;

namespace js.NodeFacade
{
  public class NodeGlobalFunctions
  {
    private readonly string baseDir;

    public NodeGlobalFunctions(string baseDir)
    {
      this.baseDir = baseDir;
    }

    public object Require(string name)
    {
      bool isCoreLib = name.IndexOfAny(new[] {'.', '/'}) < 0;
      return isCoreLib ? ReadCoreFileContents(name) : ReadFileContents(name);
    }

    private object ReadCoreFileContents(string name)
    {
      throw new NotImplementedException();
    }

    private string ReadFileContents(string name)
    {
      return File.ReadAllText(Path.Combine(baseDir, name) + ".js");
    }
  }
}
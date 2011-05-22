using System.IO;
using js.net.Engine;

namespace js.net
{
  public class JSGlobal
  {
    private readonly IEngine engine;

    public JSGlobal(IEngine engine)
    {
      this.engine = engine;            
    }

    public void BindToGlobalScope()
    {
      engine.SetGlobal("global", this);        
      engine.Run(
@"
global.require = function(file) {
  return eval(global.LoadFileContents(file));
};

for (var i in global) {
  console.log('setting this: ' + i);
  this[i] = global[i];
};
");
    }

    public string LoadFileContents(string file)
    {
      return File.ReadAllText(file);
    }
  }
}
using System;
using System.IO;
using js.net.Engine;
using js.net.FrameworkAdapters;

namespace js.net
{
  public class JSGlobal
  {
    private readonly IEngine engine;
    private readonly CWDFileLoader fileLoader;

    public JSGlobal(IEngine engine, CWDFileLoader fileLoader)
    {
      this.engine = engine;
      this.fileLoader = fileLoader;
    }

    public void BindToGlobalScope()
    {
      engine.SetGlobal("global", this);        
      engine.Run(
@"
global.__dirname = '.';
global.process = {
  platform: 'js.net',
  version: '0.1',
  binding: function(name) {
    if (name !== 'evals') throw new Error('Not supported: ' + name);

    return {
      NodeScript: {
        runInContext: eval
      },
      Context: {}
    };
  }
};

global.exports = {};

global.require = function(file) {
  switch (file) {
    case 'sys': return { puts: console.log };
    case 'url': return { parse: function(href) { return href; } };
    case 'path': return { dirname: function(url) { return url; } };
    case 'fs': return { 
      readFileSync: function(file) { return global.LoadFileContents(file, false); },
      readFile: function(file, callback) { callback(global.LoadFileContents(file, false)); } 
    };
    case 'request':
    case 'http':
    case 'https': return { request: function(options) { throw new Error('request/http(s).request not supported.'); } };
  }

  var exports = {};
  eval(global.LoadFileContents(file, true));   
  global.ScriptFinnished();
  return exports;
};

for (var i in global) {
  this[i] = global[i];
};
");
    }
    
    public string LoadFileContents(string file, bool setCwd)
    {      
      if (String.IsNullOrWhiteSpace(new FileInfo(file).Extension)) { file += ".js"; }
      return fileLoader.LoadJSFile(file, setCwd);
    }

    public void ScriptFinnished()
    {
      fileLoader.ScriptFinnished(); 
    }
  }
}
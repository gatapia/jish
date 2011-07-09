
global.setTimeout = function(func, timeOut) {
  func();
  return 0;
};

global.setInterval = global.clearTimeout = global.clearInterval = function() {};

global.__filename  = global.__dirname = '.';

if (typeof(global.navigator) === 'undefined') {
  global.navigator = {
    userAgent: 'js.net'
  };
}

global.module = {
  parent: {
    parent: {}
  }
};

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
  if (global.exports[file]) return global.exports[file];

  var exports = mockNodeRequires(file);  
  if (exports) { 
    return global.exports[file] = exports; 
  } 

  var id = global.GetFilePath(file);
  if (global.exports[id]) {
    return global.exports[id];
  }
  exports = {};
  global.exports[id] = exports;
  eval(global.LoadFileContents(file, true));     
  global.ScriptFinnished();
  return exports;
};

function mockNodeRequires(module) {
  switch (module) {
    case 'sys': return { puts: console.log };
    case 'url': return { parse: function(href) { return href; } };
    case 'path': return { dirname: function(url) { return url; } };
    case 'fs': return { 
      readFileSync: function(file) { return global.LoadFileContents(file, false); },
      readFile: function(file, callback) { callback(global.LoadFileContents(file, false)); } 
    };
    case 'assert': return { equal: function(a, b) { if (a != b) throw new Error('Assertion Failed'); } };
    case 'request':
    case 'http':
    case 'https': return { request: function(options) { throw new Error('request/http(s).request not supported.'); } };    
  }
  return null;
};


for (var i in global) {
  if (typeof(this[i]) === 'undefined') {
    this[i] = global[i];
  }
};
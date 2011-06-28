// Namespace global.jish.internal
var global = this;
global.jish = {};
global.jish.internal = {};

////////////////////////////////////////////////////////////////////////////////
// jish.assembly
////////////////////////////////////////////////////////////////////////////////

// Loads the specified assembly and places all loaded IInlineCommands in the 
// global context.
global.jish.assembly = function(assemblyName) {
  var namespaceCommands = jish.loadAssemblyImpl(assemblyName);  
  global.jish.internal.importNamespaceCommands(namespaceCommands);
};

global.jish.internal.importNamespaceCommands = function(namespaceCommands) {
  for (var namespace in namespaceCommands) {            
    var command = namespaceCommands[namespace];        
    if (global[namespace]) { throw new Error('Namespace "' + namespace + '" is already registered'); }
    global[namespace] = command;
  }
};

////////////////////////////////////////////////////////////////////////////////
// jish.closure
////////////////////////////////////////////////////////////////////////////////
global.jish.closure = function(baseJsPath) {
  eval(global.jish.internal.getFileContents(baseJsPath));  
  goog.global = global;
  global.goog = goog;
  global.goog.writeScriptTag_ = function(filename) {    
    filename = filename.replace(/\//g, '\\');
    var idx = baseJsPath.lastIndexOf('\\');
    var dir = idx > 0 ? baseJsPath.substring(0, idx) : '.';
    eval(global.jish.internal.getFileContents(dir + '\\' + filename));
    return false;
  };
  eval(global.jish.internal.getFileContents(baseJsPath.replace('base.js', 'deps.js')));  
};

global.jish.internal.alreadyLoaded = {};

global.jish.internal.getFileContents = function (file) {
  if (!file) throw new Error('file is required');
  
  if (global.jish.internal.alreadyLoaded[file]) return;
  global.jish.internal.alreadyLoaded[file] = 1;

  var f = jish.create('System.IO.File');
  if (!f.Exists(file)) throw new Error('file: ' + file + ' could not be found');
  return f.ReadAllText(file);  
};


////////////////////////////////////////////////////////////////////////////////
// jish.delegates
////////////////////////////////////////////////////////////////////////////////

// Used when calling functions in .Net that have delegates (callbacks)
global.jish.internal.realDelegates = {};
global.jish.internal.callRealDelegate = function(delegateName) {
  var realArgs = arguments.slice(1, arguments.length - 1);
  global.jish.internal.realDelegates[delegateName].apply(global, realArgs);
};

// Args should somehow be dynamic
global.jish.internal.generateJSProxyFunction = 
    function(csProxyFunctionName, arg1, callback) {
  
  var cbid = global.jish.internal.getUniqueCallbackId();
  global.jish.internal.realDelegates[cbid] = callback;

  var steps = csProxyFunctionName.split('.');
  
  var currentNode = global;  
  for (var i = 0; i < steps.length; i++) {    
    currentNode = currentNode[steps[i]];
  };
  
  currentNode.apply(global, arg1, cbid); // Args should somehow be dynamic
};

global.jish.internal.getUniqueCallbackId = function() {
  return new Date().getTime().toString();
};
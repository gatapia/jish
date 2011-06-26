// Namespace global.jish.internal
var global = this;
global.jish = {};
global.jish.internal = {};

// Loads the specified assembly and places all loaded IInlineCommands in the 
// global context.
global.jish.assembly = function(assemblyName) {
  var commands = jish.loadAssemblyImpl(assemblyName);  
  global.jish.internal.importCommands(commands);
};

global.jish.internal.importCommands = function(commands) {
  for (var nsAndFnName in commands) {    
    var split = nsAndFnName.split('.');      
    if (!global[split[0]]) {
      global[split[0]] = {};
    }
    global[split[0]][split[1]] = commands[nsAndFnName][split[1]];
  }
};

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
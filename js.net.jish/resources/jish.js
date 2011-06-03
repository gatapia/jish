var global = this;

__initialise();

function __initialise() {
};

global.jish = {};

// Loads the specified assembly and places all loaded IInlineCommands in the 
// global context.
jish.assembly = function(assemblyName) {
  var commands = jish.loadAssemblyImpl(assemblyName);
  for (var nsAndFnName in commands) {
    var split = nsAndFnName.split('.');
    if (!global[split[0]]) global[split[0]] = {};
    global[split[0]][split[1]] = commands[nsAndFnName][split[1]];
  }
};
var help = function() {
  var help = 
['jish.clear()                    Cancels the execution of a multi-line command.',
 'jish.exit()                     Exit Jish.', 
 'jish.assembly(<file>)           Loads a .Net assembly in preparation for jish.static / jish.create calls.',
 'jish.static(<class>)            Loads all static members of a .Net utility class and makes them available to you in Jish.',
 '',];
  console.log(help.join('\n'));
};

var start = function() {
  console.log('Welcome to Jish.  Press help() for more options.');
};

// start();
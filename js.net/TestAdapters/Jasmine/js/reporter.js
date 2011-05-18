var results = {};

var reporter = {
  createDom: function() {},
  reportRunnerStarting: function() {},
  reportRunnerResults: function(runnerResults) {
    var specs = runnerResults.specs();    
    for (var i = 0; i < specs.length; i++) {
      var s = specs[i];
      results[s.description] = s.results_.failedCount;      
    }
  },  
  reportSuiteResults:function() {},
  reportSpecStarting: function() {},
  reportSpecResults: function() {},
  log: function() { console.log(arguments); },
  getLocation: function() { return null; },
  specFilter: function() { return true; }
};
jasmine.getEnv().addReporter(reporter);
jasmine.getEnv().execute();

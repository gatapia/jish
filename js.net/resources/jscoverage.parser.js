// This code is unceremoniously ripped out of JSCoverage's jscoverage.js file
// which unfortunatelly is so tied to its own dom that it cannot be reused.

var coverageResults = {
  files: [],
  totalStatements: 0,
  totalExecuted: 0
};

function JSCoverage() {
  this.cc = _$jscoverage;  
  if (!this.cc) results = 'No coverage information found.';  
}

JSCoverage.prototype.run = function() {
  if (!this.cc) return results = 'No coverage information found.';  
  
  var file;
  var files = [];
  for (file in this.cc) {
    if (! this.cc.hasOwnProperty(file)) {
      continue;
    }

    files.push(file);
  }
  files.sort();

  var rowCounter = 0;
  for (var f = 0; f < files.length; f++) {
    file = files[f];
    var lineNumber;
    var num_statements = 0;
    var num_executed = 0;
    var fileCC = this.cc[file];
    var length = fileCC.length;
    var currentConditionalEnd = 0;
    var conditionals = null;
    if (fileCC.conditionals) {
      conditionals = fileCC.conditionals;
    }
    for (lineNumber = 0; lineNumber < length; lineNumber++) {
      var n = fileCC[lineNumber];

      if (lineNumber === currentConditionalEnd) {
        currentConditionalEnd = 0;
      }
      else if (currentConditionalEnd === 0 && conditionals && conditionals[lineNumber]) {
        currentConditionalEnd = conditionals[lineNumber];
      }

      if (currentConditionalEnd !== 0) {
        continue;
      }

      if (n === undefined || n === null) {
        continue;
      }

      if (n > 0) {
        num_executed++;
      }
      num_statements++;
    }

    var percentage = ( num_statements === 0 ? 0 : parseInt(100 * num_executed / num_statements) );
    coverageResults.files.push({fileName: file, statements: num_statements, executed: num_executed});
       
    coverageResults.totalStatements += num_statements;
    coverageResults.totalExecuted += num_executed;    
  }
};

new JSCoverage().run();
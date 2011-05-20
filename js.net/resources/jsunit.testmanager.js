var results = {};

(function JSNetTestManager() {
  var tests = getTests();
  runTests(tests);
})();

function getTests() {
  var tests = [];
  for (var i in this) {
    if (i.indexOf('test') === 0) {
      tests.push(i);
    }
  };
  return tests;
};

function runTests(tests) {
  if ('setUpPage' in this) setUpPage();
  for (var i = 0; i < tests.length; i++) {
    var test = tests[i];
    results[test] = null;
    if ('setUp' in this) setUp();
    try { 
      this[test](); 
    } catch (ex) {
      results[test] = ex;
    };

    if ('tearDown' in this) tearDown();
  }
  if ('tearDownPage' in this) tearDownPage();
};
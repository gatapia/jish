goog.require('jsnet.jscoverage.source');

goog.require('goog.testing.jsunit');


function testAdd() {
  assertEquals(5, jsnet.jscoverage.source.add(2, 3));
};

function testSubtract() {
  assertEquals(-1, jsnet.jscoverage.source.subtract(2, 3));
};

function testMultiply() {
  assertEquals(6, jsnet.jscoverage.source.multiply(2, 3));
};

function testDivide() {
  assertEquals(2, jsnet.jscoverage.source.divide(4, 2));
};
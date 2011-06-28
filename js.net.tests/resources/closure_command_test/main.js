
goog.provide('jish.test.main');

goog.require('jish.test.util');

/**
 * @constructor
 */
jish.test.main = function() {
  this.util = new jish.test.util();  
};

jish.test.main.prototype.util;

jish.test.main.prototype.callUtilMethod = function() {
  return this.util.testMethod();
};
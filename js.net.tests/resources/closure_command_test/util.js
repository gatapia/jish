
goog.provide('jish.test.util');

goog.require('goog.string');

/**
 * @constructor
 */
jish.test.util = function() {  
};


jish.test.util.prototype.testMethod = function() {
  return goog.string.buildString('hello', ' ', 'world');
};
//needed to get around 
(function (noop) {
    setInterval = noop;
    clearInterval = noop;
    setTimeout = function (callback, delay) {
        callback()
    }
    clearTimeout = noop;
})(function () { })


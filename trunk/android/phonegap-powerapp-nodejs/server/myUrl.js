// Server URL definition

var rawUrl = '10.136.238.174';
var url = 'http://' + rawUrl;

var port = 8012;

exports.getRawUrl = function() {
    return rawUrl;
};

exports.getUrl = function() {
    return url;
};

exports.getPort = function() {
    return port;
};


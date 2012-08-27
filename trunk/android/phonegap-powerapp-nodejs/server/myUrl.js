// Server URL definition

var rawUrl = 'a.huiq.org';
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


phantom.outputEncoding = "gbk";
var page = require('webpage').create(),
  system = require('system'),
  t, address;
if (system.args.length === 1) {
    console.log('Usage: loadspeed.js <some URL>');
    phantom.exit();
}
address = system.args[1];

page.open(address, function (status) {
    if (status !== 'success') {
        console.log('页面请求失败' + status);
    } else {
        var result = page.evaluate(function () {
            return "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head>" + document.head.innerHTML + "</head><body>" + document.body.innerHTML + "<input type='hidden' name='pagecookies' value='" + document.cookie + "'></body></html>";
        });
        console.log(result);

    }
    phantom.exit();
});
//page.onError = function (msg, trace) {

//    var msgStack = ['ERROR: ' + msg];

//    if (trace && trace.length) {
//        msgStack.push('TRACE:');
//        trace.forEach(function (t) {
//            msgStack.push(' -> ' + t.file + ': ' + t.line + (t.function ? ' (in function "' + t.function + '")' : ''));
//        });
//    }

//    console.error(msgStack.join('\n'));

//};
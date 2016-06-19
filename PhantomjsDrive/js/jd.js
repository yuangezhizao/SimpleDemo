phantom.outputEncoding = "gbk";
var page = require('webpage').create(),
    url = 'http://www.fengqu.com/other/search.html?page=2&categoryIds=226&catg2ndIds=10015&_spm=0.sear13.0.1#nav';

page.open(url, function (status) {
    if (status !== 'success') {
        console.log('Unable to access network');
    } else {
        var result = page.evaluate(function () {
            var list = [];
            $(".product-1-list li").each(function (a, b) {
                list.push($(b).html());
            });

            return list;
            
        });
        //console.log(results.join('\n'));
        console.log(result.join('\n'));
    }
    phantom.exit();
});

//page.open(url, function (status) {
//    if (status === "success") {
//        page.includeJs("http://libs.baidu.com/jquery/1.9.0/jquery.js", function () {
//            var list = [];
//          var results=  page.evaluate(function () {
//              // $("#plist li .p-price").each(function(a, b) {
//              //      list.push($(b).attr("data-sku"));

//              //  });
//              //return list;
//              console.log("$(\"#plist\").text() -> " + $("#logo-2013").html());
//          });
//          //console.log(results.join('\n'));
//            phantom.exit();
//        });
//    }
//});
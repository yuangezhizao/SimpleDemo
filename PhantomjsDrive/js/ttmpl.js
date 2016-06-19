var page1 = require('webpage'),
    system = require('system'),
    t, address,page=null;
phantom.outputEncoding="gbk";
    var t = 10;
    var line=0;
    var mbl=false,sbl=false;
    var state=0;//state:0:读取状态，1已读取的状态，正常的读取了数据，2：完成读取网页成功，3，网页加载超时，4 网络问题，没有正确的读入网页，0：不确定
    var readdata="",txt="",level=0,returl="",rettitle="",retotherdata="",action=0;
    var timecount=0;
    var Func,func,wFunc,wfunc;

    var wurl,functxt,wfunctxt,othertxt;
    var  interval,intervalout ,intervalin;
    ///test
    var itest=0;    
    var waittime=200,waittimeout=5,wtout,pageouttime=3000;//waittime 定时时间，waittimeout：超出时间重新读数据，pageout ：等待网页就续时间
console.log("1");
 wtout=((1000/waittime) * 60 *5) |0;
intervalout = setInterval(function(){
	//line++;
   // system.stdout.writeLine('ddddd '+line);
   mbl=false;
   if(state==0){
   		readdata=  system.stdin.readLine();   
	   if(readdata!=""){
	   		state=1;
	   		if(jxdata(readdata)==true){
		   			timecount=0;
		   			mbl=true;
		   			returl="";
		   			rettitle="";
		   			txt="";
		   			retotherdata="";
	   		}
	   		else{
	   			state=0;	
	   		}
	   }
	}

   if(mbl==true){
   		txt="";   		
   		mbl=false;	
   		//console.log(functxt);
   	//	console.log(wfunctxt);
		Func=new Function(functxt);  
		func=new Func();
		wFunc=new Function(wfunctxt);
		wfunc=new wFunc();	 
   		//console.log("action:"+action);		
			//console.log("-------action:"+action);	
	    if(action==0){
	    	if(page!=null)
				page.close();
			//console.log("----page.close();------------------------------------------------");			
	   		page=page1.create();
		    page.onLoadFinished=loadend;
		}
		switch(action)
		{
			case 0:
				if(wurl!=""){ 
		   			wurl=wfunc.GetWUrl(wurl); 
				  	//console.log("processdata();");				   			
		   			processdata();
		   			//wurl="";
	   			}
			break;
			case 1:

				if(func.Startaction(page)==""){
					state=4;
				}
				  
			break;	
		}
	}else
	{
		timecount++;
		if(timecount== wtout)//wtout没有完成重读数据
		{	
			state=4;
		}
	}

}, waittime);

intervalin = setInterval(function(){
	//line++;
	//	console.log("outdata:"+state);	
	if(state>1){
		//console.log("outdata,state:"+state);
		if(txt!==""){
			seeddata();
	    	system.stdout.writeLine(txt);
	    }
	    state=0;	    
	    sbl=false;
	}
}, waittime);

console.log("3");

function jxdata(txt){

	var txth=txt.substring(0,2);
	var ih,jh,nh,ii=4;
	var str="";
	var bl=false;
	wfunctxt="";
	functxt="";
	if(txth=="Z@"){
			level=txt.substring(2,3) |0;
			action=txt.substring(3,4) |0;
			bl=true;
	    ih=txt.substring(ii,ii+4) |0;
	    ii+=4;
	    jh=txt.substring(ii,ii+4) |0;
	    ii+=4;            
	    nh=txt.substring(ii,ii+4) |0;
	    ii+=4;
	    wurl=txt.substring(ii,ii+ih) ;
	    	//console.log("--------------------wurl--------------------------------:"+wurl);
	    ii+=ih;
		//	console.log("ih:"+ih+";jh:"+jh+";nh:"+nh+";wrul:"+wurl);
			console.log("url:"+wurl);			
			if(ii<txt.length){   			
	     	functxt=txt.substring(ii,ii+jh);
	        ii+=jh;
	        if(ii<txt.length){    	
	        	wfunctxt=txt.substring(ii,ii+nh);
	        	ii+=nh;
	    	}
		//	console.log("--"+functxt);	 
			//console.log("--"+wfunctxt);	 			   	
		}
	    if(ii<txt.length)
	          othertxt=txt.substr(ii);

 /* */
      }
      return bl;
}

function retstr(v,i){
	var txt="000000"+v;
	return txt.substr(txt.length-i);

}

//返回数据结构：Z!为标记，S表示成功F失败，00:层次由读入数据带入，0000：URL的长度，0000：类别长度，0000：数据长度
function seeddata()
{
	var i=returl.length |0;
	var j=rettitle.length |0;
	var n=retotherdata.length |0;

	switch(state){
		case 0:
			state=0;
		break;
		case 2:
			if(txt!=""){
				txt="Z!S"+retstr(level,1)+retstr(action,1)+retstr(i,4)+retstr(j,4)+retstr(n,4)+returl+rettitle+retotherdata+txt;

		}
		break;
		case 3:
		case 4:
			i=wurl.length |0;
			txt="Z!F"+retstr(level,1)+retstr(action,1)+retstr(i,4)+retstr(j,4)+retstr(n,4)+wurl+rettitle+retotherdata+txt;
			
		break;
	};

	//console.log("-----"+txt)
}


//Z@011500000000https://chaoshi.tmall.com/?spm=3.7396704.20000005.d5.B9DZPr&abbucket=&acm=tt-1138874-37187.1003.8.74460&aldid=74460
//Z@001300000000http://jd.com
//var iizzjj=0
/**/

 var loadend=function(status){
		   
	    if (status !== "success") {
	       state=4;
	       console.log("无法打开网页:"+wurl);
	    } else {
	        // Wait for 'signin-dropdown' to be visible
	        waitFor(function() {
	            // Check in the page if a specific element is now visible
	           // return false;
	           var dd=func.Getmdata(page);
	          // console.log("dd=func.Getmdata():"+dd)
	           return dd;
	        }, function() {
	        	console.log("打开网页");
	        	txt=func.Start(page);	       	    
	        	returl=func.Getmurl(page);
	        	//rettitle=func.Getmtitle(page);
	        	retotherdata=func.GetOther(page);
	          //  page.render("front-Thinking"+iizzjj+".png"); 
	          //  iizzjj++;	        	
	          // console.log("txt:234567666");
	           //phantom.exit();
	        },pageouttime);        
	    }
}


function processdata(){	
	   // console.log("status:");

	page.open(wurl, function (status) {

/* if (status !== "success") {
	       state=4;
	       console.log("无法打开网页"+wurl);
	    } else {
	        // Wait for 'signin-dropdown' to be visible
	        waitFor(function() {
	            // Check in the page if a specific element is now visible
	           // return false;
	           var dd=func.Getmdata(page);
	           console.log("dd=func.Getmdata():"+dd)
	           return dd;
	        }, function() {
	        	console.log("打开网页open");
	        	txt=func.Start(page);	       	    
	        	returl=func.Getmurl(page);
	        	//rettitle=func.Getmtitle(page);
	        	retotherdata=func.GetOther(page);
	          //  page.render("front-Thinking"+iizzjj+".png"); 
	          //  iizzjj++;	        	
	          // console.log("txt:234567666");
	           //phantom.exit();
	        },pageouttime);        
	    }
*/

	});
}

console.log("5");

function waitFor(testFx, onReady, timeOutMillis) {
    var maxtimeOutMillis = timeOutMillis ? timeOutMillis : 3000, //< Default Max Timout is 3s
        start = new Date().getTime(),
        condition = false,
        interval = setInterval(function() {
        	itest++;
            if ( (new Date().getTime() - start < maxtimeOutMillis) && !condition ) {
                // If not time-out yet and condition not yet fulfilled
                condition = (typeof(testFx) === "string" ? eval(testFx) : testFx()); //< defensive code
            } else {
                if(!condition) {
                	state=3;
                	console.log("超时"+state);
                	txt="  ";
                    clearInterval(interval);
                } else {
                	state=2;
                    typeof(onReady) === "string" ? eval(onReady) : onReady(); //< Do what it's supposed to do once the condition is fulfilled
                    clearInterval(interval); //< Stop this interval
                }
            }
        }, 250); //< repeat check every 250ms
};


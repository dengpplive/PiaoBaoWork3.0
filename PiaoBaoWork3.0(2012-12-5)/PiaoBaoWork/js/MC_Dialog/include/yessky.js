function gById(id) { return document.getElementById(id);}
function CreateXmlHttp() {
  var request;
  var browser = navigator.appName;
  if(browser == "Microsoft Internet Explorer") {
    var arrVersions = ["Microsoft.XMLHttp", "MSXML2.XMLHttp.4.0","MSXML2.XMLHttp.3.0", "MSXML2.XMLHttp","MSXML2.XMLHttp.5.0"];
    for (var i=0; i < arrVersions.length; i++) {
      try {
        request = new ActiveXObject(arrVersions[i]); 
        return request;
      } 
      catch(exception){
        //ԣ
	  }
    }
  }
  else{
    request = new XMLHttpRequest(); 
    return request;
  }   
}
//get cookie
function get_cookie(Name) { 
  var search = Name + "=" 
  var returnvalue = ""; 
  if (document.cookie.length > 0) { 
    offset = document.cookie.indexOf(search); 
    if (offset != -1) {                            // ָcookieѾ
      offset += search.length                     // ָcookieֵλ               
      end = document.cookie.indexOf(";", offset); // жǷ񻹰cookieֵ
      if (end == -1)                              //
        end = document.cookie.length;            //ȡcookieĳ
      returnvalue=unescape(document.cookie.substring(offset, end)) //ȡcookieֵ
    } 
  }   
  return returnvalue; 
}
//ȥո
function Trim(str)  {
  var i = 0;
  var len = str.length;
  if ( str == "" ) return( str );
  j = len -1;
  flagbegin = true;
  flagend = true;
  while ( flagbegin == true && i< len){
    if ( str.charAt(i) == " " ){
      i=i+1;
      flagbegin=true;
    }
    else{
      flagbegin=false;
    }
  }
  while  (flagend== true && j>=0){
    if (str.charAt(j)==" "){
      j=j-1;
      flagend=true;
    }
    else{
      flagend=false;
    }
  }
  if ( i > j ) return ("")
    trimstr = str.substring(i,j+1);
  return trimstr;
}
function AddEventListener(obj,act,func) {
  if(document.all){
    obj.attachEvent("on"+act,func);
  } else{
    obj.addEventListener(act,func,false);
  }
}
//array·
Array.prototype.remove = function(s){
  for(var i=0;i<this.length;i++){
    if(s == this[i]){
    	this.splice(i, 1);
    }
  }
}
var page={
tar:window,
load:function(src){
    if(src!=null && src!=""){
	  var css=(src.indexOf(".css")==-1)?false:true;
	  var js=(src.indexOf(".js")==-1)?false:true;
      if(css){
        this.tar.document.write("<link rel='stylesheet' type='text/css' rev='stylesheet' href='"+src+"' />");
      }else if(js){
        this.tar.document.write("<script language='javascript' type='text/javascript' src='"+src+"'></script>");
      } else {
      alert("unvalidate file extension!");
   }
    }
},
next:function(){this.tar.history.forward()},
back:function(){this.tar.history.back()},
init:function(f){if(typeof f=="function")f();},
onload:function(f){
    if(typeof f=="function") {
	  AddEventListener(this.tar,"load",f);
    }
  },
resize:function(f) {
     if(typeof f=="function") {
	   AddEventListener(this.tar,"resize",f);
	 }
  }
}
/*firefox rewrite event*/
function __firefox(){
    HTMLElement.prototype.__defineGetter__("runtimeStyle", __element_style);
    window.constructor.prototype.__defineGetter__("event", __window_event);
    Event.prototype.__defineGetter__("srcElement", __event_srcElement);
}
function __element_style(){
    return this.style;
}
function __window_event(){
    return __window_event_constructor();
}
function __event_srcElement(){
    return this.target;
}
function __window_event_constructor(){
    if(document.all){
        return window.event;
    }
    var _caller = __window_event_constructor.caller;
    while(_caller!=null){
        var _argument = _caller.arguments[0];
        if(_argument){
            var _temp = _argument.constructor;
            if(_temp.toString().indexOf("Event")!=-1){
                return _argument;
            }
        }
        _caller = _caller.caller;
    }
    return null;
}
if(window.addEventListener){
    __firefox();
}
/*end firefox*/


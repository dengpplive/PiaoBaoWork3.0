var _BarIdName="___tooltipbar";
var _BarTitleIdName="___tooltipbartitle";
String.prototype.NewReplace=function (sourceData,replaceData) {
    sourceData=sourceData.replace("(","\\(").replace(")","\\)");
    var reg=new RegExp(sourceData,"ig");
    var data=this.replace(reg,replaceData);
    return data;
}
function G(re) {
    return document.getElementById(re);
}
//页面上调用的方法
function initToolTip(tagName) {
    initTipbar();//创建tooltip显示区域	
    var tagaArry=new Array();
    var tag=null;
    if(tagName==undefined) {
        tagName="td";
    }
    tagaArry=document.getElementsByTagName(tagName);
    for(var i=0;i<tagaArry.length;i++) {
        tag=tagaArry[i];
        var oldTitle=tag.title;
        if(oldTitle=="") {
            hideTipbar(this);
        } else {
            //为超级连接加入鼠标事件
            //进入连接
            tag.onmouseover=function () {
                showTipbar(this);
            };
            //离开连接
            tag.onmouseout=function () {
                hideTipbar(this);
            }
            //在连接上移动，设置坐标
            tag.onmousemove=function () {
                setTipLocation();
            }
            //为Firefox加入mousemove的事件侦听,因为window.eventFirefox没有办法用
            if(tag.addEventListener) {
                tag.addEventListener('mousemove',setTipLocation,true);
            }
        }
    }

}
//初始化tooltip区域
function initTipbar() {
    var div=document.createElement("div");
    div.className="divtip";
    div.id=_BarIdName;
    div.style.zIndex=1000001;

    var divTitle=document.createElement("p");
    divTitle.id=_BarTitleIdName;
    divTitle.style.zIndex=1000001;
    divTitle.className="tip";
    div.appendChild(divTitle);
    document.body.appendChild(div);
    div.style.display="none";
}

//显示tooltip
function showTipbar(tag) {
    var tipbar=G(_BarIdName);
    var tipTitle=G(_BarTitleIdName);

    tipTitle.innerHTML=tag.title.NewReplace("<","&lt;").NewReplace(">","&gt;");
    tag.title="";
    tipbar.style.display="";
}

//隐藏tooltip
function hideTipbar(tag) {
    var tipbar=G(_BarIdName);
    var tipTitle=G(_BarTitleIdName);
    tipbar.style.display="none";
    tag.title=tipTitle.innerHTML;
}

//设置tipbar的位置
function setTipLocation(e) {
    var intX=0,intY=0;
    //判断有没有收到firefox的监听的event
    if(e==null) {
        //当没收到时用window.event IE与Opera支持的
        e=window.event;
    }

    if(e.pageX||e.pageY) {
        intX=e.pageX;intY=e.pageY;
    }
    else if(e.clientX||e.clientY) {
        if(document.documentElement.scrollTop) {
            intX=e.clientX+document.documentElement.scrollLeft;
            intY=e.clientY+document.documentElement.scrollTop;
        }
        else {
            intX=e.clientX+document.body.scrollLeft;
            intY=e.clientY+document.body.scrollTop;
        }
    }
    //取得tooltip对象
    var tipbar=G(_BarIdName);
    tipbar.style.top=(intY+5)+"px";
    tipbar.style.left=(intX+5)+"px";
}


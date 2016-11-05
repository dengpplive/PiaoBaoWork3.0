/*
作者：肖毅(YesSky)
作品: MC.Dialog js弹出层
时间: 2009.10.12 更新2010.4.17
版本: MC.Dialog.10.4.17.js
版权: 本程序版权归作者肖毅，你可以使用本程序，但不得作为商业用途，不得以本程序盈利
如果修改本程序必须通知本人并注明，在任何地方使用必须保留本段申明,如果违反本原则将追究相关责任
网站: http://live126.cn/
blog: http://blog.sina.com.cn/zkunet
qq:   441548727
mail: xuanziday@163.com 441548727@qq.com
*/

var PicPath = "images/";
var isIE = navigator.userAgent.toLowerCase().indexOf("msie") != -1;
var isIE6 = navigator.userAgent.toLowerCase().indexOf("msie 6.0") != -1;
var isGecko = navigator.userAgent.toLowerCase().indexOf("gecko") != -1;
function _TW() {    
    var w = window;
    var t;
    if (w == top) {
        t = w;
    } else {
        t = top;
    }
    return t;
};

function stopEvent(event) {
    if (!event) {
        return;
    }
    if (isGecko) {
        event.preventDefault();
        event.stopPropagation();
    }
    event.cancelBubble = true
    event.returnValue = false;
};

Array.prototype.each = function (fn, i) {
    for (var i = 0; i < this.length; i++) {
        fn(this[i], i);
    }
    return this;
};

Array.prototype.include = function (tar) {
    var flag = -1;
    this.each(function (it) {
        if (it == tar) flag = 1;
    })
    return flag;
};

function Dialog(sID) {
    this.ID = sID;
    this.Width = 400;
    this.Height = 200;
    this.Title = "系统提示";
    this.URL = null;
    this.Alert = "";
    this.DialogArguments = {};
    this.Message = null;
    this.MessageTitle = null;
    this.ShowMessageRow = false;
    this.ShowButtonRow = true;
    this.Icon = null;
    this.Mask = null;
    this.TW = null; //保存每个实例的页面所对应的顶级父窗口 以保证跨域仍然可以关闭
}

Dialog.Stock = [];  //保存Dialog实例队列

Dialog.prototype.show = function () {
    var pw = _TW();
    function serial() {
        var d = new Date();
        return d.getYear().toString() + (d.getMonth() + 1).toString() + d.getDate().toString() + d.getHours().toString() + d.getMinutes().toString() + d.getSeconds().toString() + d.getMilliseconds().toString();
    }
    if (pw.Dialog.Stock.include(this.ID)) {
        this.ID = this.ID + '_' + serial();  //防止ID重复
    }
    var mw = Math.max(pw.document.body.clientWidth, pw.document.body.scrollWidth);
    var mh = Math.max(pw.document.body.clientHeight, pw.document.body.scrollHeight);
    mw = Math.max(mw, pw.document.documentElement.clientWidth);
    mh = Math.max(mh, pw.document.documentElement.clientHeight);

    //hide select 
    if (isIE6) {
        //当没有队列时才隐藏select
        if (!pw.Dialog.Stock.length) {
            
            var ps = pw.document.getElementsByTagName("select");
            for (var i = 0; i < ps.length; i++) {
                ps[i].style.display = "none";
            }
            function hf(win) {
                var cf = null;
                if (win.getElementsByTagName) {
                    cf = win.getElementsByTagName("iframe");
                } else {
                    cf = win.document.getElementsByTagName("iframe");
                }
                for (var i = 0; i < cf.length; i++) {
                    var e = cf[i];
                    var ef = e.getElementsByTagName("select");
                    for (var k = 0; k < ef.length; k++) {
                        ef[k].style.display = "none";
                    }
                    //调用自己 继续查找每个iframe中是否还有iframe
                    hf(e);
                }
            }
            hf(pw);
        }
    }
    //init param
    this.TW = pw;
    this.DialogArguments._DialogInstance = this;
    this.DialogArguments.ID = this.ID;
    if (!this.Height) {
        this.Height = this.Width / 2;
    }

    var frameHeight = this.Height - 46;
    if (this.ShowMessageRow) {
        frameHeight = frameHeight - 50;
    }
    var frameWidth = this.Width - 26;
    if (this.ShowButtonRow) {
        frameHeight = frameHeight - 36;
    }

    //UI
    var arr = [];
    arr.push("<table style='-moz-user-select:none;' oncontextmenu='stopEvent(event);' onselectstart='stopEvent(event);' width='100%' height='100%' border='0' cellspacing='0' cellpadding='0'>");
    arr.push("<tr id='_DialogTitle_" + this.ID + "'>");
    arr.push("<td width='13' height='33' align='center' style=\"background:url(" + PicPath + "dialog_lt.png) !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_lt.png', sizingMethod='crop');background-repeat:no-repeat;background-attachment:fixed;\">");
    arr.push("<div style='width:13px;'></div></td>");
    arr.push("<td height='33' style=\"background:url(" + PicPath + "dialog_ct.png)  !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_ct.png', sizingMethod='crop'); background-repeat:repeat-x;background-attachment:fixed;\">");
    arr.push("<div style='float:left;font-weight:bold; color:#FFFFFF; padding:9px 0 0 4px; font-size:12px;'>");
    arr.push("<img src='" + PicPath + "icon_dialog.gif' align='absmiddle'>&nbsp;" + this.Title + "</div>");
    arr.push("<div style='position: relative;cursor:pointer; float:right; margin:5px 0 0; _margin:4px 0 0; height:17px; width:28px; background:url(" + PicPath + "dialog_closebtn.gif);' onMouseOver=\"this.style.background='url(" + PicPath + "dialog_closebtn_over.gif)'\" onMouseOut=\"this.style.background='url(" + PicPath + "dialog_closebtn.gif)'\" id='_DialogClose_" + this.ID + "' drag='false' onClick=\"Dialog.getInstance('" + this.ID + "').CancelButton.onclick.apply(Dialog.getInstance('" + this.ID + "').CancelButton,[]);\"></div></td>");
    arr.push("<td width='13' height='33' style=\"background:url(" + PicPath + "dialog_rt.png) !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_rt.png', sizingMethod='crop'); background-repeat:no-repeat;background-attachment:fixed;\">");
    arr.push("<div style='width:13px;'></div></td></tr>");
    arr.push("<tr drag='false'><td width='13' style=\"background:url(" + PicPath + "dialog_mlm.png) !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_mlm.png', sizingMethod='crop'); background-repeat:no-repeat;\"></td>");
    arr.push("<td align='center' valign='top'>");
    arr.push("<table width='100%' height='100%' border='0' cellpadding='0' cellspacing='0'>");
    arr.push("<tr id='_MessageRow_" + this.ID + "' style='display:none;'>");
    arr.push("<td height='50' valign='top' style='border-bottom:1px inset #666666;'><table id='_MessageTable_" + this.ID + "' width='100%' border='0' cellspacing='0' cellpadding='8' style='background:#EAECE9 url(" + PicPath + "dialog_bg.jpg) no-repeat right top;'>");
    arr.push("<tr><td width='40' height='50' align='right'><img id='_MessageIcon_" + this.ID + "' src='" + PicPath + "window.gif' width='32' height='32'></td>");
    arr.push("<td align='left' style='line-height:16px; padding-left:6px;'>");
    arr.push("<h5 id='_MessageTitle_" + this.ID + "'>&nbsp;</h5>");
    arr.push("<div id='_Message_" + this.ID + "'>&nbsp;</div></td>");
    arr.push("</tr></table></td></tr>");
    arr.push("<tr><td align='center' valign='top' height='100%' bgcolor='#ffffff'");
    arr.push("valign='top'>");
    arr.push("<iframe src='" + this.URL + "' id='_DialogFrame_" + this.ID + "' allowTransparency='true' width='" + frameWidth + "' height='" + frameHeight + "' frameborder='0' style='background-color: #transparent;border:none;overflow-x:hidden;'></iframe>");
    arr.push("</td></tr>");
    arr.push("<tr drag='false' id='_ButtonRow_" + this.ID + "'><td height='36'>");
    arr.push("<div id='_DialogButtons_" + this.ID + "' style='text-align:right; border-top:#dadee5 1px solid; padding:8px 20px; background-color:#f6f6f6;'>");
    arr.push("<input id='_ButtonOK_" + this.ID + "' type='button' value='确定'>");
    arr.push("<input id='_ButtonCancel_" + this.ID + "' type='button' onclick=\"Dialog.getInstance('" + this.ID + "').close();\" value='取消'>");
    arr.push("</div></td></tr>");
    arr.push("</table></td><td width='13' style=\"background:url(" + PicPath + "dialog_mrm.png) !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_mrm.png', sizingMethod='crop'); background-repeat:no-repeat;\"></td></tr>");
    arr.push("<tr><td width='13' height='13' style=\"background:url(" + PicPath + "dialog_lb.png) !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_lb.png', sizingMethod='crop'); background-repeat:no-repeat;\"></td>");
    arr.push("<td style=\"background:url(" + PicPath + "dialog_cb.png) !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_cb.png', sizingMethod='crop'); background-repeat:repeat-x;\"></td>");
    arr.push("<td width='13' height='13' style=\"background:url(" + PicPath + "dialog_rb.png) !important;background: none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + PicPath + "dialog_rb.png', sizingMethod='crop');; background-repeat:no-repeat;\"></td>");
    arr.push("</tr></table>");

    var Mask = pw.gById("_DialogMask");
    if (!Mask) {
        Mask = pw.document.createElement("div");
        Mask.style.display = "none";
        Mask.id = "_DialogMask";
        Mask.style.position = "absolute";
        Mask.style.zIndex = 5000;
        Mask.style.width = mw;
        Mask.style.height = mh;
        Mask.style.top = "0px";
        Mask.style.left = "0px";
        Mask.style.background = "#33393C";
        Mask.style.filter = "alpha(opacity=10)";
        Mask.style.opacity = "0.10";
        pw.document.body.appendChild(Mask);
    }

    var div = pw.document.createElement("div");
    div.style.display = "none";
    div.id = "_Dialog_" + this.ID;
    div.style.position = "absolute";
    div.style.background = "transparent"; //背景设为透明
    div.style.zIndex = 10000;
    div.style.width = this.Width + "px";
    div.style.height = this.Height + "px";
    div.style.top = parseInt(pw.document.body.scrollTop + pw.document.body.clientHeight / 2 - this.Height / 2) + "px";
    div.style.left = parseInt(pw.document.body.clientWidth / 2 - this.Width / 2) + "px";
    pw.document.body.appendChild(div);
    this.DialogDiv = div;
    div.innerHTML = arr.join('\n');

    pw.gById("_DialogFrame_" + this.ID).DialogInstance = this;
    Drag.init(pw.gById("_DialogTitle_" + this.ID), pw.gById("_Dialog_" + this.ID));
    this.OKButton = pw.gById("_ButtonOK_" + this.ID);
    this.CancelButton = pw.gById("_ButtonCancel_" + this.ID);

    if (this.ShowMessageRow) {
        pw.gById("_MessageRow_" + this.ID).style.display = "";
        if (this.MessageTitle) {
            pw.gById("_MessageTitle_" + this.ID).innerHTML = this.MessageTitle;
        }
        if (this.Message) {
            pw.gById("_Message_" + this.ID).innerHTML = this.Message;
        }
    }
    if (!this.ShowButtonRow) {
        pw.gById("_ButtonRow_" + this.ID).style.display = "none";
    }
    if (this.CancelEvent) {
        this.CancelButton.onclick = this.CancelEvent;
        this.returnValue = 0;
    }
    if (this.OKEvent) {
        this.OKButton.onclick = this.OKEvent;
        this.returnValue = 1;  //确定事件 返回1
    }
    if (this.URL == "") {
        var win = pw.gById("_DialogFrame_" + this.ID).contentWindow;
        var doc = win.document;
        doc.open();
        doc.write("<body oncontextmenu='return false;'></body>");
        var arr = [];
        arr.push("<table height='100%' border='0' align='left' cellpadding='10' cellspacing='0'>");
        arr.push("<tr><td align='right'><img id='Icon' src='" + this.Icon + "' width='34' height='34' align='absmiddle'></td>");
        arr.push("<td align='left' id='Message' style='font-size:9pt'>" + this.Alert + "</td></tr></table>");
        var div = doc.createElement("div");
        div.innerHTML = arr.join('');
        doc.body.appendChild(div);
        doc.close();
    }
    this.Mask = "_DialogMask";
    pw.gById("_Dialog_" + this.ID).style.display = "";
    if (pw.gById("_DialogMask").style.display == "none") {
        pw.gById("_DialogMask").style.display = "";
        var k = 1, timer = null;
        timer = setInterval(function () {
            k = k + 1;
            if (k < 5) {
                Mask.style.filter = "alpha(opacity=" + k * 10 + ")";
                Mask.style.opacity = "" + k / 10 + "";
            }
        }, 10);
    } else {
        clearInterval(timer);
    }
    pw.gById("_Dialog_" + this.ID).focus();
    pw.gById("_DialogFrame_" + this.ID).focus();
    if (this.ShowButtonRow) {
        if (this.OKButton.style.display == "" || this.OKButton.style.display == "block") {
            this.OKButton.focus();
        } else if (this.CancelButton.style.display == "" || this.CancelButton.style.display == "block") {
            this.CancelButton.focus();
        } else {
            pw.gById("_DialogTitle_" + this.ID).focus();
        }
    }
    pw.Dialog.Stock.push(this.ID);
    //如果已经有弹出层了 将现有的层至于遮罩下
    if (pw.Dialog.Stock.length > 1) {
        for (i = 0; i < pw.Dialog.Stock.length - 1; i++) {
            pw.Dialog.getInstance(pw.Dialog.Stock[i]).DialogDiv.style.zIndex = 4000 + i;
        }
    }
}

Dialog.prototype.close = function () {
    var pw = _TW();
    //从队列中删除
    pw.Dialog.Stock.remove(this.ID);
    //从dom中删除层 先隐藏再删除
    pw.gById("_Dialog_" + this.ID).style.display = "none";
    pw.document.body.removeChild(pw.gById("_Dialog_" + this.ID));
    //控制焦点和遮罩
    if (pw.Dialog.Stock.length == 0) { //没有弹出层了 隐藏遮罩
        pw.gById("_DialogMask").style.display = "none";
        pw.document.body.removeChild(pw.gById("_DialogMask"));
    } else { //将队列最后的一个层位置设为10000 即当前层可见 同时注册拖拽
        var lastID = pw.Dialog.Stock[pw.Dialog.Stock.length - 1];
        pw.Dialog.getInstance(lastID).DialogDiv.style.zIndex = 10000;
        pw.Drag.init(pw.gById("_DialogTitle_" + lastID), pw.gById("_Dialog_" + lastID));
        if (pw.Dialog.getInstance(lastID).ShowButtonRow) {
            if (pw.gById("_ButtonOK_" + lastID).style.display == "" || pw.gById("_ButtonOK_" + lastID).style.display == "block") {
                pw.gById("_ButtonOK_" + lastID).focus();
            } else if (pw.gById("_ButtonCancel_" + lastID).style.display == "" || pw.gById("_ButtonCancel_" + lastID).style.display == "block") {
                pw.gById("_ButtonCancel_" + lastID).focus();
            } else {
                pw.gById("_DialogTitle_" + lastID).focus();
            }
        } else {
            pw.gById("_DialogTitle_" + lastID).focus();
        }
    }
    //检测MC Dialog队列是否清空 清空则显示界面所有的select
    if (isIE6) {
        //当没有队列时才重新显示select
        if (!pw.Dialog.Stock.length) {
            var ps = pw.document.getElementsByTagName("select");
            for (var i = 0; i < ps.length; i++) {
                ps[i].style.display = "";
            }
            function hf(win) {
                //var cf = win.getElementsByTagName("iframe");
                var cf = null;
                if (win.getElementsByTagName) {
                    cf = win.getElementsByTagName("iframe");
                } else {
                    cf = win.document.getElementsByTagName("iframe");
                }
                for (var i = 0; i < cf.length; i++) {
                    var e = cf[i];
                    var ef = e.getElementsByTagName("select");
                    for (var k = 0; k < ef.length; k++) {
                        ef[k].style.display = "";
                    }
                    //递归 继续查找每个iframe中是否还有iframe
                    hf(e);
                }
            }
            hf(pw);
        }
    }
}

Dialog.blink = function (event) { //当焦点移出了层 则闪烁提示 将焦点对到队列最后一个层
    var pw = _TW();
    if (pw.Dialog.Stock.length > 0) {
        var target = pw.Dialog.getInstance(pw.Dialog.Stock[pw.Dialog.Stock.length - 1]);
        var sl = parseInt(target.DialogDiv.style.left);
        var st = parseInt(target.DialogDiv.style.top);
        var sr = sl + target.Width;
        var sb = st + target.Height;
        if (event.clientX < sl || event.clientY < st || event.clientX > sr || event.clientY > sb) {
            if (!isIE6) {
                //pw.gById("_DialogTitle_"+target.ID).childNodes[1].style.filter = "alpha(opacity=20)";
                pw.gById("_DialogTitle_" + target.ID).style.opacity = "0.20";
                setTimeout(function () {
                    //pw.gById("_DialogTitle_"+target.ID).childNodes[1].style.filter = "alpha(opacity=0)";
                    pw.gById("_DialogTitle_" + target.ID).style.opacity = "1.00";
                }
		  , 50);
            }
        }
        if (target.ShowButtonRow) {
            if (pw.gById("_ButtonOK_" + target.ID).style.display == "" || pw.gById("_ButtonOK_" + target.ID).style.display == "block") {
                pw.gById("_ButtonOK_" + target.ID).focus();
            } else if (pw.gById("_ButtonCancel_" + target.ID).style.display == "" || pw.gById("_ButtonCancel_" + target.ID).style.display == "block") {
                pw.gById("_ButtonCancel_" + target.ID).focus();
            } else {
                pw.gById("_DialogTitle_" + target.ID).focus();
            }
        } else {
            pw.gById("_DialogTitle_" + target.ID).focus();
        }
    } else {
        return;
    }
}

Dialog.getInstance = function (id) {
    var pw = _TW();
    var f = pw.gById("_DialogFrame_" + id);
    if (!f) {
        return null;
    }
    return f.DialogInstance;
}

Dialog.Alert = function (str) {
    var diag = new Dialog("Alert");
    diag.Width = 400;
    diag.Height = 224;
    diag.Title = "系统提示";
    diag.URL = "";
    diag.Alert = str;
    diag.Icon = PicPath + "icon_alert.gif"
    diag.ShowMessageRow = false;
    diag.show();
    diag.CancelButton.value = "确定";
    diag.OKButton.style.display = "none";
    diag.CancelButton.focus();
}

Dialog.Confirm = function (str, func1, func2) {
    var pw = _TW();
    var diag = new Dialog("Confirm");
    diag.Width = 400;
    diag.Height = 224;
    diag.Title = "系统提示-操作确认";
    diag.URL = "";
    diag.Alert = str;
    diag.Icon = PicPath + "icon_query.gif";
    diag.ShowMessageRow = false;
    diag.CancelEvent = function () {
        diag.close();
        if (func2) {
            func2();
        }
    };
    diag.OKEvent = function () {
        diag.close();
        if (func1) {
            func1();
        }
    };
    diag.show();
}

Dialog.setPosition = function () {
    var pw = _TW();
    var DialogArr = pw.Dialog.Stock;
    if (DialogArr == null || DialogArr.length == 0) return;
    for (i = 0; i < DialogArr.length; i++) {
        pw.gById("_DialogFrame_" + DialogArr[i]).DialogInstance.setPosition();
    }
}

Dialog.prototype.setPosition = function () {
    var pw = _TW();
    var mw = Math.max(pw.document.body.clientWidth, pw.document.body.scrollWidth);
    var mh = Math.max(pw.document.body.clientHeight, pw.document.body.scrollHeight);
    this.DialogDiv.style.top = parseInt(pw.document.body.scrollTop + pw.document.body.clientHeight / 2 - this.Height / 2) + "px";
    this.DialogDiv.style.left = parseInt(pw.document.body.clientWidth / 2 - this.Width / 2) + "px";
    pw.gById(this.Mask).style.width = mw + "px";
    pw.gById(this.Mask).style.height = mh + "px";
}

Dialog.onKeyUp = function (event) {
    if (event.keyCode == 9) {
        var pw = _TW();
        if (pw.Dialog.Stock.length > 0) {
            stopEvent(event);
        }
    }
    if (event.keyCode == 27) {
        var pw = _TW();
        if (pw.Dialog.Stock.length > 0) {
            var diag = pw.Dialog.getInstance(pw.Dialog.Stock[pw.Dialog.Stock.length - 1]);
            diag.CancelButton.onclick.apply(diag.CancelButton, []);
        } else {
            stopEvent(event);
        }
    }
    if (event.ctrlKey && event.keyCode == 65) {
        stopEvent(event);
    }
}

var Drag = {
    target: null,
    win: _TW(),
    tmp: null,
    init: function (handle, dragMain) {
        Drag.target = handle;
        Drag.target.style.cursor = "move";
        handle.root = dragMain;
        handle.onmousedown = Drag.start;
    },
    start: function () {
        var el = Drag.win.event.srcElement;
        var f = el.id.indexOf("_DialogClose_") != -1 ? true : false;
        if (!f) {
            var handle = Drag.target;
            var s = Drag.win.document.createElement("div");
            Drag.win.document.body.appendChild(s);
            s.style.display = "none";
            s.style.zIndex = parseInt(handle.root.style.zIndex) + 1;
            s.id = "_Dialog_Tmp_" + handle.root.id.replace("_Dialog_");
            s.style.position = "absolute";
            s.style.background = "#FF0066";
            s.style.width = handle.root.style.width;
            s.style.height = handle.root.style.height;
            s.style.top = handle.root.style.top;
            s.style.left = handle.root.style.left;
            s.style.filter = "alpha(opacity=20)";
            s.style.opacity = "0.20";
            s.style.cursor = "move";
            s.style.display = "block";
            Drag.tmp = s;
            Drag.mouseX = Drag.win.event.clientX;
            Drag.mouseY = Drag.win.event.clientY;
            Drag.flag = true;
            Drag.win.document.onmousemove = Drag.move;
            Drag.win.document.onmouseup = Drag.end;
        }
    },
    move: function () {
        if (Drag.flag) {
            if (Drag.win.event.button == 1 | Drag.win.event.which == 1) {
                //由于可能产生选中文字 这里清除选中部分
                Drag.win.document.selection.empty();
                var handle = Drag.target;
                var mouseX = Drag.win.event.clientX;
                var mouseY = Drag.win.event.clientY;
                var top = parseInt(Drag.tmp.style.top);
                var left = parseInt(Drag.tmp.style.left);
                var cl = left + (mouseX - Drag.mouseX);
                var ct = top + (mouseY - Drag.mouseY);
                //判断是否将对象拖出了可视范围
                var ww = Drag.win.document.body.clientWidth;
                var wh = Drag.win.document.body.clientHeight;
                if (cl + parseInt(Drag.tmp.style.width) > ww) {
                    cl = ww - parseInt(Drag.tmp.style.width);
                }
                if (ct + parseInt(Drag.tmp.style.height) > wh) {
                    ct = wh - parseInt(Drag.tmp.style.height);
                }
                Drag.tmp.style.left = cl + "px";
                Drag.tmp.style.top = ct + "px";
                Drag.mouseX = mouseX;
                Drag.mouseY = mouseY;
            }
        }
    },
    end: function () {
        var handle = Drag.target;
        Drag.win.document.selection.empty();
        Drag.win.document.onmousemove = null;
        Drag.win.document.onmouseup = null;
        handle.root.style.display = "none";
        handle.root.style.top = Drag.tmp.style.top;
        handle.root.style.left = Drag.tmp.style.left;
        Drag.win.document.body.removeChild(Drag.tmp);
        handle.root.style.display = "";
        Drag.flag = false;
    }
};
/*
var Drag = {
target:null,
win:_TW(),
init:function(handle,dragMain) {
Drag.target = handle;
Drag.target.style.cursor="move";
handle.root = dragMain; 
handle.onmousedown = Drag.start;
},
start:function() {
var handle = Drag.target;
Drag.mouseX = Drag.win.event.clientX;
Drag.mouseY = Drag.win.event.clientY;
Drag.flag = true;
Drag.win.document.onmousemove = Drag.move;
Drag.win.document.onmouseup = Drag.end;
},
move:function() {
if(Drag.flag) { 
if(Drag.win.event.button==1 | Drag.win.event.which==1) {
var handle = Drag.target;
var mouseX = Drag.win.event.clientX;
var mouseY = Drag.win.event.clientY;
var top = parseInt(handle.root.style.top);
var left = parseInt(handle.root.style.left);
var cl = left + (mouseX - Drag.mouseX);
var ct = top + (mouseY - Drag.mouseY);
//判断是否将对象拖出了可视范围
var ww = Drag.win.document.body.clientWidth;
var wh = Drag.win.document.body.clientHeight;
if(cl + parseInt(handle.root.style.width) > ww) {
cl = ww - parseInt(handle.root.style.width);
}
if(ct + parseInt(handle.root.style.height) > wh) {
ct = wh - parseInt(handle.root.style.height);
}
handle.root.style.left = cl + "px";
handle.root.style.top = ct + "px";
Drag.mouseX = mouseX;
Drag.mouseY = mouseY;
}
}
},
end:function() {
Drag.win.document.onmousemove = null;
Drag.win.document.onmouseup = null;
var handle = Drag.target;
Drag.flag = false;
}
};
*/

(function () {
    if (isIE) {
        document.attachEvent("onkeydown", Dialog.onKeyUp);
        document.attachEvent("onclick", Dialog.blink);
        window.attachEvent('onresize', Dialog.setPosition);
    } else {
        document.addEventListener("keydown", Dialog.onKeyUp, false);
        document.addEventListener("click", Dialog.blink, false);
        window.addEventListener('resize', Dialog.setPosition, false);
    }
})();

function alert(str) {
    Dialog.Alert(str);
}

function confirm(str, func1, func2) {
    Dialog.Confirm(str, func1, func2);
}

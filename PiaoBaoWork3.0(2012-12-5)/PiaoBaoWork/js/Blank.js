//>
function insertChar() {
    $("#txaBlack").val($("#txaBlack").val() + ">");
}

//读取用户之前的记录
function insertTXT() {
    $.post("../Ajax/BlackData.aspx", { BCode: "txt", KeyCode: "123", KeyWord: "txt" }, function (data) {
        $("#txaBlack").val($("#txaBlack").val() + data + ">");
    }, "text");
}
//条件查询航班
function search() {
    //查询条件不能为空
    if ($("#hiStart").val() != "" && $("#hiTarget").val() != "" && $("#dcstartdate").val() != "") {
        //组合AVH指令avh/ctupek/10may
        //转换日期
        var mm;
        switch ($("#dcstartdate").val().replace("/", "-").split('-')[1]) {
            case ("01"):
                { mm = "JAN"; break; }
            case ("1"):
                { mm = "JAN"; break; }
            case ("02"):
                { mm = "FEB"; break; }
            case ("2"):
                { mm = "FEB"; break; }
            case ("3"):
                { mm = "MAR"; break; }
            case ("03"):
                { mm = "MAR"; break; }
            case ("4"):
                { mm = "APR"; break; }
            case ("04"):
                { mm = "APR"; break; }
            case ("5"):
                { mm = "MAY"; break; }
            case ("05"):
                { mm = "MAY"; break; }
            case ("6"):
                { mm = "JUN"; break; }
            case ("06"):
                { mm = "JUN"; break; }
            case ("7"):
                { mm = "JUL"; break; }
            case ("07"):
                { mm = "JUL"; break; }
            case ("08"):
                { mm = "AUG"; break; }
            case ("8"):
                { mm = "AUG"; break; }
            case ("09"):
                { mm = "SEP"; break; }
            case ("9"):
                { mm = "SEP"; break; }
            case ("10"):
                { mm = "OCT"; break; }
            case ("11"):
                { mm = "NOV"; break; }
            case ("12"):
                { mm = "DEC"; break; }
        }
        //10may
        mm = $("#dcstartdate").val().replace("/", "-").split('-')[2] + mm;
        //组合AVH指令avh/ctupek/10may
        var avh = "avh/" + $("#hiStart").val() + $("#hiTarget").val() + "/" + mm;
        //航空公司
        if ($("#hkgs").val() != "") {
            avh = avh + "/" + $("#hkgs").val();
        }
        if ($("#txaBlack").val().substring($("#txaBlack").val().length - 1) == ">") {
            $("#txaBlack").val($("#txaBlack").val() + "\r" + avh);
        } else {
            $("#txaBlack").val($("#txaBlack").val() + "\r>" + avh);
        }
        onKey("avh");
    }
}

//键盘按键操作
function onKey(type) {
    //c=67,v=86,a=65,x=88,#=16+51,@=16+50,*=106/16+56
    //Esc--前缀
    if (event.keyCode == 27) {
        insertChar();
    }
    //F7--清屏
    if (event.keyCode == 118) {
        $.post("../Ajax/BlackData.aspx", { BCode: "deletetxt", KeyCode: "123", KeyWord: "deletetxt" }, function (data) {
            $("#txaBlack").val($("#txaBlack").val() + data);
        }, "text");
        $("#txaBlack").val(">");
        window.scrollTo(0, document.body.scrollHeight)
        event.returnValue = false;
        return false;
    }
    //F12 | 按钮查询航班 | ctrl+回车--执行命令
    else if (event.keyCode == 123 || type == "avh" || (event.ctrlKey && window.event.keyCode == 13)) {
        //获得操作指令
        var bCode = $("#txaBlack").val().split('>')[$("#txaBlack").val().split('>').length - 1].toString();
        var txaBlack = "";
        var x = "0";
        if (bCode.toLowerCase() == "cp") { 
            $("#txaBlack").val("");
            x = "1";
        }  
        if (bCode.length > 0) {
            try {
                document.getElementById("hidAVHCity").value = bCode;
                $("#txaBlack").attr("readonly", true);
                //调用ajax方法
                if (x == "0") {
                    $.post("../Ajax/BlackData.aspx", { BCode: bCode, KeyCode: "123", KeyWord: "eterm" }, function (data) {
                        $("#txaBlack").val($("#txaBlack").val() + data + "\r>");
                    }, "text"); 
                    $("#txaBlack").attr("readonly", false); 
                }
            } catch (e) {
                $("#txaBlack").attr("readonly", false);
            }
        }
        //alert(document.getElementByIdx('txaBlack').scrollHeight);
        window.scrollTo(0, document.body.scrollHeight)
        event.returnValue = false;
        return false;
    }
   
}


 

function colorSet() {
    document.getElementById("trColorPicker").style.display = "none";
    var txaStyle = "background-color: #000000;margin: 0px;font-family: black;color: " + document.getElementById("cp").value + "; font-size: 16px;scroll: no;width: 100%;height: 365px;";
    document.getElementById("txaBlack").style.cssText = txaStyle;
}

//黑屏全屏
function visibleDivRight() {
    if (document.getElementById("divRight").style.display == "none") {
        document.getElementById("divRight").style.display = "block";
        document.getElementById("divLeft").style.width = "78%";
        document.getElementById("imgMid").src = "../img/ico_midright.gif";
        document.getElementById("imgMid").alt = "隐藏";
    } else {
        document.getElementById("divRight").style.display = "none";
        document.getElementById("divLeft").style.width = "97%";
        document.getElementById("imgMid").src = "../img/ico_midleft.gif";
        document.getElementById("imgMid").alt = "显示";

    }
}

function cabinTGQ(x, y) {
    //选中的字符串，去除空格
    var selVal = document.selection.createRange().text.replace(/\s+/g, "");
    //长度等于2
    if (selVal.length == 2) {
        //获得航空公司
        var air = meizz();
        $.post("../Ajax/BlackData.aspx", { BCode: air + "|" + selVal.substring(0, 1) + "|" + document.getElementById("hidAVHCity").value, KeyCode: "cabin" }, function (data) {
            document.getElementById("divCabin").style.top = x;
            document.getElementById("divCabin").style.left = y;
            document.getElementById("divCabin").innerHTML = data;
            document.getElementById("divCabin").style.display = 'block';
        }, "text");
    } else {
    }
}


//返回航空公司
function meizz() {
    var air;
    try {
        document.all.txaBlack.focus();
        //选中的字符串
        var sel = document.selection.createRange();
        //选中的字符串前面的指定长度的字符串
        sel.moveStart('word', -40);
        var pre = sel.text;
        //舱位字符串，从航班号开始到选中的字符串为止
        //根据换行符获得最后一行数据
        var strCabin = pre.split('\r')[pre.split('\r').length - 1];

        var isCabin = false;
        if (strCabin.split(' ').length > 0) {
            try {
                for (var i = 0; i < strCabin.split(' ').length; i++) {
                    //判断最后一行数据是否有航班号
                    if (strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 5 || strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 6) {
                        air = strCabin.split(' ')[i].replace(">", "").replace("*", "").substring(0, 2);
                        isCabin = true;
                        break;
                    }
                }
            } catch (err) { }
        }
        //根据换行符获得倒数第二行数据
        if (!isCabin) {
            if (pre.split('\r').length > 1) {
                try {
                    strCabin = pre.split('\r')[pre.split('\r').length - 2];
                    for (var i = 0; i < strCabin.split(' ').length; i++) {
                        //判断倒数第二行数据是否有航班号
                        if (strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 5 || strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 6) {
                            air = strCabin.split(' ')[i].replace(">", "").replace("*", "").substring(0, 2);
                            break;
                        }
                    }
                } catch (err) { }
            }
        }
    } catch (err) { }
    return air;
}


function onFocusStr(x, y) {
    var cabin;
    var air;
    try {
        var sel = document.selection.createRange();
        //选中的字符串前面的指定长度的字符串
        sel.moveStart('word', -40);
        sel.moveEnd('word', 1);
        cabin = sel.text;
        air = cabin;
        var isTrue = false;
        var isCabin = false;
        if (cabin.length > 2 && cabin.split(' ').length > 1) {
            if (cabin.split(' ')[cabin.split(' ').length - 1].length == 2) {
                isTrue = true;
                cabin = cabin.split(' ')[cabin.split(' ').length - 1].replace(/\s+/g, "");
            } else if (cabin.split(' ')[cabin.split(' ').length - 2].length == 2) {
                isTrue = true;
                cabin = cabin.split(' ')[cabin.split(' ').length - 2].replace(/\s+/g, "");
            }
        }
        //舱位选取正常
        if (isTrue) {
            //舱位字符串，从航班号开始到选中的字符串为止
            //根据换行符获得最后一行数据
            var strCabin = air.split('\r')[air.split('\r').length - 1];
            var isCabin = false;
            if (strCabin.split(' ').length > 0) {
                try {
                    for (var i = 0; i < strCabin.split(' ').length; i++) {
                        //判断最后一行数据是否有航班号
                        if (strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 5 || strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 6) {
                            air = strCabin.split(' ')[i].replace(">", "").replace("*", "").substring(0, 2);
                            isCabin = true;
                            break;
                        }
                    }
                } catch (err) { }
            }
            //根据换行符获得倒数第二行数据
            if (!isCabin) {
                if (air.split('\r').length > 1) {
                    try {
                        strCabin = air.split('\r')[air.split('\r').length - 2];
                        for (var i = 0; i < strCabin.split(' ').length; i++) {
                            //判断倒数第二行数据是否有航班号
                            if (strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 5 || strCabin.split(' ')[i].replace(">", "").replace("*", "").length == 6) {
                                air = strCabin.split(' ')[i].replace(">", "").replace("*", "").substring(0, 2);
                                isCabin = true;
                                break;
                            }
                        }
                    } catch (err) { }
                }
            }
            if (isCabin) {
                $.post("../Ajax/BlackData.aspx", { BCode: air + "|" + cabin.substring(0, 1) + "|" + document.getElementById("hidAVHCity").value, KeyCode: "cabin" }, function (data) {
                    document.getElementById("divCabin").style.top = x;
                    document.getElementById("divCabin").style.left = y;
                    document.getElementById("divCabin").innerHTML = data;
                    document.getElementById("divCabin").style.display = 'block';
                }, "text");
            }
        }
    } catch (err) { }
}

function runCode() {
    try {
        var code;
        var sel = document.selection.createRange();
        //选中的字符串前面的指定长度的字符串
        sel.moveStart('word', -40);
        code = sel.text;
        if (code != null && code != "" && code.length > 8 && !contains(code, "出发日期  航空公司", true)) {
            if (code.split('\r')[code.split('\r').length - 1].replace(/\s+/g, "") != "") {
                code = code.split('\r')[code.split('\r').length - 1];
            } else if (code.split('\r').length > 1) {
                code = code.split('\r')[code.split('\r').length - 2];
            }
            if (contains(code, "i", true) || contains(code, "av", true) || contains(code, "rt", true) || contains(code, "xe", true) || contains(code, "wf", true) || contains(code, "pn", true) || contains(code, "fd", true) || contains(code, "pg", true) || contains(code, "xo", true) || contains(code, "xi", true) || contains(code, "xc", true) || contains(code, "to", true) || contains(code, "ti", true) || contains(code, "ec", true) || contains(code, "tn", true) || contains(code, "sd", true) || contains(code, "ct", true) || contains(code, "vt", true) || contains(code, "pat", true) || contains(code, "avh", true) || contains(code, "tol", true) || contains(code, "tpr", true) || contains(code, "tsl", true) || contains(code, "rrt", true) || contains(code, "osi", true) || contains(code, "rmk", true) || contains(code, "ssr", true) || contains(code, "tss", true) || contains(code, "trfd", true) || contains(code, "etdz", true) || contains(code, "detr", true)) {
                $("#txaBlack").val($("#txaBlack").val() + code);
                onKey("avh");
            }
        }
    } catch (err) { }
}


/*
*
*string:原始字符串
*substr:子字符串
*isIgnoreCase:忽略大小写
*/
function contains(string, substr, isIgnoreCase) {
    if (isIgnoreCase) {
        string = string.toLowerCase();
        substr = substr.toLowerCase();
    }
    var startChar = substr.substring(0, 1);
    var strLen = substr.length;
    for (var j = 0; j < string.length - strLen + 1; j++) {
        if (string.charAt(j) == startChar)//如果匹配起始字符,开始查找
        {
            if (string.substring(j, j + strLen) == substr)//如果从j开始的字符与str匹配，那ok
            {
                return true;
            }
        }
    }
    return false;
}
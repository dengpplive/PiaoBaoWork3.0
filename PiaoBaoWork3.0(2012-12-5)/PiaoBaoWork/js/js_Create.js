
//一个编码中最大可以预定婴儿数
var ApplayINFCount = 4; //默认4个
//选择的最大乘机人数
var maxPasNum = 9;
//当前选中li的索引
var currentindex = -1;
//显示列表数据的数目
var size = 0;
//列表容器id
var ContainerId = "suggestions";
//当前选中列表的样式名
var SelStyle = "suggec";
//所有常旅客LI项
var FlyerHtml = '';
jQuery(function () {
    jQuery(document).keydown(function (e) {
        var obj = e.srcElement ? e.srcElement : e.target;
        if (e.keyCode == 13) {
            if (obj.id.indexOf("txtPasName_") != -1) {
                return false;
            }
        }
    });
    //最大选择9个人的数组
    initArr(maxPasNum);
    //添加默认数据
    var num = addGroup(null, null, 0);

    //初始化注册验证事件
    jQuery("#txtPasName_" + num).attr("index", num);
    jQuery("#txtPasName_" + num).blur(pNameVate);
    //初始化自动提示控件
    initParam(num);
    //乘机人类型
    jQuery("#SelPasType_" + num).change(PasTypeChange);
    //证件号类型
    jQuery("#SelCardType_" + num).change(CardTypeChange);
    //证件号验证
    jQuery("#txtCardNum_" + num).blur(CardNum);
    jQuery("#txtBirthday_" + num).blur(CardNum);
    //手机号验证
    jQuery("#txtMobile_" + num).blur(Mobile);

    //座位是否有效
    var BigNum = jQuery.trim(jQuery("#Hid_BigNum").val());
    BigNum == ">9" ? "9" : BigNum;
    var bigSpace = parseInt(BigNum, 10);
    if (bigSpace <= 0) {
        jQuery("#btnSub").attr("disabled", true);
    } else {
        jQuery("#btnSub").attr("disabled", false);
    }
    var RoleType = parseInt(jQuery("#Hid_RoleType").val(), 10);
    if (RoleType <= 3) {
        jQuery("input[type='button']").hide();
        alert('您不能访问该页面！');
        history.go(-1);
    }
});
//------------------自动提示框-------------------------------------------------------------------
//重新获取新数据来显示
function showData(input) {
    //清空容器数据
    var suggestions = jQuery("#" + ContainerId).html("");
    //输入文本框
    var _input = jQuery(input);
    //查找到的常旅客
    var data = jQuery(FlyerHtml).find("li[filter^='" + _input.val() + "']");
    //有数据时将数据填充到容器
    // if(jQuery(data).length>0) {
    //列表容器标题    
    var conTitle = '<p  class="pTop">所搜到' + jQuery(data).length + '个结果</p>'; //显示动态结果
    //列表容器底部
    var conBottom = ''; //'<p  class="pButtom">底部</p>'; //可用来分页
    var html = jQuery(conTitle + jQuery("<div></div>").append(jQuery('<ul id="sugUL"></ul>').append(data)).html() + conBottom);
    suggestions.append(html);

    //测试用
    // jQuery("#area").text(suggestions.html());

    //初始索引值
    currentindex = -1;
    //显示容器数据
    suggestions.show();
    jQuery("#" + ContainerId + " li").click(function () {
        //单机li时数据处理                       
        CrClickHand(_input, jQuery(this), false);
        //隐藏
        suggestions.hide();
    });
    //获取li个数
    size = jQuery("#" + ContainerId + " li").size();
    //单击非容器的地方隐藏容器
    document.onclick = function (e) {
        var e = e ? e : window.event;
        var tar = e.srcElement || e.target;
        //不是在容器上单击时不隐藏
        if (tar.id != ContainerId) {
            suggestions.hide();
        }
    }
    // }
    //else {
    //没有数据隐藏容器
    //    suggestions.hide();
    //}
}
//是否上移
function movethis(up, input) {
    var _input = jQuery(input);
    //当前选中li的索引
    currentindex = currentindex + (up ? -1 : 1);
    if (currentindex == size) {
        currentindex = 0;
    }
    else if (currentindex < 0) {
        currentindex = size - 1;
    }
    //设置样式
    jQuery("#" + ContainerId + " li").removeClass(SelStyle);
    jQuery(jQuery("#" + ContainerId + " li")[currentindex]).addClass(SelStyle);

    //取当前选中的文本值
    var liObj = jQuery(jQuery("#" + ContainerId + " li")[currentindex]);
    //处理
    CrClickHand(_input, liObj, true);
}
//文本框处理键盘事件
function inputKeyUp(e) {
    try {
        var obj = e.srcElement ? e.srcElement : e.target;
        var t = e.type;
        if (obj.tagName.toLowerCase() == "input") {
            var _input = jQuery(obj);
            //文本框按键不是上下键 设置过滤内容到li的div容器
            if (e.keyCode != 40 && e.keyCode != 38 && e.keyCode != 13) {
                //文本框值
                var word = jQuery.trim(_input.val());
                if (word) {
                    //获取数据 动态过滤数据            
                    //显示列表数据
                    showData(obj);
                }
                else {
                    //文本框值为空时隐藏
                    jQuery("#" + ContainerId).hide();
                }
            }

            //文本框按键是上下键回车键处理数据
            if (e.keyCode == 38) {
                //上移
                movethis(true, obj);
            }
            else if (e.keyCode == 40) {
                //下移
                movethis(false, obj);
            }
            else if (e.keyCode == 13) {
                //回车
                //当前选中的列表项对象
                var LiObj = jQuery(jQuery("#" + ContainerId + " li")[currentindex]);
                //处理事件                        
                CrClickHand(_input, LiObj, false);
                //最后隐藏列表容器
                jQuery("#" + ContainerId).hide();
            }
        }
    } catch (e) {
    } finally {
    }
}
//文本框聚焦时设置参数
function inputFocus(e) {
    var obj = e.srcElement ? e.srcElement : e.target;
    if (obj.tagName.toLowerCase() == "input") {
        var _input = jQuery(this);
        //设置弹出li的容器div宽度 位置   
        jQuery("#" + ContainerId).css('position', 'absolute').css('left', _input.position().left + 'px').css('top',
    (_input.position().top + _input.height() + 5) + 'px').css('z-index', '100').css(
    'width', '700px').css('maxHeight', '150px').css(
    "overflow-x", "hidden").css("overflow-y", "auto").css("background-color", "white");
        //设置div关联的文本框
        jQuery("#" + ContainerId).attr("index", _input.attr("index"));
    }
}
//处理回车或者列表单击处理函数
function CrClickHand(input, li, isMove) {
    if (isMove) {//鼠标或者键盘移动列表
        //负值到文本框
        input.val(li.attr("filter"));
    } else {
        //处理鼠标单击或者回车列表
        SetData(li, input);
    }
}
//初始化参数
function initParam(num) {
    //数据li的容器div 默认隐藏               
    var suggestions = jQuery("#" + ContainerId);
    if (suggestions.length == 0) {
        suggestions = jQuery('<div></div>');
        suggestions.attr("id", ContainerId);
        jQuery(document.body).append(suggestions);
    }
    suggestions = suggestions.html("").hide();
    //常旅客
    initFlyer();
    /*
    //文本框键盘事件绑定
    jQuery("#txtPasName_"+num).keydown(function (e) {
    if(e.keyCode==13) {
    inputKeyUp(e);
    }
    });
    jQuery("#txtPasName_"+num).keyup(inputKeyUp);
    //文本框聚集事件
    jQuery("#txtPasName_"+num).focus(inputFocus);
    */
    //列表鼠标事件
    jQuery("#" + ContainerId).mouseover(function () {
        //鼠标滑过                     
        jQuery("#" + ContainerId + " li").mouseover(
                function () {
                    //选中列表的样式
                    jQuery(this).addClass(SelStyle);
                    currentindex = jQuery("#" + ContainerId + " li").index(this);
                });
    }).mouseout(function () {
        //鼠标滑出  
        jQuery("#" + ContainerId + " li").removeClass(SelStyle);
    });
}
//初始化常旅客数据
function initFlyer() {
    var flyVal = jQuery.trim(jQuery("#Hid_FlyerList").val());
    if (flyVal != "") {
        var flyList = eval("(" + unescape(flyVal) + ")");
        if (flyList.length > 0) {
            var model = null;
            FlyerHtml = '<ul>';
            for (var i = 0; i < flyList.length; i++) {
                model = flyList[i];
                var newdata = JSON.stringify(model);
                FlyerHtml += '<li   val="' + escape(newdata) + '" filter="' + model._name + '" ><table><tr><td><span style="width:120px;text-align:center;display:block">' + model._name + '</span></td><td><span style="width:150px;text-align:center; display:block">' + GetPasTypeText(model._flyertype) + '</span></td><td><span style="width:150px;text-align:center; display:block">' + model._certificatenum + '</span></td><td><span style="width:150px;text-align:center; display:block">' + model._tel + '</span></td></tr></table></li>';
            }
            FlyerHtml += '</ul>';
        }
    }
}

//设置数据
function SetData(Li, input) {
    if (jQuery.trim(Li.attr("val")) != "") {
        var model = eval("(" + unescape(Li.attr("val")) + ")");
        var num = input.attr("index");
        resetPasMsg(num);
        //设置常旅客
        SetFlyerValue(model, num);
    }
}
//获取乘机人类型
function GetPasTypeText(flyType) {
    var result = "成人";
    if (flyType == "1") {
        result = "成人";
    } else if (flyType == "2") {
        result = "儿童";
    } else if (flyType == "3") {
        result = "婴儿";
    }
    return result;
}
//----------------------------------------------------------------------------------------------
//对话框包含处理
function showdialog(t, f) {
    t = unescape(t);
    jQuery("select").hide();
    jQuery("#divDG").html(t);
    jQuery("#divDG").dialog({
        title: '提示',
        bgiframe: true,
        height: 180,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            if (f == "1") {
                ReductionPage();
            }
            if (f == "2") {
                history.go(-1);
            }
            jQuery("select").show();
        },
        buttons: {
            '确定': function () {
                jQuery(this).dialog('close');
                if (f == "1") {
                    ReductionPage();
                }
                if (f == "2") {
                    history.go(-1);
                }
                if (jQuery("#btnSub").attr("disabled")) {
                    jQuery("#btnSub").attr("disabled", false);
                }
            }
        }
    }).css({ "width": "auto", "height": "auto" });
}
//显示或者隐藏div
function showBgDiv(val) {
    if (val) {
        jQuery("#overlay").show();
        jQuery("#loading").show();
        //禁用或者隐藏按钮
        jQuery("#btnSub").hide();
    } else {
        jQuery("#overlay").hide();
        jQuery("#loading").hide();
        //显示按钮
        jQuery("#btnSub").show()
    }
}
//动态创建div
function DynCreateDiv(id) {
    var div = jQuery("body").append("<div id='" + id + "'></div>")

    //    var div = document.getElementById(id);
    //    if (div == null) {
    //        div = document.createElement("div");
    //        div.id = id;
    //        if (document.all) {
    //            document.body.appendChild(div);
    //        }
    //        else {
    //            document.insertBefore(div, document.body);
    //        }
    //    }
    return div;
}

//显示HTML的对话框
function showHtml(html, w, h) {

    jQuery("select").hide();   
    DynCreateDiv("ddv");
    jQuery("#IFrame").html(html);
    jQuery("#IFrame").dialog({
        title: '选择常旅客',
        bgiframe: true,
        height: h,
        width: w,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            jQuery("select").show();
        }
    });
    //防止出现按钮
    jQuery("#ddv").dialog("option", "buttons", {});
}

function showCondg(html, p) {
    jQuery("select").hide();
    DynCreateDiv("Tip");
    jQuery("#Tip").html(html);
    jQuery("#Tip").dialog({
        title: '提示',
        bgiframe: true,
        height: 150,
        width: 200,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            jQuery("select").show();
        },
        buttons: {
            '是否继续': function () {
                if (p != null) {//继续
                    jQuery(this).dialog('close');
                    //ConfirmContent(p.btn,p.err);
                    jQuery("#btnSub").attr("Second", "1");
                    jQuery("#btnSub").click();
                    jQuery("#btnSub").removeAttr("Second");
                }
            },
            "关闭": function () {
                jQuery(this).dialog('close');
            }
        }
    }).css({ "width": "auto", "height": "auto" });
}
//关闭对话框
function CloseDg() {
    jQuery("#divDG").dialog('close');
}

//扩展新方法
String.prototype.NewReplace = function (sourceData, replaceData) {
    sourceData = sourceData.replace("(", "\\(").replace(")", "\\)");
    var reg = new RegExp(sourceData, "ig");
    var data = this.replace(reg, replaceData);
    return data;
}
function GetStrDate(date) {
    return (date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate());
}
//数组中是否有重复数据
function isRepeat(arr) {
    var hash = {};
    for (var i in arr) {
        if (hash[arr[i]]) {
            return true;
        } else {
            hash[arr[i]] = true;
        }
    }
    return false;
}

//--------------选择人数控件start-------------------------
function dataValite(_num) {
    var IsSuc = false;
    var num = parseInt(_num, 10);
    //最大人数
    if (num > maxPasNum) {
        showdialog(escape("乘机人数不能超过" + maxPasNum + "人，请重新选择！"));
        return IsSuc;
    }

    //可用座位数
    var bigSpace = parseInt(jQuery.trim(jQuery("#Hid_BigNum").val()), 10);
    var TrLen = jQuery("#tab_Pas tbody tr[id*='tr_Pas_']").length;
    if (TrLen > bigSpace) {
        showdialog(escape("该舱位仅余" + bigSpace + "个座位,座位不足！"));
        return IsSuc;
    }


    return IsSuc = true;
}
//初始化全局数组
var gArr = []; //值为1表示序号已使用 0表示未使用
function initArr(num) {
    for (var i = 1; i <= num; i++) {
        eval("var obj={_" + i + ":0}");
        gArr.push(obj);
    }
    return gArr;
}
//重置序号状态
function resetFg(arr, val) {
    for (var i = 0; i < arr.length; i++) {
        for (var m in arr[i]) {
            arr[i][m] = val;
        }
    }
}
//设置序号状态
function setFg(arr, key, val) {
    for (var i = 0; i < arr.length; i++) {
        for (var j in arr[i]) {
            if (j == "_" + key) {
                arr[i][j] = val;
            }
        }
    }
}
//获取最小没有使用的序号
function getMinFg(arr) {
    var index = "0";
    var istrue = false;
    for (var i = 0; i < arr.length; i++) {
        if (istrue) {
            break;
        }
        for (var key in arr[i]) {
            if (arr[i][key] == "0") {
                index = key.replace("_", "");
                istrue = true;
                break;
            }
        }
    }
    return index;
}
//重置错误提示
function resetPasMsg(num) {
    jQuery("#span_PasName_" + num).html('<font class="red">*</font>');
    jQuery("#span_CardNum_" + num).html('<font class="red">*</font>');
    jQuery("#span_Mobile_" + num).html('<font class="red">*</font>');
}
//添加一行
function addGroup(evt, model, fg) {
    var TrCount = -1;
    if (evt != null) {
        var target = evt.srcElement ? evt.srcElement : evt.target;
        jQuery(target).attr("disabled", true);
    }
    //用于序号显示
    var TrLen = jQuery("#tab_Pas tbody tr[id*='tr_Pas_']").length;
    //验证
    if (dataValite((TrLen + 1))) {
        TrCount = getMinFg(gArr);
        //标记为已使用
        setFg(gArr, TrCount, "1");
        //克隆
        var cloneTr = jQuery("#tab_Pas thead tr[id='tr_Pas_0']").clone(true);
        var tr_html = jQuery("<div></div>").append(cloneTr).html();

        //替换id
        tr_html = tr_html.NewReplace("tr_Pas_0", "tr_Pas_" + TrCount).NewReplace("xuhao_0", "xuhao_" + TrCount).NewReplace("txtPasName_0", "txtPasName_" + TrCount);
        tr_html = tr_html.NewReplace("SelPasType_0", "SelPasType_" + TrCount).NewReplace("SelCardType_0", "SelCardType_" + TrCount).NewReplace("txtCardNum_0", "txtCardNum_" + TrCount);
        tr_html = tr_html.NewReplace("txtMobile_0", "txtMobile_" + TrCount).NewReplace("ck_Isflyer_0", "ck_Isflyer_" + TrCount).NewReplace("txtBirthday_0", "txtBirthday_" + TrCount);
        tr_html = tr_html.NewReplace("span_PasName_0", "span_PasName_" + TrCount).NewReplace("span_CardNum_0", "span_CardNum_" + TrCount).NewReplace("span_Mobile_0", "span_Mobile_" + TrCount);
        tr_html = tr_html.NewReplace("msgCardNum_0", "msgCardNum_" + TrCount).NewReplace("msgBirthday_0", "msgBirthday_" + TrCount);
        tr_html = tr_html.NewReplace("flyerremark_0", "flyerremark_" + TrCount); //常旅客备注  
        tr_html = tr_html.NewReplace("chddate_0", "chddate_" + TrCount).NewReplace("txtchlddate_0", "txtchlddate_" + TrCount).NewReplace("style=\"DISPLAY: inline\"", ""); //儿童
        tr_html = tr_html.NewReplace("txtcpyandno_0", "txtcpyandno_" + TrCount)
        //操作按钮 可自行设置哪些按钮
        //var opDiv='<div id="op_div_'+TrCount+'"><a href="#" onclick="return addGroup(event); ">添加</a>&nbsp;<a href="#" onclick="return removeGroup('+TrCount+');">移除</a></div>';
        var opDiv = '<div id="op_div_' + TrCount + '"><input  type="button" value="选择常旅客" onclick="SelectPassenger(' + TrCount + ')" class="btn big1 cp" />' + (fg != 0 ? '<br /><a href="#" onclick="return removeGroup(' + TrCount + ');">移除</a>' : '') + '</div>';
        var newTr = '<tr id="tr_Pas_' + TrCount + '">' + jQuery(tr_html).find("td:last").html(opDiv).parent().html() + '</tr>';
        //添加
        jQuery("#tab_Pas tbody").append(newTr);
        //重置数据
        jQuery("#txtPasName_" + TrCount).val("");

        var options = getOptions("1")
        jQuery("#SelCardType_" + TrCount).html(options);
        jQuery("#SelCardType_" + TrCount + " option:eq(0)").attr("selected", true);
        jQuery("#msgBirthday_" + TrCount).hide();
        jQuery("#msgCardNum_" + TrCount).show();
        jQuery("#txtCardNum_" + TrCount).val("");
        jQuery("#txtMobile_" + TrCount).val("");
        jQuery("#ck_Isflyer_" + TrCount).attr("checked", false)


        //乘客姓名提示
        jQuery("span[id*='span_PasName_" + TrCount + "']").html('<font class="red">*</font>');
        //证件号提示
        jQuery("span[id*='span_CardNum_" + TrCount + "']").html('<font class="red">*</font>');
        //手机号码
        jQuery("span[id*='span_Mobile_" + TrCount + "']").html('<font class="red">*</font>');

        //重置序号
        var nowLen = 0;
        jQuery("#tab_Pas tbody tr[id*='tr_Pas_']").each(function (index, tr) {
            jQuery(tr).find("span[id *='xuhao_']").text(index + 1);
            nowLen++;
        });
        //同步下拉列表
        jQuery("#passengers").val(nowLen);

        //动态添加常旅客
        SetFlyerValue(model, TrCount);

        //注册验证事件             
        jQuery("#txtPasName_" + TrCount).attr("index", TrCount);
        jQuery("#txtPasName_" + TrCount).blur(pNameVate);
        //文本框聚集事件
        jQuery("#txtPasName_" + TrCount).focus(inputFocus);
        jQuery("#txtPasName_" + TrCount).keyup(inputKeyUp);
        //文本框键盘事件绑定
        jQuery("#txtPasName_" + TrCount).keydown(function (e) {
            if (e.keyCode == 13) {
                inputKeyUp(e);
            }
        });
        //乘机人类型
        jQuery("#SelPasType_" + TrCount).change(PasTypeChange);
        //证件号类型
        jQuery("#SelCardType_" + TrCount).change(CardTypeChange);
        //证件号验证
        jQuery("#txtCardNum_" + TrCount).blur(CardNum);
        jQuery("#txtBirthday_" + TrCount).blur(CardNum);
        //手机号验证
        jQuery("#txtMobile_" + TrCount).blur(Mobile);
    }

    if (evt != null) {
        var target = evt.srcElement ? evt.srcElement : evt.target;
        jQuery(target).attr("disabled", false);
    }
    return TrCount;
}

//移除一行
function removeGroup(num) {
    if (num != null) {
        //具体删除
        jQuery('#tab_Pas tbody tr[id="tr_Pas_' + num + '"]').remove();
    } else {
        //从后往前删除
        num = jQuery('#tab_Pas tbody tr:last').attr("id").NewReplace("tr_Pas_", "");
        jQuery('#tab_Pas tbody tr:last').remove();
    }
    //标记为没有使用
    setFg(gArr, num, "0");
    //重置序号
    var nowLen = 0;
    jQuery("#tab_Pas tbody tr[id*='tr_Pas_']").each(function (index, tr) {
        jQuery(tr).find("span[id *='xuhao_']").text(index + 1);
        nowLen++;
    });
    jQuery("#passengers").val(nowLen);
    //移除验证事件

    return false;
}
//选择人数设置
function setShowGroup(_len) {
    var len = parseInt(_len, 10);
    //验证
    if (dataValite(len)) {
        var TrCount = jQuery("#tab_Pas tbody tr[id*='tr_Pas_']").length;
        if (TrCount > len) {
            //删除几条
            var delLen = TrCount - len;
            for (var i = 0; i < delLen; i++) {
                removeGroup(null);
            }
        } else {
            //添加几条
            var AddLen = len - TrCount;
            for (var i = 0; i < AddLen; i++) {
                addGroup(null);
            }
        }
    }
    return false;
}
//--------------选择人数控件End-------------------------
//1成人 2儿童 3婴儿 获取证件类型项
function getOptions(flag) {
    var cardType = unescape(jQuery("#Hid_CardType").val()).split('|');
    var KV;
    var arrOption = [];
    for (var i = 0; i < cardType.length; i++) {
        if (cardType[i] != "" && cardType[i].split('###').length == 2) {
            KV = cardType[i].split('###');
            if (flag == "1") {
                if (jQuery.trim(KV[1]) != "出生日期") {
                    arrOption.push('<option value="' + KV[0] + '">' + KV[1] + '</option>');
                }
            } else if (flag == "2") {
                if (jQuery.trim(KV[1]) != "护照" && jQuery.trim(KV[1]) != "军人证") {
                    arrOption.push('<option value="' + KV[0] + '">' + KV[1] + '</option>');
                }
            } else if (flag == "3") {
                if (jQuery.trim(KV[1]) == "其他有效证件") {
                    arrOption.push('<option value="' + KV[0] + '">' + KV[1] + '</option>');
                }
            }
        }
    }
    return arrOption.join('');
}
//将常旅客显示
function SetFlyerValue(model, num) {
    if (model != null) {
        //乘客姓名
        jQuery("#txtPasName_" + num).val(model._name);
        //乘客类型                 
        jQuery("#SelPasType_" + num).val(model._flyertype);
        //证件类型
        var options = getOptions(model._flyertype)
        jQuery("#SelCardType_" + num).html(options);
        jQuery("#SelCardType_" + num).val(model._certificatetype)
        //证件号
        if (model._flyertype == "3" || (model._flyertype == "2" && model._certificatetype == "5"))//婴儿
        {
            //日期
            jQuery("#msgBirthday_" + num).show();
            jQuery("#msgCardNum_" + num).hide();
            //设置日期
            //var date=eval("new "+model._certificatenum.NewReplace("/","")+"");
            jQuery("#txtBirthday_" + num).val(model._certificatenum);
        } else {
            //证件号
            jQuery("#msgBirthday_" + num).hide();
            jQuery("#msgCardNum_" + num).show();
            jQuery("#txtCardNum_" + num).val(model._certificatenum);
            if (model._certificatetype == 1)//身份证验证
            {
                var errMsg = "";
                if (jQuery.trim(model._certificatenum).length != 15 && jQuery.trim(model._certificatenum).length != 18) {
                    errMsg = "<font class='red'><img src='../img/onError.gif' alt='身份证号位数错误！' title='身份证号位数错误！'></font>";
                } else {
                    var CardObj = IdCardValidate(jQuery.trim(model._certificatenum));
                    if (CardObj != "") {
                        errMsg = "<font class='red'><img src='../img/onError.gif' alt='" + CardObj + "！'  title='" + CardObj + "！'></font>";
                    }
                }
                if (errMsg != "") {
                    jQuery("#span_CardNum_" + num).html(errMsg);
                }
            }
        }
        //儿童出生日期
        if (model._flyertype == "2") {
            //出生日期
            if (model._certificatetype == "5") {
                jQuery("#chddate_" + num).val(model._certificatenum);
                jQuery("#chddate_" + num).hide();
            } else {
                jQuery("#chddate_" + num).show();
            }
        } else {
            jQuery("#chddate_" + num).hide();
        }

        //手机号
        jQuery("#txtMobile_" + num).val(model._tel);
        //设置为常旅客
        jQuery("#ck_Isflyer_" + num).attr("checked", true);
        //常旅客备注
        jQuery("#flyerremark_" + num).val(escape(model._remark));
        //航空公司二字码
        var carrCode = jQuery("#Hid_Carriy").val().split('^')[0].toUpperCase();
        //航空公司卡号
        jQuery("#txtcpyandno_" + num).val(Getcpyandno(carrCode, model._cpyandno));
    }
}
//获取航空公司卡号
function Getcpyandno(carrCode, val) {
    var result = '';
    if (val != null) {
        var strArr = val.toUpperCase().split('|');
        for (var i = 0; i < strArr.length; i++) {
            if (jQuery.trim(strArr[i]) != '') {
                var secArr = jQuery.trim(strArr[i]).split(',');
                if (secArr.length == 2 && secArr[0] == carrCode) {
                    result = secArr[1];
                    break;
                }
            }
        }
    }
    return result;
}
//常旅客获取已经选择的人数
function GetSelModel() {
    var tr = jQuery("#tab_Pas tbody tr");
    var html = "[";
    var PasType = "[";
    var isExist = false;
    tr.each(function (index, tr) {
        var name = jQuery.trim(jQuery(tr).find("input[id*='txtPasName_']").val());
        var pasyype = jQuery.trim(jQuery(tr).find("select[id*='SelPasType_']").val());
        if (name != "") {
            html += "'" + name + "',";
            PasType += pasyype + ",";
            isExist = true;
        }
    });
    if (isExist) {
        html += "]";
        PasType += "]";
    } else {
        html = "[]";
        PasType = "[]";
    }

    return eval("({PNum:" + tr.length + ",List:" + html + ",PasType:" + PasType + "})");
}
//添加一个乘客
function AddOne(model, num) {
    var IsSuc = false;
    SetFlyerValue(model, num);
    closeFlyer();
    IsSuc = true;
    /*
    var tr=jQuery("#tab_Pas tbody tr").not(":hidden");
    //含有人数
    var addLen=tr.length+1;
    if(addLen>9) {
    showdialog(escape("添加人数超过不能超过9人！"));
    return false;
    } else {
    //动态添加常旅客       
        
    var IsSame=false;
    tr.each(function (index,tr) {
    var Name=jQuery(tr).find("input[id*='txtPasName_']").val();
    if(model._name==Name) {
    IsSame=true;
    return false;
    }
    });
    if(!IsSame) {
    addGroup(null,model);
    IsSuc=true;
    } else {
    showdialog(escape("该乘客已添加列表中！"));
    }
        
    }*/
    return IsSuc;
}

//添加常旅客  参数为数组[实体model]
function addPerson(ModelArr) {
    /*
    //移除所有
    jQuery("#tab_Pas tbody tr:gt(2)").remove();
    jQuery("#tab_Pas tbody tr:eq(2)").remove();
    //重置序号状态
    resetFg(gArr,"0");
    */
    //提取页面上已经使用的id序号    
    var useFg = [];
    var pasName = [];
    var tr = jQuery("#tab_Pas tbody tr").not(":hidden");
    //含有人数
    var addLen = tr.length + ModelArr.length;
    if (addLen > maxPasNum) {
        showdialog(escape("添加人数超过不能超过" + maxPasNum + "人！"));
        return false;
    }

    tr.each(function (index, tr) {
        var xuhao = jQuery.trim(jQuery(tr).attr("id")).NewReplace("tr_Pas_", "");
        var Name = jQuery(tr).find("input[id*='txtPasName_']").val();
        useFg.push(xuhao);
        pasName.push(Name);
    });

    if (ModelArr.length > 0) {
        var AdultCount = 0;
        var CHDCount = 0;
        var INFCount = 0;
        //添加
        for (var i = 0; i < ModelArr.length; i++) {
            var model = ModelArr[i];
            if (model._flyertype == "1") {
                AdultCount++;
            } else if (model._flyertype == "2") {
                CHDCount++;
            }
            else if (model._flyertype == "3") {
                INFCount++;
            }
            if (jQuery.inArray(model._name, pasName) == -1) {
                //添加常旅客
                //if(i==0) {
                //   SetFlyerValue(model,i);
                //} else {
                addGroup(null, model);
                //}
            }
        }
        //是否开启儿童必须关联成人订单号 1是 0否
        var CHDOPENAsAdultOrder = jQuery("#Hid_CHDOPENAsAdultOrder").val();
        //只有儿童 显示关联成人订单号
        if (AdultCount == 0 && CHDCount > 0) {
            if (CHDOPENAsAdultOrder == "1") {
                jQuery("#AdultDiv").show();
            } else {
                jQuery("#AdultDiv").hide();
            }
        } else {
            jQuery("#AdultDiv").hide();
        }
    }
}
//名字验证
function pNameVate() {
    var num = jQuery(this).attr("id").split('_')[1];
    //乘客名字验证           
    val = /^[\u4e00-\u9fa5]+$/;
    var txtVal = jQuery.trim(jQuery(this).val());
    if (txtVal == "") {
        jQuery("#span_PasName_" + num).html("<font class='red'><img src='../img/onError.gif' alt='输入名字不能为空！' title='输入名字不能为空！' /></font>");
    } else {
        var RareVal = jQuery.trim(jQuery("#Hid_Rare").val());
        var IsRare = [];
        for (var i = 0; i < RareVal.length; i++) {
            if (txtVal.indexOf(RareVal[i]) != -1) {
                IsRare.push(RareVal[i]);
            }
        }
        if (IsRare.length > 0) {
            jQuery("#span_PasName_" + num).html("<font class='red'><img src='../img/onError.gif' alt='含有生僻字:" + IsRare.join(',') + "请用拼音代替' title='含有生僻字:" + IsRare.join(',') + "请用拼音代替'  /></font>");
        } else {
            if (!val.test(txtVal)) {
                //不全是中文
                if (txtVal[txtVal.length - 1] == "/" || txtVal[0] == "/") {
                    jQuery("#span_PasName_" + num).html("<font class='red'><img src='../img/onError.gif' alt='格式错误' title='格式错误' /></font>");
                } else {
                    //斜杠不能超过1个
                    var xiegang = 1;
                    var mach = txtVal.match(/\//ig);
                    if (mach != null && mach.length > xiegang) {
                        jQuery("#span_PasName_" + num).html("<font class='red'><img src='../img/onError.gif' alt='格式错误' title='格式错误' /></font>");
                    } else {
                        if (txtVal.indexOf("/") <= -1) {
                            //不全是中文且没有斜杠
                            var p = /[\u4e00-\u9fa5]+([A-Za-z|\/]+)/i;
                            if (!p.test(txtVal)) {
                                jQuery("#span_PasName_" + num).html("<font class='red'><img src='../img/onError.gif' alt='格式错误' title='格式错误' /></font>");
                            }
                        } else {
                            jQuery("#span_PasName_" + num).html("<font class='green'>√</font>");
                        }
                    }
                }
            }
            else {
                jQuery("#span_PasName_" + num).html("<font class='green'>√</font>");
            }
        }
    }
}
//乘客类型选择事件
function PasTypeChange() {

    var num = jQuery(this).attr("id").split('_')[1];
    var selVal = jQuery.trim(jQuery(this).val());

    //证件号类型
    var selCardType = jQuery("#tab_Pas tbody  #SelCardType_" + num);
    var selCardTypeVal = selCardType.val();
    //选择的成人 儿童 婴儿个数
    var adultNum = 0, childNum = 0, YNum = 0;
    jQuery("#tab_Pas tbody select[id*='SelPasType_']").each(function (index, obj) {
        var val = jQuery(obj).val();
        if (val == "1") {
            adultNum++;
        } else if (val == "2") {
            childNum++;
        } else if (val == "3") {
            YNum++;
        }
    });
    if (adultNum > 0) {
        //婴儿数目不得大于成人数目
        if (YNum > adultNum) {
            //showdialog(escape("婴儿数目不得大于成人数目！"));
            jQuery(this).find("option:eq(0)").attr("selected", true);
            jQuery(this).trigger("change");
            return false;
        }
        if ((adultNum * 2) < childNum) {
            //showdialog(escape("一个成人只能带两个儿童，请重新选择！"));
            jQuery(this).find("option:eq(0)").attr("selected", true);
            jQuery(this).trigger("change");
            return false;
        }
        jQuery("#AdultDiv").hide();
    } else {
        if (YNum > 0) {
            //showdialog(escape("婴儿必须有一个成人陪伴！"));
            jQuery(this).find("option:eq(0)").attr("selected", true);
            jQuery(this).trigger("change");
            return false;
        }
        if (childNum > 0) {
            jQuery("#AdultDiv").show();
        } else {
            jQuery("#AdultDiv").hide();
        }
    }
    //证件类型option
    var options = getOptions(selVal)
    jQuery("#tab_Pas tbody #SelCardType_" + num).html(options);
    jQuery("#tab_Pas tbody #SelCardType_" + num).val(selCardTypeVal);
    if (selVal == "3") {
        //婴儿
        jQuery("#tab_Pas tbody #msgBirthday_" + num).show();
        jQuery("#tab_Pas tbody #msgCardNum_" + num).hide();

        //隐藏
        jQuery("#tab_Pas tbody #chddate_" + num).hide();
    }
    else {
        if (selVal == "2") //儿童
        {
            //设置默认类型 出生日期
            jQuery("#tab_Pas tbody #SelCardType_" + num + " option:contains('出生日期')").attr("selected", true);
            jQuery("#tab_Pas tbody #msgBirthday_" + num).show();
            jQuery("#tab_Pas tbody #msgCardNum_" + num).hide();
            //隐藏
            jQuery("#tab_Pas tbody #chddate_" + num).hide();
        } else {
            jQuery("#tab_Pas tbody #msgBirthday_" + num).hide();
            jQuery("#tab_Pas tbody #msgCardNum_" + num).show();

            //隐藏
            jQuery("#tab_Pas tbody #chddate_" + num).hide();
        }
    }
}
//证件号类型选择事件
function CardTypeChange() {

    var num = jQuery(this).attr("id").split('_')[1];
    var selVal = jQuery.trim(jQuery(this).val());
    //乘客类型
    var selPasType = jQuery("#tab_Pas tbody #SelPasType_" + num);
    var selPasTypeVal = selPasType.val();
    //其他有效证件   出生日期
    if ((selVal == "4" && (selPasTypeVal == "3" || selVal == "5")) || (selPasTypeVal == "2" && selVal == "5")) {
        //婴儿
        jQuery("#tab_Pas tbody #msgBirthday_" + num).show();
        jQuery("#tab_Pas tbody #msgCardNum_" + num).hide();
    } else {
        jQuery("#tab_Pas tbody #msgBirthday_" + num).hide();
        jQuery("#tab_Pas tbody #msgCardNum_" + num).show();
    }
    if (selPasTypeVal == "2") {
        if (selVal == "5") {
            //隐藏
            jQuery("#tab_Pas tbody #chddate_" + num).hide();
        } else {
            jQuery("#tab_Pas tbody #chddate_" + num).show();
        }
    } else {
        jQuery("#tab_Pas tbody #chddate_" + num).hide();
    }
}

//证件号验证
function CardNum() {

    var id = jQuery(this).attr("id");
    var num = id.split('_')[1];
    var val = jQuery.trim(jQuery(this).val());
    var text = jQuery("#tab_Pas tbody #SelCardType_" + num).find("option:selected").text();
    //乘客类型
    var selPasType = jQuery("#tab_Pas tbody #SelPasType_" + num);
    var selPasTypeVal = selPasType.val();
    //证件号类型
    var selCardType = jQuery("#tab_Pas tbody #SelCardType_" + num);
    var selCardTypeVal = selCardType.val();
    msg = "<font class='green'>√</font>";
    if (val == "") {
        msg = "<font class='red'><img src='../img/onError.gif' alt='证件号不能为空' title='证件号不能为空' /></font>";
    } else {
        if (id.indexOf("txtCardNum_") != -1) {
            if (text.indexOf("身份证") != -1) {
                //验证身份证
                if (jQuery.trim(val).length != 15 && jQuery.trim(val).length != 18) {
                    msg = "<font class='red'><img src='../img/onError.gif' alt='身份证号位数错误'  title='身份证号位数错误' /></font>";
                } else {
                    var CardObj = IdCardValidate(jQuery.trim(val));
                    if (CardObj != "") {
                        msg = "<font class='red'><img src='../img/onError.gif' alt='" + CardObj + "' title='" + CardObj + "'/></font>";
                    }
                }
            }
        } else if (id.indexOf("txtBirthday_") != -1) {
            //验证日期
        }
    }
    //儿童
    if (selPasType == "2") {
        //不为出生日期
        if (selCardType != "5") {
            var ChldDate = jQuery.trim(jQuery("#tab_Pas tbody #txtchlddate_" + num).val());
            if (ChldDate == "") {
                msg = "<font class='red'><img src='../img/onError.gif' alt='儿童出生日期不能为空'  title='儿童证件号不能为空' /></font>";
            }
        }
    }
    jQuery("#tab_Pas tbody #span_CardNum_" + num).html(msg);
}

//手机号验证
function Mobile() {
    /*
    var num=jQuery(this).attr("id").replace("txtMobile_","")
    var patern=/^[0-9]{11}$/i;
    var mobile=jQuery.trim(jQuery(this).val());
    var msg="<font class='green'>√</font>";
    if(mobile=="") {
    msg="<font class='red'><img src='../img/onError.gif' alt='手机号码不能为空' title='手机号码不能为空' /></font>";
    } else {
    if(!patern.test(mobile)) {
    msg="<font class='red'><img src='../img/onError.gif' alt='输入手机号码有误' title='输入手机号码有误' /></font>";
    }
    }
    jQuery("#span_Mobile_"+num).html(msg);
    */
}

//选择常旅客
function SelectPassenger(XuHao) {
    try {
        //有效座位数
        var BigNum = jQuery.trim(jQuery("#Hid_BigNum").val());
        var account = jQuery("#Hid_LoginAccount").val();
        var id = jQuery("#Hid_LoginID").val();
        var url = "FlyerList.aspx?LoginAccount=" + account + "&LoginID=" + id + "&BigNum=" + BigNum + "&FgNum=" + XuHao + "&i=" + Math.random() + "&currentuserid=" + jQuery("#currentuserid").val();
        showHtml("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='yes' allowtransparency='yes' width='100%' style='margin:0px;padding:0px;' height='100%' src='" + SetUrlRandomParameter(url) + "'/>", 750, 400);
    } catch (e) {
        alert(e.message);
    }
}
function closeFlyer() {
    jQuery("#ddv").dialog("close");
}
//不缓存url
function SetUrlRandomParameter(_url) {
    var url;
    if (_url.indexOf("?") > 0) {
        url = _url + "&rand=" + Math.random();
    }
    else {
        url = _url + "?rand=" + Math.random();
    }
    return url;
}
//清除或者重置残留错误验证数据
function ResetErrValid() {
    jQuery("#tab_Pas tbody tr[id*='tr_Pas_']").each(function (index, tr) {
        var PasTr = jQuery(tr);
        //乘客姓名提示
        PasTr.find("span[id*='span_PasName_']").html('<font class="red">*</font>');
        //证件号提示
        PasTr.find("span[id*='span_CardNum_']").html('<font class="red">*</font>');
        //手机号码
        // PasTr.find("span[id*='span_Mobile_']").html('<font class="red">*</font>');
    });
}
//儿童假期是否只能预定O舱
var ChdXZ = false;
//失效时间
var NoValidTime = '2013-09-15 23:59:59';
//按钮验证
function ConfirmContent(btnObj) {
    //清除错误验证数据
    ResetErrValid();
    var IsOK = false;
    try {
        var ispas = jQuery("#btnSub").attr("Second");
        jQuery("#btnSub").attr("disabled", true);

        var validateIsPas = false;
        var pasList = [];
        var pasData = []; //多个乘客数据
        var shengfenzhnghao = []; //身份证号
        var SsrData = []; //证件号
        var errMsg = "";
        //成人 儿童 婴儿个数
        var adultNum = 0, childNum = 0, YNum = 0;
        //航空公司二字码CA
        var carry = jQuery("#Hid_Carriy").val().toUpperCase();
        //舱位A~B
        var Space = jQuery("#Hid_Space").val().toUpperCase();
        //南航儿童是否通过
        var CZ_ChdIsOk = true;
        //验证数据 return false可以跳出each
        jQuery("#tab_Pas tbody tr[id*='tr_Pas_']").each(function (index, tr) {
            var num = jQuery(this).attr("id").replace("tr_Pas_", "");
            //查找数据进行验证
            var PasName = jQuery.trim(jQuery(tr).find('input[id="txtPasName_' + num + '"]').val());
            if (PasName == "") {
                errMsg = "<font class='red'>第" + (index + 1) + "组乘客姓名不能为空！</font>";
                jQuery("#span_PasName_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客姓名不能为空' title='第" + (index + 1) + "组乘客姓名不能为空'/>");
            } else {
                pasList.push(PasName.toUpperCase());
            }
            if (errMsg != "" && ispas == null) return false;
            //乘客类型
            var SelPasType = jQuery(tr).find('select[id="SelPasType_' + num + '"]').val();
            if (SelPasType == "1") {
                adultNum++;
            } else if (SelPasType == "2") {
                childNum++;
            } else if (SelPasType == "3") {
                YNum++;
            }
            //证件号类型
            var SelCardType = jQuery(tr).find('select[id="SelCardType_' + num + '"]').val();
            //证件号
            var CardNum = jQuery.trim(jQuery(tr).find('input[id ="txtCardNum_' + num + '"]').val()); //span_CardNum_0
            var Birthday = jQuery.trim(jQuery(tr).find('input[id ="txtBirthday_' + num + '"]').val()); //span_CardNum_0
            var tempCardNum = CardNum;
            //儿童标识
            var ChldDate = jQuery.trim(jQuery(tr).find('input[id ="txtchlddate_' + num + '"]').val());
            if (SelPasType == "3")//婴儿
            {
                //日期
                if (jQuery.trim(Birthday) == "") {
                    errMsg = "<font class='red'>第" + (index + 1) + "组乘客出生日期不能为空！</font>";
                    jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客出生日期不能为空' title='第" + (index + 1) + "组乘客出生日期不能为空'/>");
                }
                tempCardNum = Birthday;
            }
            else if (SelPasType == "2")//儿童 
            {
                if (SelCardType == "1") {//身份证
                    //身份证是否重复使用
                    shengfenzhnghao.push(jQuery.trim(CardNum));
                    //身份证验证
                    if (jQuery.trim(CardNum) == "") {
                        errMsg = "<font class='red'>第" + (index + 1) + "组乘客证件号不能为空！</font>";
                        jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客证件号不能为空' title='第" + (index + 1) + "组乘客输入证件号不能为空' />");
                        validateIsPas = true;
                    } else {

                        if (jQuery.trim(CardNum).length != 15 && jQuery.trim(CardNum).length != 18) {
                            //errMsg="<font class='red'>*身份证号位数错误！</font>";
                            jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客身份证号位数错误' title='第" + (index + 1) + "组乘客身份证号位数错误'  />");
                            validateIsPas = true;
                        } else {
                            var CardObj = IdCardValidate(jQuery.trim(CardNum));
                            if (CardObj != "") {
                                //errMsg="<font class='red'>* "+CardObj.ErrMsg+"！</font>";
                                jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客 " + CardObj + "' title='第" + (index + 1) + "组乘客 " + CardObj + "' />");
                                validateIsPas = true;
                            }
                        }
                    }
                } else if (SelCardType == "5")//出生日期
                {
                    //日期
                    if (jQuery.trim(Birthday) == "") {
                        errMsg = "<font class='red'>第" + (index + 1) + "组乘客出生日期不能为空！</font>";
                        jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客出生日期不能为空' title='第" + (index + 1) + "组乘客出生日期不能为空' />");
                    }
                    tempCardNum = Birthday;
                    if (Birthday != "") {
                        jQuery(tr).find('input[id ="txtchlddate_' + num + '"]').val(Birthday);
                        ChldDate = Birthday;
                    }
                } else {
                    if (jQuery.trim(CardNum) == "") {
                        errMsg = "<font class='red'>输入证件号不能为空！</font>";
                        jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客证件号不能为空'  title='第" + (index + 1) + "组乘客证件号不能为空' />");
                    }
                }
                /*
                if(SelPasType=="2"&&CZ_ChdIsOk&&(carry.indexOf("CZ")!= -1||carry.indexOf("MU")!= -1)) {
                var datePatern=/^\d{4}-?\d{2}-?\d{2}$/i;
                if(!datePatern.test(tempCardNum)) {
                CZ_ChdIsOk=false;
                }
                }
                */
                //儿童chld
                if (SelCardType != "5" && ChldDate == "") {
                    errMsg = "<font class='red'>儿童出生日期不能为空！</font>";
                    jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客儿童出生日期不能为空'  title='第" + (index + 1) + "组乘客儿童出生日期不能为空' />");
                    return false;
                }
            } else {
                //成人
                if (SelCardType == "1") {//身份证
                    //身份证是否重复使用
                    shengfenzhnghao.push(jQuery.trim(CardNum));
                    //身份证验证
                    if (jQuery.trim(CardNum) == "") {
                        errMsg = "<font class='red'>第" + (index + 1) + "组乘客证件号不能为空！</font>";
                        jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客证件号不能为空'  title='第" + (index + 1) + "组乘客证件号不能为空'/>");
                    } else {
                        if (jQuery.trim(CardNum).length != 15 && jQuery.trim(CardNum).length != 18) {
                            errMsg = "<font class='red'>第" + (index + 1) + "组乘客身份证号位数错误！</font>";
                            jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客身份证号位数错误' title='第" + (index + 1) + "组乘客身份证号位数错误'   />");
                            validateIsPas = true;
                        } else {

                            var CardObj = IdCardValidate(jQuery.trim(CardNum));
                            if (CardObj != "") {
                                errMsg = "<font class='red'>第" + (index + 1) + "组乘客 " + CardObj + "！</font>";
                                jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客 " + CardObj + "'  title=' 第" + (index + 1) + "组乘客" + CardObj + "' />");
                                validateIsPas = true;
                            }

                        }
                    }

                } else {
                    if (jQuery.trim(CardNum) == "") {
                        errMsg = "<font class='red'>第" + (index + 1) + "组乘客证件号不能为空！</font>";
                        jQuery("#span_CardNum_" + num).html("<img src='../img/onError.gif' alt='第" + (index + 1) + "组乘客证件号不能为空' title='第" + (index + 1) + "组乘客证件号不能为空' />");
                    }
                }
            }
            if (errMsg != "" && ispas == null) return false;
            //手机号
            var Mobile = jQuery.trim(jQuery(tr).find('input[id="txtMobile_' + num + '"]').val());
            /*
            if(Mobile=="") {
            errMsg="<font class='red'>第"+(index+1)+"组乘客手机号不能为空！</font>";
            jQuery("#span_Mobile_"+num).html("<img src='../img/onError.gif' alt='第"+(index+1)+"组乘客手机号不能为空' title='第"+(index+1)+"组乘客手机号不能为空' />");
            } else {
            var patern=/^[0-9]{11}$/i;
            if(!patern.test(Mobile)) {
            errMsg="<font class='red'>第"+(index)+"组乘客手机号位数错误！</font>";
            jQuery("#span_Mobile_"+num).html("<img src='../img/onError.gif' alt='第"+(index+1)+"组乘客手机号位数错误' title='第"+(index+1)+"组乘客手机号位数错误'/>");
            validateIsPas=true;
            }
            }
            */
            if (errMsg != "" && ispas == null) return false;
            //是否常旅客
            var Isflyer = jQuery(tr).find('input[id="ck_Isflyer_' + num + '"]').is(":checked");
            //常旅客备注
            var flyerRemark = jQuery(tr).find('input[type="hidden"][id="flyerremark_' + num + '"]').val();

            //航空公司卡号 111
            var cpyandno = jQuery(tr).find('input[id="txtcpyandno_' + num + '"]').val();

            //-------------处理传入后台数据----------------------
            //当前乘客数据
            var temppasData = [];
            //序号
            temppasData.push(index);
            //乘客姓名
            temppasData.push(PasName);
            //乘客类型
            temppasData.push(SelPasType);
            //证件号类型                
            temppasData.push(SelCardType);
            //证件号码
            temppasData.push(tempCardNum);
            //证件号
            SsrData.push(tempCardNum);
            //乘客手机
            temppasData.push(Mobile);
            //是否常旅客 1是 0否
            temppasData.push(Isflyer ? "1" : "0");
            //常旅客备注
            temppasData.push(flyerRemark);
            //儿童标识 Chld 日期型2013-06-30
            temppasData.push(ChldDate);
            //航空公司卡号
            temppasData.push(cpyandno);
            //扩展其他。。
            //...
            //加入
            pasData.push(temppasData);
            //-----------------------------------
        });

        if (errMsg != "" && ispas == null) {
            if (validateIsPas) {
                //弹出是否继续对话框
                showCondg(errMsg, { btn: btnObj, err: 1 });
                return false;
            } else {
                showdialog(escape(errMsg));
                return IsOK;
            }
        }
        //乘客不能为空
        if (pasList == null || pasList.length == 0) {
            showdialog(escape("乘机人不能为空！"));
            return IsOK;
        }
        //当前服务器时间
        var serCurrTime = jQuery("#Hid_CurrTime").val();
        //暑期儿童EU只能预定o舱 在2013-09-15之前判断有效
        if (ChdXZ && !CompareDate(serCurrTime, NoValidTime)) {
            //儿童个数大于0
            if (childNum > 0 && carry.indexOf("EU") != -1 && Space.indexOf("O") == -1) {
                showdialog(escape("EU航空儿童只能预定O舱！"));
                return IsOK;
            }
        }

        //检查乘机人名字重复
        //if(isRepeat(pasList)) {
        //    showdialog(escape("输入乘机人不能重复！"));
        //    return IsOK;
        //}
        //检查乘机人身份证是否重复
        //if(isRepeat(shengfenzhnghao)) {
        //    showdialog(escape("输入乘机人身份证号不能重复！"));
        //    return IsOK;
        //}

        //证件号是否重复
        if (isRepeat(SsrData)) {
            showdialog(escape("输入乘机人证件号不能重复！"));
            return IsOK;
        }


        //编码中只能备注几个婴儿
        if (YNum > ApplayINFCount) {
            showdialog(escape("最多只能输入" + ApplayINFCount + "个婴儿！"));
            return IsOK;
        }


        //检查符合条件的人数
        if (adultNum > 0) {
            //婴儿数目不得大于成人数目
            if (YNum > adultNum) {
                showdialog(escape("婴儿数目不得大于成人数目！"));
                return IsOK;
            }
            if ((adultNum * 2) < childNum) {
                showdialog(escape("一个成人只能带两个儿童，请重新选择！"));
                return IsOK;
            }
        } else {
            if (YNum > 0) {
                showdialog(escape("婴儿必须有一个成人陪伴！"));
                return IsOK;
            }
        }
        //南航儿童证件号验证
        if (!CZ_ChdIsOk) {
            //showdialog(escape("南航(CZ)或者东航(MU)儿童证件类型必须为出生日期,请更改证件类型后再预订!"));
            showdialog(escape("儿童证件类型必须为出生日期,请更改证件类型后再预订!"));
            return IsOK;
        }
        //验证JD航空           
        if (carry.indexOf("JD") != -1) {
            var IsNext = false;
            var b1 = adultNum == 1 && childNum == 0 && YNum == 0;
            var b2 = adultNum == 0 && childNum == 1 && YNum == 0;
            var b3 = adultNum == 1 && childNum == 1 && YNum == 0;
            var b4 = adultNum == 1 && childNum == 1 && YNum == 1;
            var b5 = adultNum == 1 && childNum == 0 && YNum == 1;
            if (b1 || b2 || b3 || b4 || b5) {
                IsNext = true;
            }
            if (!IsNext) {
                showdialog(escape("JD航空订座时限制最多只能预订一个成人,一个儿童,一个婴儿，请分开预订!"));
                return IsOK;
            }
        }
        var LinkName = jQuery.trim(jQuery("#txtLinkName").val());
        var LinkMobile = jQuery.trim(jQuery("#txtMobile").val())
        if (LinkName == "") {
            jQuery("#spanName").html("<font class='red'>* 联系人姓名不能为空！</font>");
            showdialog(escape("联系人姓名不能为空！"));
            return IsOK;
        }
        if (LinkMobile == "") {
            jQuery("#spanMobile").html("<font class='red'>* 手机不能为空！</font>");
            showdialog(escape("联系人手机不能为空！"));
            return IsOK;
        } else {
            var patern = /^[0-9]{11}$/i;
            if (!patern.test(LinkMobile)) {
                jQuery("#spanMobile").html("<font class='red'>* 联系人手机号码输入格式错误！</font>");
                showdialog(escape("联系人手机号码输入格式错误！"));
                return IsOK;
            }
        }
        //开启儿童编码必须关联成人编码或者成人订单号 1是 0否
        var CHDOPENAsAdultOrder = jQuery("#Hid_CHDOPENAsAdultOrder").val();
        if (CHDOPENAsAdultOrder == "1") {
            jQuery("#Hid_IsAsAdultOrder").val("0");
            jQuery("#AdultDiv").hide();
        } else {
            jQuery("#Hid_IsAsAdultOrder").val("1");
        }

        //关联成人订单号是否隐藏
        var IsHidden = jQuery("#AdultDiv").is(":hidden");
        if (!IsHidden) {
            var AdultOrder = jQuery.trim(jQuery("#txtAdultOrder").val());
            if (AdultOrder == "" && CHDOPENAsAdultOrder == "1") {
                showdialog(escape("关联成人订单号不能为空！"));
                return IsOK;
            }
        }
        //是否关联成人订单号 0不关联 1关联
        jQuery("#Hid_IsAsAdultOrder").val(IsHidden ? "0" : "1");

        //有乘客数据时
        if (pasData.length > 0) {
            //提交通过//------------------------
            jQuery("#Hid_Global").val(escape(" gArr=([" + Serializable(gArr) + "])"));
            jQuery("#Hid_ViewState").val(escape(jQuery("#div_ViewState").html()));
            //显示遮罩
            showBgDiv(true);
            IsOK = true;
            var pData = "";
            for (var i = 0; i < pasData.length; i++) {
                pData += pasData[i].join("#####") + "@";
            }
            //设置乘机人数据信息
            jQuery("#Hid_PasData").val(pData);
        } else {
            showdialog(escape("请选择乘客人数！"));
            //隐藏遮罩层
            showBgDiv(false);
            return IsOK;
        }
    } catch (e) {
        alert(e.message);
        //隐藏遮罩层
        showBgDiv(false);
    } finally {
        jQuery("#btnSub").attr("disabled", false);
    }
    return IsOK;
}
function OK(btnBbj, evt) {
    var b = ConfirmContent(btnBbj);
    return b;
}
function Serializable(arr) {
    var p = [];
    for (var i = 0; i < arr.length; i++) {
        for (var m in arr[i]) {
            p.push("{" + m + ":" + arr[i][m] + "}");
        }
    }
    return p.join(",");
}

//还原页面状态
function ReductionPage() {
    eval(unescape(jQuery("#Hid_Global").val()));
    jQuery("#div_ViewState").html(unescape(jQuery("#Hid_ViewState").val()));
}
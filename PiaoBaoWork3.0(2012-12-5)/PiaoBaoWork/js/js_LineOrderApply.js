var maxSky=9;//最大航段数
var maxPasNum=9;//最大乘机人数
//重新加载城市控件
function ReLoad(type) {
    var rd="?r="+Math.random();
    var SE=new CtripJsLoader();
    var files=null;
    if(type=="0") {
        //国内 
        files=[["../AJAX/GetCity.aspx"+rd,"GB2312",true,null],["../js/CitySelect/tuna_100324.js"+rd,"GB2312",true,null]];
    } else if(type=="1") {//国际
        files=[["../AJAX/GetCity.aspx"+rd+"&IsGJ=1","GB2312",true,null],["../js/CitySelect/tuna_100324.js"+rd+"&IsGJ=1"+rd,"GB2312",true,null]];
    }
    SE.scriptAll(files);
}



//显示隐藏遮罩层 true 显示 false隐藏 【可选参数】text:为遮罩中的提示文字
function ShowDiv(flag,text) {
    if(flag) {
        jQueryOne("#overlay").show();
        jQueryOne("#loading").show();
        if(text!=null) {
            jQueryOne("#span_text").html(text);
        } else {
            jQueryOne("#span_text").html('请稍等，正在处理PNR数据<br />');
        }
    } else {
        jQueryOne("#overlay").hide();
        jQueryOne("#loading").hide();
    }
}
//对话框包含处理
function showdialog(t,f) {
    jQueryOne("select").hide();
    jQueryOne("#show").html(t);
    jQueryOne("#show").dialog({
        title: '提示',
        bgiframe: true,
        height: 180,
        modal: true,
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        close: function () {
            jQueryOne("select").show();
        },
        buttons: {
            '确定': function (evt) {
                if(f!=null&&f==1) {
                    location.href='LineOrderList.aspx?go=1&currentuserid='+jQueryOne("#currentuserid").val();
                } else {
                    var target=evt.srcElement?evt.srcElement:evt.target;
                    jQueryOne(target).attr("disabled",true);
                    jQueryOne(this).dialog('close');
                }
            }
        }
    });
}
//扩展新方法
String.prototype.NewReplace=function (sourceData,replaceData) {
    sourceData=sourceData.replace("(","\\(").replace(")","\\)");
    var reg=new RegExp(sourceData,"ig");
    var data=this.replace(reg,replaceData);
    return data;
}
//padLeft(10,'0')
String.prototype.padLeft=function (length,char) {
    var d=this;
    var len=d.length;
    while(len<length) {
        d=char+d;
        len++;
    }
    return d;
}
//padRight(10,'0')
String.prototype.padRight=function (length,char) {
    var d=this;
    var len=d.length;
    while(len<length) {
        d=d+char;
        len++;
    }
    return d;
}
//在字符串指定位置插入字符串
String.prototype.InsertChar=function (index,char) {
    var d=this;
    var reg=new RegExp("(.{"+index+"})","");
    d=d.replace(reg,"$1"+char);
    return d;
}

function GetStrDate(date,fg) {
    var d1=(date.getFullYear()+"-"+(date.getMonth()+1).toString().padLeft(2,'0')+"-"+date.getDate().toString().padLeft(2,'0'));
    if(fg) {
        d1=(date.getFullYear()+"-"+(date.getMonth()+1).toString().padLeft(2,'0')+"-"+date.getDate().toString().padLeft(2,'0')+" "+date.getHours().toString().padLeft(2,'0')+":"+date.getMinutes().toString().padLeft(2,'0'));
    }
    return d1;
}
//数组中是否有重复数据
function isRepeat(arr) {
    var hash={};
    for(var i in arr) {
        if(hash[arr[i]]) {
            return true;
        } else {
            hash[arr[i]]=true;
        }
    }
    return false;
}
//获取城市信息
function GetCityInfo(code) {
    var reModel=null;
    var CityList=eval("("+unescape(jQueryOne("#Hid_CityData").val())+")");
    for(var i=0;i<CityList.length;i++) {
        var model=CityList[i];
        if(jQueryOne.trim(model._citycodeword).toUpperCase()==jQueryOne.trim(code).toUpperCase()||jQueryOne.trim(model._cityname).toUpperCase()==jQueryOne.trim(code).toUpperCase()) {
            reModel=model;
            break;
        }
    }
    return reModel;
}
//获取航空公司信息
function GetAirInfo(code) {
    var reModel=null;
    var AirList=eval("("+unescape(jQueryOne("#Hid_AirData").val())+")");
    for(var i=0;i<AirList.length;i++) {
        var model=AirList[i];
        if(jQueryOne.trim(model._code).toUpperCase()==jQueryOne.trim(code).toUpperCase()||jQueryOne.trim(model._shortname).toUpperCase()==jQueryOne.trim(code).toUpperCase()) {
            reModel=model;
            break;
        }
    }
    return reModel;
}

//start------------------------------------------------------------------------------------------------------
//1成人 2儿童 3婴儿 获取证件类型项
function getOptions(flag) {
    var cardType=unescape(jQueryOne("#Hid_CardType").val()).split('|');
    var KV;
    var arrOption=[];
    for(var i=0;i<cardType.length;i++) {
        if(cardType[i]!=""&&cardType[i].split('###').length==2) {
            KV=cardType[i].split('###');
            if(flag=="1") {
                if(jQueryOne.trim(KV[1])!="出生日期") {
                    arrOption.push('<option value="'+KV[0]+'">'+KV[1]+'</option>');
                }
            } else if(flag=="2") {
                if(jQueryOne.trim(KV[1])!="护照"&&jQueryOne.trim(KV[1])!="军人证") {
                    arrOption.push('<option value="'+KV[0]+'">'+KV[1]+'</option>');
                }
            } else if(flag=="3") {
                if(jQueryOne.trim(KV[1])=="其他有效证件") {
                    arrOption.push('<option value="'+KV[0]+'">'+KV[1]+'</option>');
                }
            }
        }
    }
    return arrOption.join('');
}
//初始化全局数组
var gArr=[];//值为1表示序号已使用 0表示未使用
function initArr(num) {
    for(var i=1;i<=num;i++) {
        eval("var obj={_"+i+":0}");
        gArr.push(obj);
    }
    return gArr;
}
//重置序号状态
function resetFg(arr,val) {
    for(var i=0;i<arr.length;i++) {
        for(var m in arr[i]) {
            arr[i][m]=val;
        }
    }
}
//设置序号状态
function setFg(arr,key,val) {
    for(var i=0;i<arr.length;i++) {
        for(var j in arr[i]) {
            if(j=="_"+key) {
                arr[i][j]=val;
            }
        }
    }
}
//获取最小没有使用的序号
function getMinFg(arr) {
    var index="0";
    var istrue=false;
    for(var i=0;i<arr.length;i++) {
        if(istrue) {
            break;
        }
        for(var key in arr[i]) {
            if(arr[i][key]=="0") {
                index=key.replace("_","");
                istrue=true;
                break;
            }
        }
    }
    return index;
}
//添加一行乘客
function addGroup(evt,model) {
    if(evt!=null) {
        var target=evt.srcElement?evt.srcElement:evt.target;
        jQueryOne(target).attr("disabled",true);
    }
    //用于序号显示
    var TrLen=jQueryOne("#tab_Pas tbody tr[id*='tr_Pas_']").length;
    //验证
    if(dataValite((TrLen+1))) {
        var TrCount=getMinFg(gArr);
        //标记为已使用
        setFg(gArr,TrCount,"1");
        //克隆
        var cloneTr=jQueryOne("#tab_Pas thead tr[id='tr_Pas_0']").clone(true);
        var tr_html=jQueryOne("<div></div>").append(cloneTr).html();
        //替换id
        tr_html=tr_html.NewReplace("tr_Pas_0","tr_Pas_"+TrCount).NewReplace("xuhao_0","xuhao_"+TrCount).NewReplace("txtPasName_0","txtPasName_"+TrCount);
        tr_html=tr_html.NewReplace("SelPasType_0","SelPasType_"+TrCount).NewReplace("SelCardType_0","SelCardType_"+TrCount).NewReplace("txtCardNum_0","txtCardNum_"+TrCount);
        tr_html=tr_html.NewReplace("txtMobile_0","txtMobile_"+TrCount).NewReplace("ck_Isflyer_0","ck_Isflyer_"+TrCount).NewReplace("txtBirthday_0","txtBirthday_"+TrCount);
        tr_html=tr_html.NewReplace("span_PasName_0","span_PasName_"+TrCount).NewReplace("span_CardNum_0","span_CardNum_"+TrCount).NewReplace("span_Mobile_0","span_Mobile_"+TrCount);
        tr_html=tr_html.NewReplace("msgcardid_0","msgcardid_"+TrCount).NewReplace("msgPasName_0","msgPasName_"+TrCount);
        tr_html=tr_html.NewReplace("msgCardNum_0","msgCardNum_"+TrCount).NewReplace("txtCardNum_0","txtCardNum_"+TrCount).NewReplace("msgBirthday_0","msgBirthday_"+TrCount).NewReplace("txtBirthday_0","txtBirthday_"+TrCount);
        tr_html=tr_html.NewReplace("txtPhone_0","txtPhone_"+TrCount).NewReplace("spanPhone_0","spanPhone_"+TrCount);
        tr_html=tr_html.NewReplace("txtNewSeat_0","txtNewSeat_"+TrCount).NewReplace("flyerremark_0","flyerremark_"+TrCount);//常旅客备注
        //操作按钮 可自行设置哪些按钮 msgPasName_0
        //var opDiv='<div id="op_div_'+TrCount+'"><a href="#" onclick="addGroup(event); ">添加</a>&nbsp;<a href="#" onclick="return removeGroup('+TrCount+');">移除</a></div>';
        var opDiv='<div id="op_div_'+TrCount+'"><span  class="btn btn-ok-s" ><input id="btnFind" type="button" value="选择常旅客" onclick="SelectPassenger('+TrCount+')"  /></span>&nbsp;<span  class="btn btn-ok-s" ><input  type="button" value="移除" onclick="return removeGroup('+TrCount+');"  /></span></div>';

        var newTr='<tr id="tr_Pas_'+TrCount+'">'+jQueryOne(tr_html).find("td:last").html(opDiv).parent().html()+'</tr>';
        //添加
        jQueryOne("#tab_Pas tbody").append(newTr);
        //重置数据
        jQueryOne("#txtPasName_"+TrCount).val("");
        jQueryOne("#msgPasName_"+TrCount).html("<font class='red'>*</font>");
        jQueryOne("#msgcardid_"+TrCount).html("<font class='red'>* </font>");
        var options=getOptions("1")
        //如果实体值不为空
        if(model!=null&&model._PassengerType!="") {
            var PassengerType=model._PassengerType;
            options=getOptions(PassengerType);
        }
        jQueryOne("#SelCardType_"+TrCount).html(options);
        if(model!=null&&model._PassengerType=="2") {
            jQueryOne("#SelCardType_"+TrCount+" option:contains('出生日期')").attr("selected",true);
        }

        //注册验证事件
        jQueryOne("#txtPasName_"+TrCount).attr("index",TrCount);
        jQueryOne("#txtPasName_"+TrCount).blur(pNameVate);
        jQueryOne("#txtPasName_"+TrCount).keyup(inputKeyUp);
        //文本框聚集事件
        jQueryOne("#txtPasName_"+TrCount).focus(inputFocus);
        //文本框键盘事件绑定
        jQueryOne("#txtPasName_"+TrCount).keydown(function (e) {
            if(e.keyCode==13) {
                inputKeyUp(e);
            }
        });

        jQueryOne("#SelCardType_"+TrCount+" option:eq(0)").attr("selected",true);
        jQueryOne("#msgBirthday_"+TrCount).hide();
        jQueryOne("#msgCardNum_"+TrCount).show();
        jQueryOne("#txtCardNum_"+TrCount).val("");
        jQueryOne("#txtPhone_"+TrCount).val("");
        //jQueryOne("#spanPhone_"+TrCount).html("<font class='red'>* </font>");
        //重置序号
        var nowLen=0;
        jQueryOne("#tab_Pas tbody tr[id*='tr_Pas_']").each(function (index,tr) {
            jQueryOne(tr).find("span[id *='xuhao_']").text(index+1);
            nowLen++;
        });
        //序号
        jQueryOne("#xuhao_"+TrCount).html(nowLen);
        //同步下拉列表
        jQueryOne("#passengers").val(nowLen);



        //乘机人类型
        jQueryOne("#SelPasType_"+TrCount).change(PasTypeChange);
        //证件号类型
        jQueryOne("#SelCardType_"+TrCount).change(CardTypeChange);
        //证件号验证
        jQueryOne("#txtCardNum_"+TrCount).blur(CardNum);
        jQueryOne("#SelCardType_"+TrCount).blur(CardNum);
        //手机号
        jQueryOne("#txtPhone_"+TrCount).blur(Mobile);
    }

    if(evt!=null) {
        var target=evt.srcElement?evt.srcElement:evt.target;
        jQueryOne(target).attr("disabled",false);
    }
    return TrCount;
}

//移除一行
function removeGroup(num) {
    if(num!=null) {
        //具体删除
        jQueryOne('#tab_Pas tbody tr[id="tr_Pas_'+num+'"]').remove();
    } else {
        //从后往前删除
        var lastTr=jQueryOne('#tab_Pas tbody tr:last');
        if(lastTr.length>0) {
            num=lastTr.attr("id").NewReplace("tr_Pas_","");
            lastTr.remove();
        } else {
            showdialog("没有可删除的乘机人项！");
            return false;
        }
    }
    //标记为没有使用
    setFg(gArr,num,"0");
    //重置序号
    var nowLen=0;
    jQueryOne("#tab_Pas tbody tr[id*='tr_Pas_']").each(function (index,tr) {
        jQueryOne(tr).find("span[id *='xuhao_']").text(index+1);
        nowLen++;
    });
    jQueryOne("#passengers").val(nowLen);
    //移除验证事件

    return false;
}
//选择人数设置
function setShowGroup(_len) {
    var len=parseInt(_len,10);
    //验证
    if(dataValite(len)) {
        var TrCount=jQueryOne("#tab_Pas tbody tr[id*='tr_Pas_']").length;
        if(TrCount>len) {
            //删除几条
            var delLen=TrCount-len;
            for(var i=0;i<delLen;i++) {
                removeGroup(null);
            }
        } else {
            //添加几条
            var AddLen=len-TrCount;
            for(var i=0;i<AddLen;i++) {
                addGroup(null);
            }
        }
    }
    return false;
}
//end------------------------------------------------------------------------------------------------------
//是否跳到登录页面
function IsGoLoginPage(data) {
    var isGo=false;
    if(data.indexOf('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">')!= -1) {
        isGo=true;
    }
    return isGo;
}
//航空公司选择
function GetCode(text,val,sel,FN) {
    //设置航空公司
    jQueryOne("#Hid_AirCode").val(text);
    if(jQueryOne.trim(val)!="") {
        //获取基础仓位     
        jQueryOne.post("../AJAX/CommonAjAx.ashx",
        {
            OpFunction: "PTSpace",
            AirCode: val,
            num: Math.random(),
            currentuserid: jQueryOne("#currentuserid").val()
        },
        function (data) {
            ShowDiv(false);
            //判断网页是否失效
            var isgo=IsGoLoginPage(data);
            if(isgo) {
                showdialog("网页已失效请重新登录！");
                return;
            }
            try {
                if(data!="") {
                    var strReArr=data.split('##');
                    if(strReArr.length==3) {
                        var errCode=strReArr[0];
                        var errDes=strReArr[1];
                        var result=unescape(strReArr[2]);
                        if(result!="") {
                            //返回结果
                            var objList=eval("("+result+")");
                            var options="";
                            for(var i=0;i<objList.length;i++) {
                                options+='<option value="'+objList[i]._cabin+'">'+objList[i]._cabin+'</option>';
                            }
                            jQueryOne("select[name='myspace']").html(options);
                        }
                    }
                }
            } catch(e) {
                alert(e.message);
            }
            if(FN!=null) {
                FN();
            }
        },"text");
    }
}
//机型选择
function GetPlaneType(text,val,sel) {
    jQueryOne("#Hid_PlaneType").val(text);
    if(jQueryOne.trim(val)!="") {
        jQueryOne("#Hid_innerFare").val(val.split('-')[0]);
    }
}
//----乘客验证------------------------------------------------
//最大乘客
function dataValite(_num) {
    var IsSuc=false;
    var num=parseInt(_num,10);
    //最大人数
    if(num>maxPasNum) {
        showdialog("乘机人数不能超过"+maxPasNum+"人，请重新选择！");
        return IsSuc;
    }
    return IsSuc=true;
}
//乘客名字
function pNameVate() {
    var num=jQueryOne(this).attr("id").split('_')[1];
    var PasName=jQueryOne.trim(jQueryOne(this).val());
    if(PasName=="") {
        jQueryOne("#msgPasName_"+num).html("<img src='../img/onError.gif' alt='乘客姓名不能为空！' title='乘客姓名不能为空！'>");
        //showdialog("乘客姓名不能为空！");
    } else {
        jQueryOne("#msgPasName_"+num).html("<font class='green'>√</font>");
    }
}
function showINF(fg,num) {
    if(fg) {
        jQueryOne("#msgCardNum_"+num).hide();
        jQueryOne("#msgBirthday_"+num).show();
    } else {
        jQueryOne("#msgCardNum_"+num).show();
        jQueryOne("#msgBirthday_"+num).hide();
    }
}
//乘机人类型
function PasTypeChange() {

    var num=jQueryOne(this).attr("id").split('_')[1];
    var PasType=jQueryOne.trim(jQueryOne(this).val());
    var options=getOptions(PasType);
    jQueryOne("#SelCardType_"+num).html(options);
    if(PasType=="3"||PasType=="2") {
        showINF(true,num);
        jQueryOne("#msgcardid_"+num).html("<font class='red'>*</font>");
        if(PasType=="2") {
            jQueryOne("#SelCardType_"+num+" option:contains('出生日期')").attr("selected",true);
        }
    } else {
        showINF(false,num);
    }
}
//证件类型
function CardTypeChange() {
    var num=jQueryOne(this).attr("id").split('_')[1];
    var CardType=jQueryOne.trim(jQueryOne(this).val());
    var text=jQueryOne(this).find("option[value='"+CardType+"']").text();
    if(text.indexOf("出生日期")!= -1) {
        showINF(true,num);
        jQueryOne("#msgcardid_"+num).html("<font class='red'>*</font>");
    } else {
        showINF(false,num);
    }
}
//证件号码
function CardNum() {

    var target=event.srcElement?event.srcElement:event.target;
    var aid=jQueryOne(target).attr("id");
    var bid=jQueryOne(this).attr("id");
    var num=bid.split('_')[1];

    var selPasType=jQueryOne("select[id*='SelPasType_"+num+"'] option:selected");
    var SelPasType=selPasType.val().toUpperCase();
    var SelPasTypeText=jQueryOne.trim(selPasType.text().toUpperCase());

    var selCardType=jQueryOne("select[id*='SelCardType_"+num+"'] option:selected");
    var SelCardType=selCardType.val().toUpperCase();
    var SelCardTypeText=jQueryOne.trim(selCardType.text().toUpperCase());

    var cardId=jQueryOne("#txtCardNum_"+num).val();
    if(!jQueryOne("#msgBirthday_"+num).is(":hidden")) {
        cardId=jQueryOne("#txtBirthday_"+num).val();
    }
    if(SelPasTypeText=="婴儿"||(SelPasTypeText=="儿童"&&SelCardTypeText=="出生日期")) {
        cardId=jQueryOne("#txtBirthday_"+num).val();
    }
    jQueryOne("#msgcardid_"+num).html("<font class='green'>√</font>");
    if(aid.indexOf("txtCardNum_")!= -1) {
        if(jQueryOne.trim(cardId)=="") {
            //showdialog("证件号不能为空！");
            jQueryOne("#msgcardid_"+num).html("<img src='../img/onError.gif' alt='证件号不能为空！' title='证件号不能为空！'>");
        } else {
            if(SelCardTypeText=="身份证") {
                if(jQueryOne.trim(cardId).length!=15&&jQueryOne.trim(cardId).length!=18) {
                    errMsg="<font class='red'>*身份证号位数错误！</font>";
                    jQueryOne("#msgcardid_"+num).html("<img src='../img/onError.gif' alt='身份证号位数错误！' title='身份证号位数错误！'>");
                } else {
                    var CardObj=IdCardValidate(jQueryOne.trim(cardId));
                    if(CardObj!="") {
                        errMsg="<font class='red'>* "+CardObj+"！</font>";
                        jQueryOne("#msgcardid_"+num).html("<img src='../img/onError.gif' alt='"+CardObj+"！' title='"+CardObj+"！'>");
                    }
                }
            }
        }
    }
}
//手机号验证
function Mobile() {
    /*
    var num=jQueryOne(this).attr("id").replace("txtPhone_","")
    var patern=/^[0-9]{11}$/i;
    var mobile=jQueryOne.trim(jQueryOne(this).val());
    var msg="<font class='green'>√</font>";
    if(mobile=="") {
    msg="<img src='../img/onError.gif' alt='手机号码不能为空！' title='手机号码不能为空！'>";//"<font class='red'>* 手机号码不能为空！</font>";
    } else {
    if(!patern.test(mobile)) {
    msg="<img src='../img/onError.gif' alt='输入手机号码有误！' title='输入手机号码有误！'>";//"<font class='red'>*输入手机号码有误！</font>";
    }
    }
    jQueryOne("#spanPhone_"+num).html(msg);
    */
}

//----------航段----------------------------------------------------------------------------

//初始化全局数组
var skyArr=[];//值为1表示序号已使用 0表示未使用
function initSkyArr(num) {
    for(var i=1;i<=num;i++) {
        eval("var obj1={_"+i+":0}");
        skyArr.push(obj1);
    }
    return skyArr;
}
//城市选择
function ddlSetText(ddlObj,flag,num) {
    var ddlVal=jQueryOne.trim(jQueryOne(ddlObj).val()).split('-')[0].toUpperCase();
    jQueryOne("#"+flag+"_"+num).val(ddlVal);
}
function txtSetSel(txtObj,flag,num) {
    var txtVal=jQueryOne(txtObj).val().toUpperCase();
    if(txtVal!="") {
        jQueryOne("#"+flag+"_"+num+" option[value*='"+txtVal+"']").attr("selected",true);
    } else {
        jQueryOne("#"+flag+"_"+num+" option").eq(0).attr("selected",true);
    }
}
//添加一条航段信息
function addSkyGroup(evt) {
    var num=0;
    var trObj=jQueryOne('#tblsky tbody tr[id="trsky_0"]');
    var trLen=jQueryOne("#tblsky tbody tr[id*='trsky_']").length;
    if(trLen<maxSky) {
        if(evt!=null) {
            var target=evt.srcElement?evt.srcElement:evt.target;
            jQueryOne(target).attr("disabled",true);
        }
        //获取可用标号       
        num=getMinFg(skyArr);
        //标记为已使用
        setFg(skyArr,num,"1");
        //克隆
        var NewTr=jQueryOne(trObj).clone(true);
        var html=jQueryOne(NewTr).html();
        //替换id
        html=html.NewReplace("txtFromCity_0","txtFromCity_"+num).NewReplace("txtSetSel(this,'ddlFromCity',0)","txtSetSel(this,'ddlFromCity',"+num+")").NewReplace("ddlFromCity_0","ddlFromCity_"+num).NewReplace("ddlSetText(this,'txtFromCity',0)","ddlSetText(this,'txtFromCity',"+num+")");
        html=html.NewReplace("txtToCity_0","txtToCity_"+num).NewReplace("txtSetSel(this,'ddlToCity',0)","txtSetSel(this,'ddlToCity',"+num+")").NewReplace("ddlToCity_0","ddlToCity_"+num).NewReplace("ddlSetText(this,'txtToCity',0)","ddlSetText(this,'txtToCity',"+num+")");
        html=html.NewReplace("startdate_0","startdate_"+num).NewReplace("flight_0","flight_"+num).NewReplace("selSpace_0","selSpace_"+num)
        html=html.NewReplace("spansky_0","spansky_"+num).NewReplace("enddate_0","enddate_"+num);
        //---
        html=html.NewReplace("ddlFromCity_0","ddlFromCity_"+num).NewReplace("hidfromcode_0","hidfromcode_"+num).NewReplace("ddlToCity_0","ddlToCity_"+num).NewReplace("hidtocode_0","hidtocode_"+num);
        html=html.NewReplace("ddlFromCityGJ_0","ddlFromCityGJ_"+num).NewReplace("hidfromcodeGJ_0","hidfromcodeGJ_"+num).NewReplace("cityType_0","cityType_"+num).NewReplace("cityTypeSel1_0","cityTypeSel1_"+num);



        var opdiv='<div id="skydiv_'+num+'"><a href="#" onclick="addSkyGroup(event); ">添加</a>&nbsp;<a href="#" onclick="return removeSkyGroup(event,'+num+');">移除</a></div>';
        html="<tr id='trsky_"+num+"'>"+jQueryOne("<tr>"+html+"</tr>").find("td:last").html(opdiv).parent().html()+"</tr>";
        jQueryOne("#tblsky").append(html);
        if(evt!=null) {
            jQueryOne(target).attr("disabled",false);
        }

        //重置序号        
        jQueryOne("#tblsky tbody tr[id*='trsky_']").each(function (index,tr) {
            jQueryOne(tr).find("span[id *='spansky_']").text((index+1));
        });
        //恢复初始数据
        jQueryOne("#ddlFromCity_"+num).val("");
        jQueryOne("#hidfromcode_"+num).val("");
        jQueryOne("#ddlToCity_"+num).val("");
        jQueryOne("#hidtocode_"+num).val("");
        jQueryOne("#flight_"+num).val("");
        var date=new Date();
        jQueryOne("#startdate_"+num).val(GetStrDate(date,true));

        //重新加载城市控件
        //出发城市 选择国内还是国际                     
        var val=jQueryOne("input[type='radio'][name*='cityType_']:checked").val();
        if(val=="0")//国内
        {
            ReLoad("0");
        } else if(val=="1")//国际
        {
            ReLoad("1");
        }
    } else {
        showdialog('最多添加'+maxSky+'组航段信息');
    }
    return num;
}

//移除一条航段信息
function removeSkyGroup(evt,num) {
    if(num!=null) {
        jQueryOne("#tblsky tbody tr[id*='trsky_"+num+"']").remove();
    } else {
        var len=jQueryOne("#tblsky tbody tr[id*='trsky_']").length;
        if(len>1) {
            var last=jQueryOne("#tblsky tbody tr[id*='trsky_']").last();
            num=last[0].id.NewReplace("trsky_","");
            last.remove();
        } else {
            showdialog('该航段信息不可删除！');
            return false;
        }
    }
    //标记为没有使用
    setFg(skyArr,num,"0");
    //重置序号        
    jQueryOne("#tblsky tbody tr[id*='trsky_']").each(function (index,tr) {
        jQueryOne(tr).find("span[id *='spansky_']").text((index+1));
    });
    return false;
}
//选择人数设置
function setSkyGroup(_len) {
    var len=parseInt(_len,10);
    //验证
    if(dataValite(len)) {
        var TrCount=jQueryOne("#tblsky tbody tr[id*='trsky_']").length;
        if(TrCount>len) {
            //删除几条
            var delLen=TrCount-len;
            for(var i=0;i<delLen;i++) {
                removeSkyGroup(null);
            }
        } else {
            //添加几条
            var AddLen=len-TrCount;
            for(var i=0;i<AddLen;i++) {
                addSkyGroup(null);
            }
        }
    }
    return false;
}


//申请
function Apply(btnObj) {
    var IsSuc=false;
    //选择方式0手动 1pnr 2pnr内容
    var val_OrderfangShi=jQueryOne("#Hid_OrderfangShi").val();
    var val_Pnr="";
    if(val_OrderfangShi!=0) {
        val_Pnr=jQueryOne("#Hid_Pnr").val();
    }
    var UninCode=jQueryOne.trim(jQueryOne("#Hid_UninCode").val().toUpperCase());
    /*
    var carry=jQueryOne.trim(jQueryOne("#Hid_AirCode").val().toUpperCase());
    var carrArr=carry.split('-');
    if(carrArr.length!=2) {
    showdialog('请选择航空公司！');
    return IsSuc;
    }*/
    var airCode="";//carrArr[0];
    var airName="";//carrArr[1];
    //机型 暂不验证机型
    var PlaneType="";//jQueryOne.trim(jQueryOne("#Hid_PlaneType").val());
    /*
    if(PlaneType=="") {
    showdialog('请选择机型，机型不能为空！');
    return IsSuc;
    }*/
    //基建
    var JJFare=jQueryOne.trim(jQueryOne("#Hid_innerFare").val());
    if(jQueryOne.isNaN(JJFare)||JJFare=="") {
        JJFare="0";
    }
    //默认出票公司编号
    var CPCpyNo=jQueryOne.trim(jQueryOne("#Hid_CPCpyNo").val());
    //联系人
    var linkMan=jQueryOne.trim(jQueryOne("#linkName").val());
    //联系手机
    var linkPhone=jQueryOne.trim(jQueryOne("#linkPhone").val());
    //备注
    var Remark=jQueryOne.trim(jQueryOne("#linkRemark").val());
    var pnrType=jQueryOne("input[name='pnrType']")
    var skyStr="",pasStr="",TipMsg="";
    var skyArr=[],pasArr=[];
    //航段
    jQueryOne("#tblsky tbody tr[id*='trsky_']").each(function (index,tr) {
        trObj=jQueryOne(tr);
        var skyone=[];

        //航班号CA1475
        var flight=trObj.find("input[id*='flight_']").val().toUpperCase().NewReplace('#####','');
        if(flight.length<=2) {
            TipMsg="航段"+(index+1)+"，"+"输入航班号有误，正确格式如:CA7384";
            return false;
        }
        //航空公司二字码
        airCode=flight.substring(0,2);
        //航班号
        flight=flight.substring(2);
        var AirInfo=GetAirInfo(airCode);
        if(AirInfo==null) {
            TipMsg="航段"+(index+1)+"，"+"输入航班号有误，正确格式如:CA7384";
            return false;
        }
        //航空公司简称
        airName=AirInfo._shortname;
        var fromcity='',fromcode='';
        var tocity='',tocode='';
        //隐藏域取值
        fromcode=trObj.find("input[id*='hidfromcode_'][type='hidden']").val().toUpperCase();
        tocode=trObj.find("input[id*='hidtocode_'][type='hidden']").val().toUpperCase();

        if(fromcode==""||tocode=="") {
            TipMsg="航段"+(index+1)+"，"+"请选择出发城市和到达城市！";
            return false;
        } else {
            if(fromcode.toUpperCase()==tocode.toUpperCase()) {
                TipMsg="航段"+(index+1)+"，"+"出发城市和到达城市不能相同！";
                return false;
            }
        }
        //城市实体信息  
        var fromModel=GetCityInfo(fromcode);
        if(fromModel==null) {
            TipMsg="航段"+(index+1)+"，"+"出发城市"+fromcode+"未找到！";
            return false;
        }
        var toModel=GetCityInfo(tocode);
        if(toModel==null) {
            TipMsg="航段"+(index+1)+"，"+"到达城市"+tocode+"未找到！";
            return false;
        }
        fromcity=fromModel._cityname;
        tocity=toModel._cityname;
        //出发日期
        var startdate=jQueryOne.trim(trObj.find("input[id*='startdate_']").val().NewReplace('#####',''));
        //到达日期
        var enddate=jQueryOne.trim(trObj.find("input[id*='enddate_']").val().NewReplace('#####',''));


        //var selSpace=trObj.find("select[id*='selSpace_'] option:selected").val().toUpperCase();
        var selSpace=trObj.find("input[id*='txtNewSeat_']").val().toUpperCase();

        if(jQueryOne.trim(startdate)=="") {
            TipMsg="航段"+(index+1)+"，"+"起飞日期不能为空！";
            return false;
        }
        if(CompareDate(startdate,enddate)||startdate==enddate) {
            TipMsg="航段"+(index+1)+"，出发日期必须小于到达日期";
            return false;
        }
        if(jQueryOne.trim(flight)=="") {
            TipMsg="航段"+(index+1)+"，航班号格式错误或者不能为空！";
            return false;
        }

        if(selSpace.length<=0) {
            TipMsg="航段"+(index+1)+"，"+"请输入航空公司舱位！";
            return false;
        }


        skyone.push((index+1));//序号
        skyone.push(startdate);//出发日期
        skyone.push(enddate);//到达日期
        skyone.push(fromcity);//出发城市名称
        skyone.push(fromcode);//出发城市二字码
        skyone.push(tocity);//到大城市名称
        skyone.push(tocode);//到大城市二字码
        skyone.push(airName);//航空公司简称
        skyone.push(airCode);//航空公司二字码
        skyone.push(flight);//航班号
        skyone.push(selSpace);//舱位
        skyone.push(PlaneType);//机型
        skyone.push(JJFare);//基建
        //加入
        skyArr.push(skyone.join('#####'));
    });
    if(TipMsg!="") {
        showdialog(TipMsg);
        return IsSuc;
    }
    if(skyArr.length>0) {
        skyStr=skyArr.join('@');
    }
    //证件号
    var SsrArr=[];
    //乘客姓名
    var PasNameArr=[];
    var pasList=jQueryOne("#tab_Pas tbody tr[id*='tr_Pas_']");
    if(pasList.length==0) {
        showdialog('请添加乘机人项！');
        return IsSuc;
    }
    //乘机人
    pasList.each(function (index,tr) {
        var trObj=jQueryOne(tr);
        var pasArrOne=[];

        var txtPasName=jQueryOne.trim(trObj.find("input[id*='txtPasName_']").val().toUpperCase()).NewReplace('#####','');
        var selPasType=trObj.find("select[id*='SelPasType_'] option:selected");
        var SelPasType=selPasType.val().toUpperCase();
        var SelPasTypeText=jQueryOne.trim(selPasType.text().toUpperCase());

        var selCardType=trObj.find("select[id*='SelCardType_'] option:selected");
        var SelCardType=selCardType.val().toUpperCase();
        var SelCardTypeText=jQueryOne.trim(selCardType.text().toUpperCase());

        var CardVal=jQueryOne.trim(trObj.find("input[id*='txtCardNum_']").val());
        if(SelPasTypeText=="婴儿"||(SelPasTypeText=="儿童"&&SelCardTypeText=="出生日期")) {
            CardVal=jQueryOne.trim(trObj.find("input[id*='txtBirthday_']").val());
        }
        CardVal=CardVal.NewReplace('#####','');

        //手机号
        var phoneNum=jQueryOne.trim(trObj.find("input[id*='txtPhone_']").val());
        if(txtPasName=="") {
            TipMsg="<font class='red'>乘客姓名不能为空！</font>";
            trObj.find("span[id*='msgPasName_']").html("<img src='../img/onError.gif' alt='乘客姓名不能为空！' title='乘客姓名不能为空！'>");
            return false;
        } else {
            trObj.find("span[id*='msgPasName_']").html("<font class='green'>√</font>");
        }
        if(CardVal=="") {
            TipMsg="<font class='red'>*证件号不能为空！</font>";
            trObj.find("span[id*='msgcardid_']").html("<img src='../img/onError.gif' alt='证件号不能为空！' title='证件号不能为空！'>");
            return false;
        } else {
            trObj.find("span[id*='msgcardid_']").html("<font class='green'>√</font>");
        }
        if(SelCardTypeText=="身份证") {
            if(jQueryOne.trim(CardVal).length!=15&&jQueryOne.trim(CardVal).length!=18) {
                TipMsg="<font class='red'>*身份证号位数错误！</font>";
                trObj.find("span[id*='msgcardid_']").html("<img src='../img/onError.gif' alt='身份证号位数错误！' title='身份证号位数错误！'>");
                return false;
            } else {
                trObj.find("span[id*='msgcardid_']").html("<font class='green'>√</font>");
                var CardObj=IdCardValidate(jQueryOne.trim(CardVal));
                if(CardObj!="") {
                    //TipMsg="<font class='red'>* "+CardObj+"！</font>";
                    trObj.find("span[id*='msgcardid_']").html("<img src='../img/onError.gif' alt='"+CardObj+"！' title='"+CardObj+"！'>");
                    //return false;
                } else {
                    trObj.find("span[id*='msgcardid_']").html("<font class='green'>√</font>");
                }
            }
        }
        /*
        //验证手机号
        var patern=/^[0-9]{11}$/i;
        if(!patern.test(phoneNum)) {
        TipMsg="<img src='../img/onError.gif' alt='输入手机号格式错误！' title='输入手机号格式错误！'>";//"<font class='red'> *输入手机号格式错误！</font>";
        trObj.find("span[id*='spanPhone_']").html(TipMsg);
        return false;
        } else {
        trObj.find("span[id*='spanPhone_']").html("<font class='green'>√</font>");
        }
        */
        //常旅客备注
        var flyerremark=jQueryOne.trim(trObj.find("input[type='hidden'][id*='flyerremark_']").val());

        pasArrOne.push((index+1));
        pasArrOne.push(txtPasName);
        pasArrOne.push(SelPasType);
        pasArrOne.push(SelCardType);
        pasArrOne.push(CardVal);
        pasArrOne.push(phoneNum);//手机号              
        pasArrOne.push("0"); //是否为常旅客
        pasArrOne.push(flyerremark);//常旅客备注

        //乘客姓名
        PasNameArr.push(txtPasName)
        //乘客证件号
        SsrArr.push(CardVal);
        //最终使用的数据
        pasArr.push(pasArrOne.join('#####'));
    });
    if(TipMsg!="") {
        showdialog(TipMsg);
        return IsSuc;
    }
    if(pasArr.length>0) {
        pasStr=pasArr.join('@');
    }
    if(linkMan=="") {
        showdialog('联系人姓名不能为空！');
        return IsSuc;
    }
    if(linkPhone=="") {
        showdialog('联系人手机不能为空！');
        return IsSuc;
    }
    if(isRepeat(SsrArr)) {
        showdialog('输入乘客证件号不能重复！');
        return IsSuc;
    }
    //    if(isRepeat(PasNameArr)) {
    //        showdialog("输入乘客姓名不能重复！");
    //        return IsSuc;
    //    }
    var url="../AJAX/CommonAjAx.ashx";
    var val_OpFunction="lineOrder";//线下订单
    var val_OpType="1";//添加
    var val_OpPage="LineOrderApply.aspx";
    var param={
        SkyStr: escape(skyStr),
        PasStr: escape(pasStr),
        Remark: escape(Remark),
        linkMan: escape(linkMan),
        linkPhone: escape(linkPhone),
        PlaneType: escape(PlaneType),
        JJFare: escape(JJFare),
        airCode: escape(airCode),
        airName: escape(airName),
        UninCode: escape(UninCode),
        CPCpyNo: escape(CPCpyNo),
        Pnr: escape(val_Pnr),
        OrderfangShi: escape(val_OrderfangShi),
        OpFunction: escape(val_OpFunction),
        OpType: escape(val_OpType),
        OpPage: escape(val_OpPage),
        num: Math.random(),
        currentuserid: jQueryOne("#currentuserid").val()
    };
    jQueryOne(btnObj).attr("disabled",true);
    //显示遮罩
    var tipText="请稍等，正在申请订单数据<br />";
    ShowDiv(true,tipText);
    jQueryOne.post(url,param,function Handle(data) {
        jQueryOne(btnObj).attr("disabled",false);
        //关闭遮罩
        ShowDiv(false);
        //处理数据
        var strReArr=data.split('##');
        if(strReArr.length==3) {
            //错误代码
            var errCode=strReArr[0];
            //错误描述
            var errDes=strReArr[1];
            //错误结果
            var result=jQueryOne.trim(unescape(strReArr[2]));
            if(errCode=="1") {
                if(errDes!="") {
                    if(errDes.indexOf("订单号")!= -1) {
                        showdialog(errDes,1);
                    } else {
                        //处理成功
                        showdialog(errDes);
                    }
                } else {
                    showdialog("生成成功",1);
                }
            } else {
                showdialog(errDes);
            }
        }
    },"text");
    return IsSuc;
}
//PNR导入
function ApplayPnr(obj) {
    try {
        var val_OrderfangShi=jQueryOne("#Hid_OrderfangShi").val();
        var val_LoginName=jQueryOne("#Hid_LoginName").val();
        var val_CpyNo=jQueryOne("#Hid_UninCode").val();
        var errMsg='';
        var Pnr="";
        var PnrCon='';
        if(val_OrderfangShi=="1") {
            var pnrPatern=/^[A-Za-z0-9]{6}$/i;
            //Pnr
            Pnr=jQueryOne.trim(jQueryOne("#txtPNR").val());
            if(Pnr=="") {
                errMsg="输入编码不能为空！";
            } else if(Pnr.length!=6) {
                errMsg="输入编码长度错误！";
            }
            else if(!pnrPatern.test(Pnr)||Pnr=="ooooo"||Pnr=="oooooo") {
                errMsg="输入编码格式有误！";
            }
        } else if(val_OrderfangShi=="2") {
            //PNR内容
            PnrCon=jQueryOne.trim(jQueryOne("#txtPNRCon").val());
            if(PnrCon=="") {
                errMsg="输入PNR内容不能为空！";
            }
        }
        if(errMsg!=null&&errMsg!="") {
            showdialog(errMsg);
        } else {
            //---------------------
            ShowDiv(true);
            var url="../AJAX/PnrImport.ashx";
            var param=
            {
                PNR: escape(Pnr),
                PNRCon: escape(PnrCon),
                OrderFangShi: escape(val_OrderfangShi),
                LoginName: val_LoginName,
                CpyNo: escape(val_CpyNo),
                OPPage: "GetPnrInfo",
                num: Math.random(),
                currentuserid: jQueryOne("#currentuserid").val()
            };
            jQueryOne("#Hid_Pnr").val(Pnr);
            jQueryOne(obj).attr("diabled",true);
            //请求
            jQueryOne.post(url,param,function (data) {
                ShowDiv(false);
                jQueryOne(obj).attr("diabled",false);
                var reArr=data.split('@@@####');
                if(reArr!=null&&reArr.length==2) {
                    var status=reArr[0];
                    var result=reArr[1];
                    if(status=="0") {
                        if(result==""||result==null) {
                            result="申请失败";
                        }
                        showdialog(result);
                    } else if(status=="1") {
                        var PnrModel=eval("("+result+")");
                        if(jQueryOne.trim(PnrModel._ErrMsg)!="") {
                            showdialog("解析失败，原因如下:<br />"+PnrModel._ErrMsg);
                        } else {
                            PnrToOrder(PnrModel);
                        }
                    }
                } else {
                    showdialog("页面出错");
                }
            },"text");
        }
    } catch(e) {
        alert(e.message);
    }
}
//Pnr 和Pnr内容 实体处理
function PnrToOrder(pnrModel) {
    try {
        //清空
        // jQueryOne("#tblsky tbody tr[id*='trsky_']:gt(0)").remove();
        // jQueryOne("#tab_Pas tbody tr[id*='tr_Pas_']:gt(0)").remove();

        var pasLen=jQueryOne("#tab_Pas tbody tr").length;
        //设置乘客数
        jQueryOne("#passengers").val(pnrModel._PassengerList.length);

        //乘机人集合
        var _PassengerList=pnrModel._PassengerList;
        for(var i=0;i<_PassengerList.length;i++) {
            if(i==0&&pasLen>0) {
                SetModel(1,_PassengerList[i],1);
            } else {
                var lineNum=addGroup(null,_PassengerList[i]);
                SetModel(1,_PassengerList[i],lineNum);
            }
        }

        //匿名函数
        // var fn=function () {
        //航段集合           
        var _LegList=pnrModel._LegList;
        for(var i=0;i<_LegList.length;i++) {
            if(i==0) {
                SetModel(0,_LegList[i],i);
            } else {
                var lineNum=addSkyGroup(null);
                SetModel(0,_LegList[i],lineNum);
            }
        }
        // ShowDiv(false);
        // }

        //设置航空公司
        var CarrCode=pnrModel._CarryCode;
        if(jQueryOne.trim(CarrCode)!="") {
            var AirModel=GetAirInfo(CarrCode);
            if(AirModel==null) {
                showdialog("未能解析出航空公司二字码");
            } else {
                var text=AirModel._code+"-"+AirModel._shortname;
                jQueryOne("#Hid_AirCode").val(text);
            }
            //设置编码
            jQueryOne("#Hid_Pnr").val(pnrModel._Pnr);

            // jQueryOne("#SelAir_txt_AirCo_SelAir").val(CarrCode);
            // ShowDiv(true);
            //选择航空公司
            // GetCode(text,CarrCode,null,fn);
        } else {
            showdialog("未能解析出航空公司二字码");
        }
    } catch(e) {
        alert(e.message);
    }
}


//通过实体设置的界面值 type=0 航段实体 type=1乘客实体 num为序号
function SetModel(type,Model,num) {

    if(type==0) {
        //设置航段数据
        //出发城市ddlFromCity_0
        var FromCityInfo=GetCityInfo(Model._FromCode);
        jQueryOne("#ddlFromCity_"+num).val(FromCityInfo._cityname);
        jQueryOne("#hidfromcode_"+num).val(FromCityInfo._citycodeword);
        //到达城市
        var ToCityInfo=GetCityInfo(Model._ToCode);
        jQueryOne("#ddlToCity_"+num).val(ToCityInfo._cityname);
        jQueryOne("#hidtocode_"+num).val(ToCityInfo._citycodeword);
        //出发日期
        var fromDate=Model._FlyDate1+" "+Model._FlyStartTime.InsertChar(2,":");
        jQueryOne("#startdate_"+num).val(fromDate);
        //到达日期
        var toDate=Model._FlyDateE+" "+Model._FlyEndTime.InsertChar(2,":");
        jQueryOne("#enddate_"+num).val(toDate);
        //航班号
        jQueryOne("#flight_"+num).val(Model._AirCodeFlightNum);//Model._FlightNum 
        //舱位
        jQueryOne("#txtNewSeat_"+num).val(Model._Seat.toUpperCase());
        //jQueryOne("select[id='selSpace_"+num+"'][name='myspace']").val(Model._Seat.toUpperCase());
    } else if(type==1) {
        //设置乘客数据
        //jQueryOne("#xuhao_"+num).html((num+1));
        //乘客姓名
        jQueryOne("#txtPasName_"+num).val(Model._PassengerName);
        //乘客类型
        jQueryOne("#SelPasType_"+num).val(Model._PassengerType);
        var pastype=Model._PassengerType;
        var cardTypeHTML=getOptions(pastype);
        jQueryOne("#SelCardType_"+num).html(cardTypeHTML);

        var ssrType='身份证';
        if(Model._PassengerType!="3") {
            var ssrPatern=/(\d{4}\-\d{2}\-\d{2}){10}|(\d{4}\d{2}\d{2}){8}/;
            if(ssrPatern.test(Model._SsrCardID)) {
                ssrType='出生日期';
                jQueryOne("#msgBirthday_"+num).show();
                jQueryOne("#msgCardNum_"+num).hide();

            }
        } else {
            ssrType='其他有效证件';
            jQueryOne("#msgBirthday_"+num).show();
            jQueryOne("#msgCardNum_"+num).hide();
        }
        //证件类型
        jQueryOne("#SelCardType_"+num+" option").each(function (index,op) {
            var text=jQueryOne(op).text();
            if(text==ssrType) {
                jQueryOne(op).attr("selected",true);
                return false;
            }
        });

        //证件号码
        if(ssrType=='出生日期') {
            jQueryOne("#txtBirthday_"+num).val(Model._SsrCardID);
        }
        if(!jQueryOne("#msgBirthday_"+num).is(":hidden")) {
            jQueryOne("#txtBirthday_"+num).val(Model._SsrCardID);
        } else {
            jQueryOne("#txtCardNum_"+num).val(Model._SsrCardID);
        }
        //儿童编码处理
        if(Model._PassengerType=="2") {
            if(/\d{4}\-\d{2}\-\d{2}/.test(Model._SsrCardID)) {
                jQueryOne("#msgBirthday_"+num).show();
                jQueryOne("#msgCardNum_"+num).hide();
                jQueryOne("#SelCardType_"+num+" option:contains('出生日期')").attr("selected",true);
            }
        }
    }
}

//选择订单生成方式
function SelPnrType(obj) {
    var val=jQueryOne(obj).val();
    if(val=="0") {//0手动录入
        jQueryOne("#tr_pnr").hide();
        jQueryOne("#tr_PnrCon").hide();
        jQueryOne("#OrderSpan").hide();
    } else if(val=="1") {//1PNR录入
        jQueryOne("#tr_pnr").show();
        jQueryOne("#tr_PnrCon").hide();
        jQueryOne("#OrderSpan").show();
    }
    else if(val=="2") {//1PNR内容录入
        jQueryOne("#tr_pnr").hide();
        jQueryOne("#tr_PnrCon").show();
        jQueryOne("#OrderSpan").show();
    }
    jQueryOne("#Hid_OrderfangShi").val(val);
}
//--------------------------------------------------------------
//选择常旅客
function SelectPassenger(XuHao) {
    //有效座位数
    var BigNum=9;
    var account=jQueryOne("#Hid_LoginAccount").val();
    var id=jQueryOne("#Hid_LoginID").val();
    var url="../Buy/FlyerList.aspx?LoginAccount="+account+"&LoginID="+id+"&BigNum="+BigNum+"&FgNum="+XuHao+"&i="+Math.random()+"&currentuserid="+jQueryOne("#currentuserid").val();
    showHtml("<iframe frameborder='no' border='0' marginwidth='0' marginheight='0' scrolling='yes' allowtransparency='yes' width='100%' style='margin:0px;padding:0px;' height='100%' src='"+SetUrlRandomParameter(url)+"'/>",750,400);
}
//动态创建div
function DynCreateDiv(id) {
    var div=document.getElementById(id);
    if(div==null) {
        div=document.createElement("div");
        div.id=id;
        if(document.all) {
            document.body.appendChild(div);
        }
        else {
            document.insertBefore(div,document.body);
        }
    }
    return div;
}
//显示HTML的对话框
function showHtml(html,w,h) {
    jQueryOne("select").hide();
    DynCreateDiv("ddv");
    jQueryOne("#ddv").html(html);
    jQueryOne("#ddv").dialog({
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
            jQueryOne("select").show();
        }
    });
    //防止出现按钮
    jQueryOne("#ddv").dialog("option","buttons",{});
}
function closeFlyer() {
    jQueryOne("#ddv").dialog("close");
}
//不缓存url
function SetUrlRandomParameter(_url) {
    var url;
    if(_url.indexOf("?")>0) {
        url=_url+"&rand="+Math.random();
    }
    else {
        url=_url+"?rand="+Math.random();
    }
    return url;
}
//常旅客获取已经选择的人数
function GetSelModel() {
    var tr=jQueryOne("#tab_Pas tbody tr");
    var html="[";
    var PasType="[";
    var isExist=false;
    tr.each(function (index,tr) {
        var name=jQueryOne.trim(jQueryOne(tr).find("input[id*='txtPasName_']").val());
        var pasyype=jQueryOne.trim(jQueryOne(tr).find("select[id*='SelPasType_']").val());
        if(name!="") {
            html+="'"+name+"',";
            PasType+=pasyype+",";
            isExist=true;
        }
    });
    if(isExist) {
        html+="]";
        PasType+="]";
    } else {
        html="[]";
        PasType="[]";
    }

    return eval("({PNum:"+tr.length+",List:"+html+",PasType:"+PasType+"})");
}
//添加一个乘客
function AddOne(model,num) {
    var IsSuc=false;
    var tr=jQueryOne("#tab_Pas tbody tr").not(":hidden");
    //含有人数
    var addLen=tr.length;
    if(addLen>maxPasNum) {
        showdialog("添加人数超过不能超过"+maxPasNum+"人！");
        return false;
    } else {
        //动态添加常旅客
        setFlyer(model,num);
        closeFlyer();
        IsSuc=true;
        /*
        var IsSame=false;
        tr.each(function (index,tr) {
        var Name=jQueryOne(tr).find("input[id*='txtPasName_']").val();
        if(model._name==Name) {
        IsSame=true;
        return false;
        }
        });
        if(!IsSame) {
        var num=addGroup(null);
        setFlyer(model,num);
        IsSuc=true;
        } else {
        showdialog("该乘客已添加列表中！");
        }
        */
    }
    return IsSuc;
}
//------------------自动提示框-------------------------------------------------------------------
//当前选中li的索引
var currentindex= -1;
//显示列表数据的数目
var size=0;
//列表容器id
var ContainerId="suggestions";
//当前选中列表的样式名
var SelStyle="suggec";
//所有常旅客LI项
var FlyerHtml='';
//重新获取新数据来显示
function showData(input) {
    //清空容器数据
    var suggestions=jQueryOne("#"+ContainerId).html("");
    //输入文本框
    var _input=jQueryOne(input);
    //查找到的常旅客
    var data=jQueryOne(FlyerHtml).find("li[filter^='"+_input.val()+"']");
    //有数据时将数据填充到容器
    // if(jQueryOne(data).length>0) {
    //列表容器标题
    var conTitle='<p  class="pTop">所搜到'+jQueryOne(data).length+'个结果</p>'; //显示动态结果
    //列表容器底部
    var conBottom='';//'<p  class="pButtom">底部</p>'; //可用来分页
    var html=jQueryOne(conTitle+jQueryOne("<div></div>").append(jQueryOne('<ul id="sugUL"></ul>').append(data)).html()+conBottom);
    suggestions.append(html);

    //测试用
    // jQueryOne("#area").text(suggestions.html());

    //初始索引值
    currentindex= -1;
    //显示容器数据
    suggestions.show();
    jQueryOne("#"+ContainerId+" li").click(function () {
        //单机li时数据处理                       
        CrClickHand(_input,jQueryOne(this),false);
        //隐藏
        suggestions.hide();
    });
    //获取li个数
    size=jQueryOne("#"+ContainerId+" li").size();
    //单机非容器的地方隐藏容器
    document.onclick=function (e) {
        var e=e?e:window.event;
        var tar=e.srcElement||e.target;
        //不是在容器上单击时不隐藏
        if(tar.id!=ContainerId) {
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
function movethis(up,input) {
    var _input=jQueryOne(input);
    //当前选中li的索引
    currentindex=currentindex+(up?-1:1);
    if(currentindex==size) {
        currentindex=0;
    }
    else if(currentindex<0) {
        currentindex=size-1;
    }
    //设置样式
    jQueryOne("#"+ContainerId+" li").removeClass(SelStyle);
    jQueryOne(jQueryOne("#"+ContainerId+" li")[currentindex]).addClass(SelStyle);

    //取当前选中的文本值
    var liObj=jQueryOne(jQueryOne("#"+ContainerId+" li")[currentindex]);
    //处理
    CrClickHand(_input,liObj,true);
}
//文本框处理键盘事件
function inputKeyUp(e) {
    var obj=e.srcElement?e.srcElement:e.target;
    if(obj.tagName.toLowerCase()=="input") {
        var _input=jQueryOne(obj);
        //文本框按键不是上下键 设置过滤内容到li的div容器
        if(e.keyCode!=40&&e.keyCode!=38&&e.keyCode!=13) {
            //文本框值
            var word=jQueryOne.trim(_input.val());
            if(word) {
                //获取数据 动态过滤数据            
                //显示列表数据
                showData(obj);
            }
            else {
                //文本框值为空时隐藏
                jQueryOne("#"+ContainerId).hide();
            }
        }

        //文本框按键是上下键回车键处理数据
        if(e.keyCode==38) {
            //上移
            movethis(true,obj);
        }
        else if(e.keyCode==40) {
            //下移
            movethis(false,obj);
        }
        else if(e.keyCode==13) {
            //回车
            //当前选中的列表项对象
            var LiObj=jQueryOne(jQueryOne("#"+ContainerId+" li")[currentindex]);
            //处理事件                        
            CrClickHand(_input,LiObj,false);
            //最后隐藏列表容器
            jQueryOne("#"+ContainerId).hide();
        }
    }
}
//文本框聚焦时设置参数
function inputFocus(e) {
    var obj=e.srcElement?e.srcElement:e.target;
    if(obj.tagName.toLowerCase()=="input") {
        var _input=jQueryOne(this);
        //设置弹出li的容器div宽度 位置   
        jQueryOne("#"+ContainerId).css('position','absolute').css('left',_input.position().left+'px').css('top',
    (_input.position().top+_input.height()+5)+'px').css('z-index','100').css(
    'width','700px').css('maxHeight','150px').css(
    "overflow-x","hidden").css("overflow-y","auto").css("background-color","white");
        //设置div关联的文本框
        jQueryOne("#"+ContainerId).attr("index",_input.attr("index"));
    }
}
//处理回车或者列表单击处理函数
function CrClickHand(input,li,isMove) {
    if(isMove) {//鼠标或者键盘移动列表
        //负值到文本框
        input.val(li.attr("filter"));
    } else {
        //处理鼠标单击或者回车列表
        SetData(li,input);
    }
}
//初始化参数
function initParam() {
    //数据li的容器div 默认隐藏               
    var suggestions=jQueryOne("#"+ContainerId);
    if(suggestions.length==0) {
        suggestions=jQueryOne('<div></div>');
        suggestions.attr("id",ContainerId);
        jQueryOne(document.body).append(suggestions);
    }
    suggestions=suggestions.html("").hide();
    //常旅客
    initFlyer();
    //文本框键盘事件绑定
    jQueryOne("#txtPasName_0").keydown(function (e) {
        if(e.keyCode==13) {
            inputKeyUp(e);
        }
    });
    jQueryOne("#txtPasName_0").keyup(inputKeyUp);
    //文本框聚集事件
    jQueryOne("#txtPasName_0").focus(inputFocus);

    //列表鼠标事件
    jQueryOne("#"+ContainerId).mouseover(function () {
        //鼠标滑过                     
        jQueryOne("#"+ContainerId+" li").mouseover(
                function () {
                    //选中列表的样式
                    jQueryOne(this).addClass(SelStyle);
                    currentindex=jQueryOne("#"+ContainerId+" li").index(this);
                });
    }).mouseout(function () {
        //鼠标滑出  
        jQueryOne("#"+ContainerId+" li").removeClass(SelStyle);
    });
}
//-------------------------------------------------------------------------------------------------------------------------------
//初始化常旅客数据
function initFlyer() {
    var flyVal=jQueryOne.trim(jQueryOne("#Hid_FlyerList").val());
    if(flyVal!="") {
        var flyList=eval("("+unescape(flyVal)+")");
        if(flyList.length>0) {
            var model=null;
            FlyerHtml='<ul>';
            for(var i=0;i<flyList.length;i++) {
                model=flyList[i];
                var newdata=JSON.stringify(model);
                FlyerHtml+='<li   val="'+escape(newdata)+'" filter="'+model._name+'" ><table><tr><td><span style="width:120px;text-align:center;display:block">'+model._name+'</span></td><td><span style="width:150px;text-align:center; display:block">'+GetPasTypeText(model._flyertype)+'</span></td><td><span style="width:150px;text-align:center; display:block">'+model._certificatenum+'</span></td><td><span style="width:150px;text-align:center; display:block">'+model._tel+'</span></td></tr></table></li>';
            }
            FlyerHtml+='</ul>';
        }
    }
}
//设置数据
function SetData(Li,input) {
    if(jQueryOne.trim(Li.attr("val"))!="") {
        var model=eval("("+unescape(Li.attr("val"))+")");
        var num=input.attr("index");
        //设置常旅客
        setFlyer(model,num);
    }
}
//获取乘机人类型
function GetPasTypeText(flyType) {
    var result="成人";
    if(flyType=="1") {
        result="成人";
    } else if(flyType=="2") {
        result="儿童";
    } else if(flyType=="3") {
        result="婴儿";
    }
    return result;
}
//添加数据到列表
function setFlyer(model,num) {
    //乘客姓名
    jQueryOne("#txtPasName_"+num).val(model._name);
    if(jQueryOne.trim(model._name)!="") {
        jQueryOne("#msgPasName_"+num).html("<font class='green'>√</font>");
    }
    //乘客类型
    jQueryOne("#SelPasType_"+num).val(model._flyertype);
    //如果实体值不为空
    if(model!=null&&model._flyertype!="") {
        var PassengerType=model._flyertype;
        options=getOptions(PassengerType);
        jQueryOne("#SelCardType_"+num).html(options);
    }
    //证件类型
    var SsrObj=jQueryOne("#SelCardType_"+num+" option[value='"+model._certificatetype+"']");
    SsrObj.attr("selected",true);
    //证件号
    jQueryOne("#txtCardNum_"+num).val(model._certificatenum);
    jQueryOne("#msgBirthday_"+num).hide();
    jQueryOne("#msgCardNum_"+num).show();
    if(model._flyertype=="2"||model._flyertype=="3"||model._certificatetype=="5") {
        var text=SsrObj.text();
        var ssrPatern=/(\d{4}\-\d{2}\-\d{2}){10}|(\d{4}\d{2}\d{2}){8}/g;
        if(text=="出生日期"||ssrPatern.test(model._certificatenum)) {

            jQueryOne("#msgBirthday_"+num).show();
            jQueryOne("#msgCardNum_"+num).hide();
            jQueryOne("#txtBirthday_"+num).val(model._certificatenum);
        }
    }
    //身份证验证
    if(model._certificatetype=="1") {
        var errMsg="";
        if(jQueryOne.trim(model._certificatenum).length!=15&&jQueryOne.trim(model._certificatenum).length!=18) {
            errMsg="<img src='../img/onError.gif' alt='身份证号位数错误！' title='身份证号位数错误！'>";//"<font class='red'>*身份证号位数错误！</font>";
        } else {
            var CardObj=IdCardValidate(jQueryOne.trim(model._certificatenum));
            if(CardObj!="") {
                errMsg="<img src='../img/onError.gif' alt='"+CardObj+"！' title='"+CardObj+"！'>";//"<font class='red'>* "+CardObj+"！</font>";
            }
        }
        if(errMsg!="") {
            jQueryOne("#msgcardid_"+num).html(errMsg);
        } else {
            jQueryOne("#msgcardid_"+num).html("<font class='green'>√</font>");
        }
    }

    //手机号码
    jQueryOne("#txtPhone_"+num).val(model._tel);
    /*  if(jQueryOne.trim(model._tel)!=""&&jQueryOne.trim(model._tel).length==11) {
    jQueryOne("#spanPhone_"+num).html("<font class='green'>√</font>");
    }*/
    //常旅客备注
    jQueryOne("#flyerremark_"+num).val(escape(model._remark));
}
//--------------------------------------------------------------
//初始化乘客与航段下拉框
function initSelect() {
    //航段
    var htmlOptions='';
    for(var i=1;i<=maxSky;i++) {
        htmlOptions+='<option value="'+i+'">'+i+'</option>';
    }
    jQueryOne("#selddlSky").html(htmlOptions);
    //乘客
    htmlOptions='';
    for(var i=1;i<=maxPasNum;i++) {
        htmlOptions+='<option value="'+i+'">'+i+'</option>';
    }
    jQueryOne("#passengers").html(htmlOptions);
}
//加载。。。
jQueryOne(function () {
    //加载城市控件
    ReLoad("0");
    //初始化乘客
    initArr(maxPasNum);
    //初始化航段
    initSkyArr(maxSky);
    //添加一条默认的乘机人
    var num=addGroup(window.event);
    //初始化航段下拉框
    initSelect();
    //输入乘客框回车
    jQueryOne(document).keydown(function (e) {
        var obj=e.srcElement?e.srcElement:e.target;
        if(e.keyCode==13) {
            if(obj.id.indexOf("txtPasName_")!= -1) {
                return false;
            }
        }
    });
    //初始化注册验证事件   
    jQueryOne("#txtPasName_"+num).attr("index",num);
    jQueryOne("#txtPasName_"+num).blur(pNameVate);
    //常旅客
    initParam();

    //乘机人类型
    jQueryOne("#SelPasType_"+num).change(PasTypeChange);
    //证件号类型
    jQueryOne("#SelCardType_"+num).change(CardTypeChange);
    //证件号验证
    jQueryOne("#txtCardNum_"+num).blur(CardNum);
    jQueryOne("#SelCardType_"+num).blur(CardNum);
    //手机号
    jQueryOne("#txtPhone_"+num).blur(Mobile);
    //重置数据
    jQueryOne("#btncz").click(function () {
        jQueryOne("input[type='text']").attr("value","");
        jQueryOne("select option").eq(0).attr("selected",true);
    });

    //出发城市 选择国内还是国际 
    jQueryOne("input[type='radio'][name*='cityType_']").click(function () {
        var Num=jQueryOne(this).attr("name").NewReplace("cityType_","");
        var val=jQueryOne(this).val();
        if(val=="0")//国内
        {
            ReLoad("0");
        } else if(val=="1")//国际
        {
            ReLoad("1");
        }
    });
})
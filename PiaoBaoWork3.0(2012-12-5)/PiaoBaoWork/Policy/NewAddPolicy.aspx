<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewAddPolicy.aspx.cs" Inherits="Policy_NewAddPolicy" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>添加政策</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script id="script_k0" src="../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
        body
        {
            font-size: 12px;
        }
        a:link, a:active
        {
            color: #6D69EE;
        }
        a:visited
        {
            color: blue;
        }
        a:hover
        {
            color: Red;
        }
        
        .input
        {
            width: 100px;
            color: #14B4E3;
            font-size: 12px;
            font-weight: bold;
        }
        .Content
        {
            margin-left: 5%;
            margin-right: 5%;
        }
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
        }
        .TextBorder
        {
            border: 1px solid #999;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .textLeft
        {
            text-align: left;
        }
        .textCenter
        {
            text-align: center;
        }
        .textRight
        {
            text-align: right;
        }
        .inputBorder
        {
            border: 1px solid #999;
        }
        .FontStyle
        {
            color: Green;
        }
        .tdWidth200
        {
            width: 200px;
        }
        .tdWidth100
        {
            width: 100px;
        }
        .tdWidth350
        {
            width: 350px;
        }
        .w_td
        {
            width: 12%;
            text-align: right;
            border: 1px solid #EEEEEE;
            font-weight: normal;
        }
        .r-td
        {
            width: 86%;
            border: 1px solid #EEEEEE;
        }
        #spanSun
        {
            color: Red;
        }
        #spanMonth
        {
            color: Red;
        }
        #spanStar
        {
            color: Red;
        }
        #LevelMsg
        {
            color: Red;
        }
        .tdw120
        {
            width: 120px;
        }
        .textAreaFlight
        {
            width: 549px;
            border: 1px solid #999;
        }
        .tdBorder
        {
            border: 1px solid #C5DBF2;
        }
        .AP_div
        {
            /*右侧内容区*/
            position: absolute;
            display: block;
           /* background-color: Red;
            border: solid 1px green;*/
            z-index: 200;
            left: 650px;
            top: 30px;
            width: 600px;
            height: 210px;
        }
    </style>
    <script type="text/javascript" language="javascript">
    <!--
        //为Jquery重新命名
        var jQueryOne=jQuery.noConflict(false);
        //-------------String扩展新方法-------------------------------------------
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
        //--------------------------------------------------------
        function IsReTurnList() {
            //页面类型  1普通政策 2特价政策 3默认政策 4散冲团政策
            var PageType=jQueryOne("#Hid_PageType").val();
            //是否编辑 1是 0否
            var IsEdit=jQueryOne("#Hid_IsEdit").val();
            if(IsEdit=="1") {
                var Isgo=jQueryOne('#showmsg input[name="IsGoList"][type="radio"]:checked').val();
                if(Isgo=="1") {
                    //返回列表                               
                    var cid=jQueryOne("#Hid_id").val();
                    var pid=jQueryOne("#Hid_currPage").val();
                    var where=jQueryOne("#Hid_where").val();//条件
                    location.href="PolicyList.aspx?PageType="+PageType+"&edit="+IsEdit+"&cid="+cid+"&pid="+pid+"&where="+where+"&currentuserid=<%=this.mUser.id.ToString() %>";
                }
            }
        }
        //对话框包含处理
        function showdialog(t,f) {
            jQueryOne("#showmsg").html(t);
            jQueryOne("#showmsg").dialog({
                title: '提示',
                bgiframe: true,
                height: 180,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    IsReTurnList();
                },
                buttons: {
                    '确定': function () {
                        IsReTurnList();
                        jQueryOne(this).dialog('close');
                    }
                }
            });
        }
        //只用于来显示信息
        function dialogTip(t,flag) {
            jQueryOne("#Div1").html(t);
            jQueryOne("#Div1").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    if(flag=="0") {
                        //跳到登录页面
                        top.location.href="../Login.aspx";
                    }
                },
                buttons: {
                    '确定': function () {
                        jQueryOne(this).dialog('close');
                        if(flag=="0") {
                            //跳到登录页面
                            top.location.href="../Login.aspx";
                        }
                    }
                }
            });
        }
        //返回按钮
        function ReGo() {
            var PageType=jQueryOne("#Hid_PageType").val();
            location.href="PolicyList.aspx?PageType="+PageType+"&currentuserid=<%=this.mUser.id.ToString() %>";
        }
        //普通政策发布中显示特价舱位
        function showDiv(num) {
            var ckObj=jQueryOne("#TJCK_"+num);
            if(ckObj.is(":checked")) {
                if(ckObj[0].isshow==null||ckObj[0].isshow==undefined) {
                    ckObj[0].isshow="1";
                    var airCode=jQueryOne("#Select_"+num).val().split('^')[0].toUpperCase();
                    GetSeat(airCode,num,3);
                }
                jQueryOne("#TJSpace_"+num).show();
            } else {
                jQueryOne("#TJSpace_"+num).hide();
            }
        }
        //获取城市信息
        function GetCityInfo(city) {
            var CityInfo=null;//
            var cityList=eval("("+unescape(jQueryOne("#Hid_InnerCityData").val())+")");
            if(cityList!=null&&cityList.length>0) {
                for(var i=0;i<cityList.length;i++) {
                    if(cityList[i]._cityname==jQueryOne.trim(city)||cityList[i]._citycodeword.toUpperCase()==jQueryOne.trim(city).toUpperCase()) {
                        CityInfo=cityList[i];
                        break;
                    }
                }
            }
            return CityInfo;
        }
        //获取航空公司二字码
        function GetAirCode(num) {
            var ArrCodeArr=unescape(jQueryOne("#Hid_AirCodeCache").val()).split('|');
            if(ArrCodeArr.length>0) {
                var strOp=[];
                //strOp.push('<option value="0">--请选择航空公司--</option>');
                var item=[];
                var Val='',Name='';
                for(var i=0;i<ArrCodeArr.length;i++) {
                    item=ArrCodeArr[i].split('^');
                    if(item.length==4) {
                        Name=item[1]+'-'+item[2];
                        //code 全称 结算码
                        Val=item[1]+"^"+item[0]+"^"+item[3];
                        strOp.push('<option value="'+Val+'">'+Name+'</option>');
                    }
                }
                jQueryOne("#Select_"+num).html(strOp.join(''));
                //是否编辑 1是 0否
                var IsEdit=jQueryOne("#Hid_IsEdit").val();
                if(IsEdit=="0") {
                    //默认选择的航空公司二字码 触发异步ajax获取舱位
                    var airCode=jQueryOne("#Select_"+num).val().split('^')[0].toUpperCase();
                    GetSeat(airCode,num);
                    jQueryOne("#txtAirCode_"+num).val(airCode);
                }
            }
        }
        //舱位全选
        function SelectAll(selObj,n) {
            var seatCols=document.getElementsByName("seat"+n);
            for(var i=0;i<seatCols.length;i++) {
                seatCols[i].checked=selObj.checked;
            }
        }
        //舱位全选
        function ReverseAll(selObj,n) {
            var seatCols=document.getElementsByName("seat"+n);
            if(selObj.checked) {
                for(var i=0;i<seatCols.length;i++) {
                    if(seatCols[i].checked) {
                        seatCols[i].checked=false;
                    } else {
                        seatCols[i].checked=true;
                    }
                }
            }
        }
        //文本框调用 设置下拉框的值 
        function SetSel(txtObj,num) {
            var txtVal=txtObj.value.toUpperCase();
            if(txtVal=="") return;
            var sel=document.getElementById("Select_"+num);
            var v1;
            for(var i=0;i<sel.options.length;i++) {
                //航空公司二字码
                v1=sel.options[i].value.split('^')[0].toUpperCase();
                if(v1==txtVal) {//&&!sel.options[i].checked
                    sel.options[i].selected=true;
                    if(sel.lastAircode==null||sel.lastAircode!=v1) {
                        sel.lastAircode=v1;
                        //获取舱位
                        GetSeat(txtVal,num);
                    }
                    break;
                }
            }
        }
        //下拉列表调用  设置文本框的值
        function SetText(ddlObj,num) {
            var ddlVal=jQueryOne(ddlObj).val();
            //航空公司二字码
            var vCode=ddlVal.split('^')[0].toUpperCase();
            jQueryOne("#txtAirCode_"+num).val(vCode);
        }
        //通过航空公司获取舱位
        function GetSeat(aircode,num,_PolicyKindType) {
            if(jQueryOne.trim(aircode).length<2) { return };
            // 1.普通，2.特价
            var PolicyKindType=jQueryOne("#Hid_PolicyKind").val();
            if(_PolicyKindType!=undefined&&_PolicyKindType!=null) {
                PolicyKindType=_PolicyKindType;
            }
            //1编辑 0添加
            var IsEdit=jQueryOne("#Hid_IsEdit").val();
            var OpType=PolicyKindType=="1"?"PTSpace":"TJSpace";
            jQueryOne.post("../AJAX/CommonAjAx.ashx",
            {
                OpFunction: OpType,
                AirCode: aircode,
                num: Math.random(),
                currentuserid: '<%=this.mUser.id.ToString() %>'
            },
            function (data) {

                //判断网页是否失效
                var isgo=IsGoLoginPage(data);
                if(isgo) {
                    dialogTip("网页已失效请重新登录！","0");
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
                                var html='';
                                if(PolicyKindType=="1") {
                                    html='<p><label for="selectAll_'+num+'"><input name="selectAll_'+num+'" type="radio" class="radio_input" id="selectAll_'+num+'" onclick="SelectAll(this,'+num+')" checked="true" /><font style="color: Green;">全选</font></label>'+
                                    '&nbsp;&nbsp;&nbsp;&nbsp;<label for="selectReverse_'+num+'"><input name="selectAll_'+num+'" type="radio" class="radio_input" id="selectReverse_'+num+'" onclick="ReverseAll(this,'+num+')" /><font style="color: Green;">反选</font></label></p>';
                                    html+="<p>基本舱位:</p>";
                                } else if(PolicyKindType=="2"||PolicyKindType=="3") {
                                    html="<p>特价舱位:</p>";
                                    //特价舱位
                                }
                                //返回结果
                                var objList=eval("("+result+")");
                                //编辑的舱位
                                var editSpace=EmptyArr(jQueryOne("#Hid_EditSeat").val().split('/'));
                                var val='',text='',IsChecked='';
                                //按照类型操作
                                if(PolicyKindType=="1") {
                                    var Len=10;//每排11个舱位
                                    for(var i=0;i<objList.length;i++) {
                                        val=objList[i]._cabin.toUpperCase();
                                        text=objList[i]._cabin.toUpperCase()+'['+objList[i]._rebate+']';
                                        if(IsEdit=="1") {
                                            if(jQueryOne.inArray(objList[i]._cabin.toUpperCase(),editSpace)!= -1) {
                                                IsChecked=' checked="checked" ';
                                            } else {
                                                //IsChecked=' checked="false" ';
                                                IsChecked=" ";
                                            }
                                        } else {
                                            IsChecked=' checked="checked" ';
                                        }
                                        //排列
                                        if((i+1)%Len==0) {
                                            html+='<label for="seat_'+num+'_'+i+'"><input id="seat_'+num+'_'+i+'" type="checkbox" name="seat'+num+'" value="'+val+'" '+IsChecked+'  /><font style="color:green;">'+text.padRight(7,"&nbsp;")+'</font></label> &nbsp;&nbsp;<br />';
                                        } else {
                                            //普通舱位
                                            html+='<label for="seat_'+num+'_'+i+'"><input id="seat_'+num+'_'+i+'" type="checkbox" name="seat'+num+'" value="'+val+'" '+IsChecked+'  /><font style="color:green;">'+text.padRight(7,"&nbsp;")+'</font></label>&nbsp;&nbsp;';
                                        }
                                    }
                                    //
                                    jQueryOne("#seat_"+num).html(html);
                                } else if(PolicyKindType=="2"||PolicyKindType=="3") {
                                    //特价舱位
                                    if(objList.length>0) {
                                        //特价舱位 用“/”分割
                                        var InitLength=jQueryOne("input[name='seat"+num+"'][type='checkbox']").length;
                                        var TJSeatArr=EmptyArr(objList[0]._spcabin.split('/'));
                                        for(var i=InitLength,j=0;i<TJSeatArr.length+InitLength;i++,j++) {
                                            if(IsEdit=="1") {
                                                if(jQueryOne.inArray(TJSeatArr[j].toUpperCase(),editSpace)!= -1) {
                                                    IsChecked=' checked="checked" ';
                                                } else {
                                                    IsChecked=" ";
                                                }
                                            } else {
                                                IsChecked=' checked="checked" ';
                                            }
                                            html+='<label for="seat_'+num+'_'+i+'"><input id="seat_'+num+'_'+i+'" type="checkbox" name="seat'+num+'" value="'+TJSeatArr[j]+'" '+IsChecked+'  /><font style="color:green;">'+TJSeatArr[j].padRight(7,"&nbsp;")+'</font></label>&nbsp;&nbsp;';
                                        }
                                        //设置特价舱位
                                        jQueryOne("#TJSpace_"+num).html(html);
                                        if(PolicyKindType=="2") {
                                            jQueryOne("#seat_"+num).hide();
                                            jQueryOne("#TJSpace_"+num).show();
                                        }
                                    } else {
                                        //没有舱位的可以继续选择其他
                                        jQueryOne("#TJCK_"+num)[0].isshow=null;
                                    }
                                }
                            } else {
                                //按照类型操作
                                if(PolicyKindType=="1") {
                                    jQueryOne("#seat_"+num).html("没有找到"+aircode+"基本舱位！");
                                } else if(PolicyKindType=="2"||PolicyKindType=="3") {
                                    //特价舱位
                                    //没有舱位的可以继续选择其他
                                    jQueryOne("#TJCK_"+num)[0].isshow=null;
                                    if(PolicyKindType=="2") {
                                        jQueryOne("#seat_"+num).hide();
                                        jQueryOne("#TJSpace_"+num).show();
                                    }
                                    jQueryOne("#TJSpace_"+num).html("没有找到"+aircode+"设置的特价舱位！");
                                }
                            }
                        } else {
                            alert(data);
                        }
                    } else {
                        alert(data);
                    }
                } catch(e) {
                    alert(e.message);
                }
            },
            "text");
        }
        //设置隐藏显示是否有自动出票方式
        function SetAutoHide(obj,num) {
            var selPolicyType=jQueryOne(obj).val();
            if(selPolicyType=="3") {
                jQueryOne("#tr_etdztype_"+num).hide();
            } else {
                jQueryOne("#tr_etdztype_"+num).show();
            }
        }
        //是否跳到登录页面
        function IsGoLoginPage(data) {
            var isGo=false;
            if(data.indexOf('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">')!= -1) {
                isGo=true;
            }
            return isGo;
        }
        //最大9组
        var maxGroup=9;
        //添加一组
        function AddTrGroup(obj,evt) {
            jQueryOne(obj).attr("disabled",true);
            var num=0;
            var trObj=jQueryOne('#tabAll tr[id="policy_tr'+num+'"]');
            var objlength=jQueryOne('#tabAll tr[id*="policy_tr"]').length;
            if(trObj.length>0) {
                if(objlength<maxGroup) {
                    var newNum=objlength;
                    var rowIndex=trObj[0].rowIndex+1;
                    var NewTr=jQueryOne(trObj[0]).clone(true);
                    var html=jQueryOne(NewTr).html();
                    //替换id
                    html=html.NewReplace("policytab_"+num,"policytab_"+newNum).NewReplace("SetSel(this,"+num+")","SetSel(this,"+newNum+")").NewReplace("txtAirCode_"+num,"txtAirCode_"+newNum);
                    html=html.NewReplace("Container_"+num,"Container_"+newNum).NewReplace("SetSel(this,"+num+")","SetSel(this,"+newNum+")").NewReplace("Select_"+num,"Select_"+newNum).NewReplace("Container_"+num,"Container_"+newNum);
                    html=html.NewReplace("SetText(this,"+num+")","SetText(this,"+newNum+")").NewReplace("txtOffice_"+num,"txtOffice_"+newNum).NewReplace("SelPolicyType_"+num,"SelPolicyType_"+newNum);
                    html=html.NewReplace("seat_"+num,"seat_"+newNum).NewReplace("seat"+num,"seat"+newNum).NewReplace("txtTicketStartDate_"+num,"txtTicketStartDate_"+newNum).NewReplace("txtTicketEndDate_"+num,"txtTicketEndDate_"+newNum);
                    html=html.NewReplace("txtFlightStartDate_"+num,"txtFlightStartDate_"+newNum).NewReplace("txtFlightEndDate_"+num,"txtFlightEndDate_"+newNum).NewReplace("policy_"+num,"policy_"+newNum);
                    html=html.NewReplace("DownPoint_"+num,"DownPoint_"+newNum).NewReplace("LaterPoint_"+num,"LaterPoint_"+newNum).NewReplace("SharePoint_"+num,"SharePoint_"+newNum).NewReplace("txtAdvanceDay_"+num,"txtAdvanceDay_"+newNum);
                    html=html.NewReplace("btnAdd_"+num,"btnAdd_"+newNum).NewReplace("optionDiv_"+num,"optionDiv_"+newNum).NewReplace("tabContainer_"+num,"tabContainer_"+newNum);
                    html=html.NewReplace("selectAll_"+num,"selectAll_"+newNum).NewReplace("selectReverse_"+num,"selectReverse_"+newNum);
                    html=html.NewReplace("SelectAll(this,"+num+")","SelectAll(this,"+newNum+")").NewReplace('ReverseAll(this,'+num+')','ReverseAll(this,'+newNum+')');
                    html=html.NewReplace("TJshow_"+num,"TJshow_"+newNum).NewReplace("TJCK_"+num,"TJCK_"+newNum).NewReplace("showDiv("+num+")","showDiv("+newNum+")").NewReplace("TJSpace_"+num,"TJSpace_"+newNum);
                    html=html.NewReplace("td_cangwei_"+num,"td_cangwei_"+newNum).NewReplace("SetAutoHide(this,"+num+")","SetAutoHide(this,"+newNum+")");
                    html=html.NewReplace("divetdztype_"+num,"divetdztype_"+newNum).NewReplace("etdz0_"+num,"etdz0_"+newNum).NewReplace("etdz1_"+num,"etdz1_"+newNum).NewReplace("etdz2_"+num,"etdz2_"+newNum).NewReplace("etdztype_"+num,"etdztype_"+newNum).NewReplace("tr_etdztype_"+num,"tr_etdztype_"+newNum);

                    //替换现返
                    html=html.NewReplace("DownReturnMoney_"+num,"DownReturnMoney_"+newNum).NewReplace("LaterReturnMoney_"+num,"LaterReturnMoney_"+newNum).NewReplace("ShareReturnMoney_"+num,"ShareReturnMoney_"+newNum);

                    //特价
                    html=html.NewReplace("tjTab_"+num,"tjTab_"+newNum).NewReplace("msg_"+num,"msg_"+newNum).NewReplace("txtTJGuding_"+num,"txtTJGuding_"+newNum).NewReplace("txtTJcankao_"+num,"txtTJcankao_"+newNum)

                    //添加操作内容
                    var opHtml='<DIV id=optionDiv_'+newNum+'><SPAN class="btn btn-ok-s"><INPUT id=btnAdd_'+newNum+' onclick=AddTrGroup(this,event) value=添加 type=button></SPAN> </DIV>'+
                    '<span class="btn btn-ok-s"><input id="btnDel_'+newNum+'" type="button" value="删除" onclick="DelTrGroup(event,'+newNum+')"  /></span>';
                    //新的一行数据                                                                                           
                    var newHtml='<tr id="policy_tr'+newNum+'"><td class="w_td">舱位与政策:</td><td>'+jQueryOne(html).find("#tabContainer_"+newNum+" td:last").html(opHtml).parent().parent().parent().parent().html()+"</td></tr>";
                    //添加到后面
                    jQueryOne('#tabAll tr[id="policy_tr'+(newNum-1)+'"]').after(newHtml);
                    var objlength=jQueryOne('#tabAll tr[id*="policy_tr"]').length;
                    jQueryOne("#Hid_dyncId").val(objlength);
                    //注册事件
                    jQueryOne("#DownPoint_"+newNum).blur(NumVate);
                    jQueryOne("#LaterPoint_"+newNum).blur(NumVate);
                    jQueryOne("#SharePoint_"+newNum).blur(NumVate);
                    jQueryOne("#txtTJGuding_"+newNum).blur(NumVate);
                    jQueryOne("#txtTJcankao_"+newNum).blur(NumVate);
                    jQueryOne("#txtAdvanceDay_"+newNum).blur(NumVate);
                    //现返
                    jQueryOne("#DownReturnMoney_"+newNum).blur(NumVate);
                    jQueryOne("#LaterReturnMoney_"+newNum).blur(NumVate);
                    jQueryOne("#ShareReturnMoney_"+newNum).blur(NumVate);



                    jQueryOne("#txtOffice_"+newNum).blur(function () {
                        var val=jQueryOne.trim(jQueryOne(this).val());
                        var patOffice=/^[A-Za-z]{3}\d{3}$/;
                        if(val!=""&&!patOffice.test(val)) {
                            showdialog("输入Office格式错误!");
                            return false;
                        }
                    });
                    //设置航空公司下拉列表数据
                    var AirCode=jQueryOne.trim(jQueryOne('#Select_'+num).val());
                    jQueryOne('#Select_'+newNum).val(AirCode);
                    SetText(jQueryOne('#Select_'+num),newNum)
                } else {
                    showdialog('最多添加9组');
                }
            }
            jQueryOne(obj).attr("disabled",false);
        }
        //移除一组
        function DelTrGroup(evt,num) {
            jQueryOne('#tabAll tr[id="policy_tr'+num+'"]').remove();
            //更改Id
            var dyncId=parseInt(jQueryOne("#Hid_dyncId").val())-1;
            jQueryOne("#Hid_dyncId").val(dyncId);
            return false;
        }
        //------------------------控件部分js----------------------------------------------  
        // //浏览器名称
        //var browserName = browser(navigator.userAgent.toLowerCase()); 
        function browser(a) {
            var c=/opera/.test(a),
            h=/chrome/.test(a),
            b=/webkit/.test(a),
            m=!h&&/safari/.test(a),
            g=!c&&/msie/.test(a),
            e=g&&/msie 7/.test(a),
            f=g&&/msie 8/.test(a),
            i=g&&!e&&!f,
            k=!b&&/gecko/.test(a),
            n=k&&/rv:1\.8/.test(a);
            k&&/rv:1\.9/.test(a);
            return {
                IE: g,
                IE6: i,
                IE7: e,
                IE8: f,
                Moz: k,
                FF2: n,
                Opera: c,
                Safari: m,
                WebKit: b,
                Chrome: h
            }
        }
        //重新加载城市控件
        function ReLoad() {
            try {
                //发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策
                var PageType=jQueryOne("#Hid_PageType").val();

                var rd="?r="+Math.random();
                var SE=new CtripJsLoader();
                var jipiaoType=jQueryOne("#Hid_jipiaoType").val();
                var dataUrl="../AJAX/GetCity.aspx"+rd;
                if(jipiaoType=="1") {
                    dataUrl="../JS/CitySelect/fltdomestic1_gb2312.js"+rd;//国际机票
                }
                var files=[["../JS/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],[dataUrl,"GB2312",true,null]];
                SE.scriptAll(files);

                //注册添加和排除按钮回车事件      
                jQueryOne("#txtFromCode").keydown(function (event) {
                    if(event.keyCode==13) {
                        KeyBoardInput('txtFromCode','From_RightBox','From_LeftBox');
                    }
                });
                jQueryOne("#txtFromCode_01").keydown(function (event) {
                    if(event.keyCode==13) {
                        KeyBoardInput('txtFromCode_01','From_LeftBox','From_RightBox');
                    }
                });
                jQueryOne("#txtMiddleCity2").keydown(function (event) {
                    if(event.keyCode==13) {
                        KeyBoardInput('txtMiddleCity2','Middle_RightBox','Middle_LeftBox');
                    }
                });
                jQueryOne("#txtMiddleCity_02").keydown(function (event) {
                    if(event.keyCode==13) {
                        KeyBoardInput('txtMiddleCity_02','Middle_LeftBox','Middle_RightBox');
                    }
                });
                jQueryOne("#txtToCode").keydown(function (event) {
                    if(event.keyCode==13) {
                        KeyBoardInput('txtToCode','To_RightBox','To_LeftBox');
                    }
                });
                jQueryOne("#txtToCode_02").keydown(function (event) {
                    if(event.keyCode==13) {
                        KeyBoardInput('txtToCode_02','To_LeftBox','To_RightBox');
                    }
                });
                //列表框键盘事件 6个
                jQueryOne("#From_LeftBox").keydown(function (event) {
                    if(event.keyCode==39) {//右
                        dblclick('From_RightBox','From_LeftBox','1','rl');
                    }
                    else if(event.keyCode==40) {//下
                        dblclick('From_RightBox','From_LeftBox','2','rl')
                    }
                });
                jQueryOne("#From_RightBox").keydown(function (event) {
                    if(event.keyCode==37) {//左
                        dblclick('From_RightBox','From_LeftBox','1','lr');
                    }
                    else if(event.keyCode==38) {//上
                        dblclick('From_RightBox','From_LeftBox','2','lr');
                    }
                });
                jQueryOne("#Middle_LeftBox").keydown(function (event) {
                    if(event.keyCode==39) {//右                  
                        dblclick('Middle_RightBox','Middle_LeftBox','1','rl');
                    }
                    else if(event.keyCode==40) {//下
                        dblclick('Middle_RightBox','Middle_LeftBox','2','rl')
                    }
                });
                jQueryOne("#Middle_RightBox").keydown(function (event) {
                    if(event.keyCode==37) {//左
                        dblclick('Middle_RightBox','Middle_LeftBox','1','lr');
                    }
                    else if(event.keyCode==38) {//上
                        dblclick('Middle_RightBox','Middle_LeftBox','2','lr');
                    }
                });
                jQueryOne("#To_LeftBox").keydown(function (event) {
                    if(event.keyCode==39) {//右
                        dblclick('To_RightBox','To_LeftBox','1','rl');
                    }
                    else if(event.keyCode==40) {//下
                        dblclick('To_RightBox','To_LeftBox','2','rl');
                    }
                });
                jQueryOne("#To_RightBox").keydown(function (event) {
                    if(event.keyCode==37) {//左
                        dblclick('To_RightBox','To_LeftBox','1','lr');
                    }
                    else if(event.keyCode==38) {//上
                        dblclick('To_RightBox','To_LeftBox','2','lr');
                    }
                });

                //注册文本框数字小数的验证
                jQueryOne("#txtAirReBate").blur(NumVate);
                jQueryOne("#DownPoint_0").blur(NumVate);
                jQueryOne("#LaterPoint_0").blur(NumVate);
                jQueryOne("#SharePoint_0").blur(NumVate);
                jQueryOne("#txtTJGuding_0").blur(NumVate);
                jQueryOne("#txtTJcankao_0").blur(NumVate);
                jQueryOne("#txtAdvanceDay_0").blur(NumVate);
                //现返
                jQueryOne("#DownReturnMoney_0").blur(NumVate);
                jQueryOne("#LaterReturnMoney_0").blur(NumVate);
                jQueryOne("#ShareReturnMoney_0").blur(NumVate);
                jQueryOne("#txtAirReturnMoney").blur(NumVate);


                jQueryOne("#txtOffice_0").blur(function () {
                    var val=jQueryOne.trim(jQueryOne(this).val());
                    var patOffice=/^[A-Za-z]{3}\d{3}$/;
                    if(val!=""&&!patOffice.test(val)) {
                        showdialog("输入Office格式错误!");
                        return false;
                    }
                });

                //是否开启高返权限1是 0否
                var IsOpenGF=jQueryOne("#Hid_IsOpenGF").val();
                if(IsOpenGF=="0") {
                    //隐藏高返
                    jQueryOne("#tr_gaofan").hide();
                } else {
                    jQueryOne("#tr_gaofan").show();
                }
                //是否编辑 1是 0否
                var IsEdit=jQueryOne("#Hid_IsEdit").val();
                var EditData=unescape(jQueryOne("#Hid_EditData").val());
                if(PageType=="1"||PageType=="2") {
                    //特价政策时隐藏
                    if(PageType=="2") {
                        //隐藏联程
                        //jQueryOne("#span_fourTravel").hide();
                        //隐藏高返
                        jQueryOne("#tr_gaofan").hide();
                        //隐藏团队标识
                        jQueryOne("#tr_tuan").hide();
                        //显示票价生成方式
                        jQueryOne("#tr_GenerationType").show();
                        //隐藏正常价格
                        jQueryOne("#span_tjtype0").hide();
                        jQueryOne("#tjtype1").attr("checked",true);

                        //设置舱位区域大小
                        jQueryOne("#td_cangwei_0").attr("style","width:auto!important;");
                    }
                    //普通政策
                    if(IsEdit=="0") {
                        //添加政策
                        GetAirCode(0);
                    } else if(IsEdit=="1") {
                        //隐藏添加多个按钮
                        jQueryOne("#optionDiv_0").hide();
                        //编辑
                        var ObjModel=eval("("+EditData+")");
                        if(ObjModel!=null&&ObjModel!="") {
                            //政策种类
                            jQueryOne("#Hid_PolicyKind").val(ObjModel._policykind);
                            //票价生成方式
                            setGenerationType(ObjModel._generationtype)
                            //发布类型
                            var releasetype=ObjModel._releasetype;
                            //行程类型
                            var traveltype=ObjModel._traveltype;
                            //发布类型
                            setPublishType(releasetype);
                            //行程类型
                            setTravelType(traveltype);
                            //自动出票方式
                            setETDZType(ObjModel._autoprintflag);
                            //是否低开
                            jQueryOne("#cklowerOpen").attr("checked",ObjModel._isloweropen=="1"?true:false);
                            //是否高返
                            jQueryOne("#ckIsGaoFan").attr("checked",ObjModel._highpolicyflag=="1"?true:false);
                            var fromCode=ObjModel._startcitynamecode;
                            var middleCode=ObjModel._middlecitynamecode;
                            var toCode=ObjModel._targetcitynamecode;
                            //出发城市
                            if(releasetype=="1") {
                                var f1=EmptyArr(fromCode.split('/'))[0];
                                //出发城市
                                jQueryOne("#FromCityCode").val(f1);
                                var cityInfo=GetCityInfo(f1);
                                if(cityInfo!=null) {
                                    jQueryOne("#txtFromCityName").val(cityInfo._cityname);
                                } else {
                                    jQueryOne("#txtFromCityName").val(f1);
                                }
                                jQueryOne("#FromSameCitySharePolicy").attr("checked",ObjModel._startcitynamesame=="1"?true:false);
                            } else {
                                //出发城市
                                KeyBoardInput('txtFromCode','From_RightBox','From_LeftBox',fromCode);
                            }
                            //中转城市
                            KeyBoardInput('txtMiddleCity2','Middle_RightBox','Middle_LeftBox',middleCode);
                            //到达城市
                            if(releasetype=="2") {
                                //到达城市
                                var t1=EmptyArr(toCode.split('/'))[0];
                                //出发城市
                                jQueryOne("#ToCityCode").val(t1);
                                var cityInfo=GetCityInfo(t1);
                                if(cityInfo!=null) {
                                    jQueryOne("#ToCityName").val(cityInfo._cityname);
                                } else {
                                    jQueryOne("#ToCityName").val(t1);
                                }
                                jQueryOne("#ToSameCitySharePolicy").attr("checked",ObjModel._targetcitynamesame=="1"?true:false);
                            } else {
                                //到达城市
                                KeyBoardInput('txtToCode','To_RightBox','To_LeftBox',toCode);
                            }
                            //班期限制
                            setSchedule(ObjModel._scheduleconstraints);
                            //使用航班号
                            var fvalue=ObjModel._applianceflighttype=="1"?"":ObjModel._applianceflighttype=="2"?ObjModel._applianceflight:ObjModel._unapplianceflight;
                            setFlightNumGroup(null,ObjModel._applianceflighttype,fvalue);
                            //设置航空公司
                            GetAirCode(0);
                            //设置隐藏域舱位
                            jQueryOne("#Hid_EditSeat").val(ObjModel._shippingspace);
                            jQueryOne("#txtAirCode_0").val(ObjModel._carrycode.NewReplace("/",""));

                            try {
                                jQueryOne("#Select_0 option[value*='"+ObjModel._carrycode.NewReplace("/","")+"']").attr("selected",true);
                                /*IE: g,
                                IE6: i,
                                IE7: e,
                                IE8: f,
                                Moz: k,
                                FF2: n,
                                Opera: c,
                                Safari: m,
                                WebKit: b,
                                Chrome: h
                                */
                            } catch(e) {
                                //舱位
                                GetSeat(ObjModel._carrycode.NewReplace("/",""),0);
                                var browserName=browser(navigator.userAgent.toLowerCase());
                                if(browserName.IE6) {
                                    jQueryOne("#Select_0 option[value*='"+ObjModel._carrycode.NewReplace("/","")+"']").find("option").attr("selected",true);
                                }
                            }
                            //office
                            jQueryOne("#txtOffice_0").val(ObjModel._office);
                            //政策类型
                            jQueryOne("#SelPolicyType_0").val(ObjModel._policytype);
                            //点数
                            jQueryOne("#DownPoint_0").val(ObjModel._downpoint);
                            jQueryOne("#LaterPoint_0").val(ObjModel._laterpoint);
                            jQueryOne("#SharePoint_0").val(ObjModel._sharepoint);
                            //现返
                            jQueryOne("#DownReturnMoney_0").val(ObjModel._downreturnmoney);
                            jQueryOne("#LaterReturnMoney_0").val(ObjModel._laterreturnmoney);
                            jQueryOne("#ShareReturnMoney_0").val(ObjModel._sharepointreturnmoney);

                            //特价部分
                            jQueryOne("#txtTJGuding_0").val(ObjModel._spaceprice);
                            //参考价格
                            jQueryOne("#txtTJcankao_0").val(ObjModel._referenceprice);

                            //乘机开始日期
                            var date=eval("new "+ObjModel._flightstartdate.NewReplace("/","")+"");
                            jQueryOne("#txtFlightStartDate_0").val(GetStrDate(date));
                            //乘机结束日期
                            date=eval("new "+ObjModel._flightenddate.NewReplace("/","")+"");
                            jQueryOne("#txtFlightEndDate_0").val(GetStrDate(date));
                            //出票开始日期
                            date=eval("new "+ObjModel._printstartdate.NewReplace("/","")+"");
                            jQueryOne("#txtTicketStartDate_0").val(GetStrDate(date));
                            //出票结束日期
                            date=eval("new "+ObjModel._printenddate.NewReplace("/","")+"");
                            jQueryOne("#txtTicketEndDate_0").val(GetStrDate(date));
                            //提前天数
                            jQueryOne("#txtAdvanceDay_0").val(ObjModel._advanceday);

                            //航空公司返点
                            jQueryOne("#txtAirReBate").val(ObjModel._airrebate);
                            //现返
                            jQueryOne("#txtAirReturnMoney").val(ObjModel._airrebatereturnmoney);

                            //是否团标识 团队标志 0.普通，1.团队
                            jQueryOne("#txtTeamFlagCK").attr("checked",ObjModel._teamflag=="1"?true:false);
                            //是否审核 
                            jQueryOne("#ck_AuditType").attr("checked",ObjModel._audittype=="1"?true:false);
                            //是否使用共享航班
                            //jQueryOne("input[name='rdChild'][type='radio'][value='"+ObjModel._isapplytoshareflight+"']").attr("checked",true);
                            jQueryOne("#txtApplyToFlightNo").val(ObjModel._shareaircode);

                            ApplyCarry(ObjModel._isapplytoshareflight);
                            jQueryOne("#txtPolicyRemark").val(ObjModel._remark);
                        }
                    }
                }
            } catch(e) {
                alert(e.message);
            }
        }

        //数字验证【0-100.00】
        function NumVate() {
            try {

                var value=jQueryOne.trim(jQueryOne(this).val());
                if(value==null||value=='') {
                    showdialog("文本框不能为空,请输入正确的数字!");
                    jQueryOne(this).val("0");
                    return false;
                }
                var ctrlId=jQueryOne(this).attr("id");
                //数据范围0-100
                var NumFanWei=jQueryOne(this).attr("fanwei");
                if(!isNaN(value)) {
                    var userreg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
                    if(userreg.test(value)) {
                        //验证通过
                        if(parseFloat(value).toString().length>5) {
                            jQueryOne(this).val("0");
                            showdialog("输入数据超出范围!");
                            return false;
                        }
                        var idArr=[
                        'txtAirReBate',
                        'DownPoint_',
                        'LaterPoint_',
                        'SharePoint_'
                        ];
                        var isXianZhi=false;
                        for(var i=0;i<ctrlId.length;i++) {
                            if(ctrlId.indexOf(ctrlId[i])!= -1) {
                                isXianZhi=true;
                                break;
                            }
                        }
                        //数据范围有限制
                        if(isXianZhi&&NumFanWei!=null) {
                            var xzArr=NumFanWei.split('-');
                            if(xzArr.length==2) {
                                var min=parseFloat(xzArr[0],10);
                                var max=parseFloat(xzArr[1],10);
                                var curVal=parseFloat(value,10);
                                if(curVal>=max||curVal<min) {
                                    jQueryOne(this).val("0");
                                    showdialog("输入数据超出范围["+NumFanWei+"]!");
                                    return false;
                                }
                            }
                        }

                    } else {
                        var numindex=parseInt(value.indexOf("."),10);
                        if(numindex==0) {
                            jQueryOne(this).val("0");
                            showdialog("输入的数字不规范");
                            return false;
                        }
                        var head=value.substring(0,numindex);
                        var bottom=value.substring(numindex,numindex+3);
                        var fianlNum=head+bottom;
                        jQueryOne(this).val(fianlNum);
                    }
                } else {
                    jQueryOne(this).val("0");
                    showdialog("请输入数字");
                    return false;
                }
                var s='0';
                if(NumFanWei!=null) {
                    var x=parseFloat(value,10);
                    s=ShowPoint(x,1);//保留一位小数
                } else {
                    if(ctrlId.indexOf("txtAdvanceDay")!= -1) {
                        s=parseInt(value,10);
                    } else {
                        s=parseFloat(value,10);
                    }
                }
                jQueryOne(this).val(s);

            } catch(e) {
                //alert(e.message);
            }
        }
        //数字 保留小数位数
        function ShowPoint(number,PointNum) {
            var tempNum=number.toString();
            if(tempNum.indexOf(".")!= -1)//存在小数点
            {
                var strArr=tempNum.split('.');
                if(strArr[1].length>PointNum) {
                    strArr[1]=strArr[1].substring(0,PointNum);
                } else {
                    strArr[1].padRight(PointNum,'0');
                }
                tempNum=strArr[0]+"."+strArr[1];
            }
            return parseFloat(tempNum,10);
        }
        //双击或者添加listBox 
        function dblclick(sourceId,targetId,Only_Mul,Direct) {
            var SourceBox=document.getElementById(sourceId); // jQueryOne("#" + sourceId); 
            var TargetBox=document.getElementById(targetId); //jQueryOne("#" + targetId);
            if(Direct=="lr") {
                if(Only_Mul=="0") {
                    //单个
                    if(SourceBox.selectedIndex<0) {
                        return;
                    }
                    var val=SourceBox.options[SourceBox.selectedIndex].value;
                    var text=SourceBox.options[SourceBox.selectedIndex].text;
                    //移除源
                    SourceBox.removeChild(SourceBox.options[SourceBox.selectedIndex]);
                    //添加目的
                    TargetBox.options.add(new Option(text,val));
                } else if(Only_Mul=="1") {
                    //多个
                    var len=SourceBox.options.length;
                    var selArr=[];
                    for(var i=0;i<len;i++) {
                        if(SourceBox.options[i].selected) {
                            selArr.push(SourceBox.options[i]);
                        }
                    }
                    for(var j=0;j<selArr.length;j++) {
                        TargetBox.options.add(new Option(selArr[j].text,selArr[j].value));
                        SourceBox.removeChild(selArr[j]);
                    }
                }
                else if(Only_Mul=="2") {
                    //所有
                    for(var i=0;i<SourceBox.options.length;i++) {
                        TargetBox.options.add(new Option(SourceBox.options[i].text,SourceBox.options[i].value));
                    }
                    SourceBox.options.length=0;
                }
            } else if(Direct=="rl") {
                if(Only_Mul=="0") {
                    //单个
                    if(TargetBox.selectedIndex<0) {
                        return;
                    }
                    var val=TargetBox.options[TargetBox.selectedIndex].value;
                    var text=TargetBox.options[TargetBox.selectedIndex].text;
                    //移除源
                    TargetBox.removeChild(TargetBox.options[TargetBox.selectedIndex]);
                    //添加目的
                    SourceBox.options.add(new Option(text,val));
                } else if(Only_Mul=="1") {
                    //多个
                    var len=TargetBox.options.length;
                    var selArr=[];
                    for(var i=0;i<len;i++) {
                        if(TargetBox.options[i].selected) {
                            selArr.push(TargetBox.options[i]);
                        }
                    }
                    for(var j=0;j<selArr.length;j++) {
                        SourceBox.options.add(new Option(selArr[j].text,selArr[j].value));
                        TargetBox.removeChild(selArr[j]);
                    }
                }
                else if(Only_Mul=="2") {
                    //所有                   
                    for(var i=0;i<TargetBox.options.length;i++) {
                        SourceBox.options.add(new Option(TargetBox.options[i].text,TargetBox.options[i].value));
                    }
                    TargetBox.options.length=0;
                }
            }
            GetString(sourceId);
        }
        //键盘输入文本框
        function KeyBoardInput(SourceId,sBox,tBox,Content) {
            var sVal='';
            if(Content!=undefined&&Content!=null) {
                sVal=Content;
            } else {
                sVal=jQueryOne("#"+SourceId).val().toUpperCase();
            }
            if(sVal.length<2) {
                return;
            }
            var LBox=document.getElementById(sBox);
            var RBox=document.getElementById(tBox);
            if(LBox.options.length>0) {
                var IsValite=false;
                var len=LBox.options.length;
                var val="";
                var cityName='';
                if(sVal.indexOf("/")!= -1) {
                    var vArr=sVal.split("/");
                    for(var c=0;c<vArr.length;c++) {
                        for(var i=0;i<LBox.options.length;i++) {
                            val=LBox.options[i].text.split('_')[0].toUpperCase();
                            cityName=LBox.options[i].text.split('_')[1].toUpperCase(); //城市名
                            if(vArr[c]==val||vArr[c]==cityName) {
                                //找到                                        
                                RBox.options.add(new Option(LBox.options[i].text,val));
                                LBox.removeChild(LBox.options[i]);
                                IsValite=true;
                                break;
                            }
                        }
                    }
                    //jQueryOne("#" + SourceId).val(""); //不清空
                } else {
                    var cityName='';
                    for(var i=0;i<len;i++) {
                        val=LBox.options[i].text.split('_')[0].toUpperCase();
                        cityName=LBox.options[i].text.split('_')[1].toUpperCase(); //城市名
                        if(sVal==val||sVal==cityName) {
                            //找到
                            RBox.options.add(new Option(LBox.options[i].text,val));
                            LBox.removeChild(LBox.options[i]);
                            IsValite=true;
                            break;
                        }
                    }
                }
            }
            GetString(SourceId);
        }
        //用于排序
        function sortBy(arr,prop,desc) {
            var props=[],ret=[],i=0,len=arr.length;
            if(typeof prop=='string') {
                for(;i<len;i++) {
                    var oI=arr[i];
                    (props[i]=new String(oI&&oI[prop]||''))._obj=oI;
                }
            } else if(typeof prop=='function') {
                for(;i<len;i++) {
                    var oI=arr[i];
                    (props[i]=new String(oI&&prop(oI)||''))._obj=oI;
                }
            } else if(typeof prop=='number') {
                for(;i<len;i++) {
                    var oI=arr[i];
                    (props[i]=new String(oI&&oI[prop]||''))._obj=oI;
                }
            }
            else {
                throw '参数类型错误';
            }
            props.sort();
            for(i=0;i<len;i++) {
                ret[i]=props[i]._obj;
            }
            if(desc) ret.reverse();
            return ret;
        };
        //对下拉列表排序
        function selOptionSort(selId) {
            var RData=[];
            var SelObjArr=jQueryOne("#"+selId)[0].options;
            //排序
            RData=sortBy(SelObjArr,function (op) {
                return op.value;
            });
            SelObjArr.length=0;
            for(var i=0;i<RData.length;i++) {
                SelObjArr.add(RData[i]);
            }
            return SelObjArr;
        }
        //赋值
        function GetString(SourceId) {
            var val='';
            if(SourceId=="From_LeftBox"||SourceId=="From_RightBox"||SourceId=="txtFromCode"||SourceId=="txtFromCode_01") {
                //出发城市列表
                var FromSel=document.getElementById("From_RightBox");
                var LData=[],RData=[];
                //对左右两个列表排序
                LData=selOptionSort('From_LeftBox');
                RData=selOptionSort('From_RightBox');

                for(var i=0;i<LData.length;i++) {
                    val+=LData[i].value+"/";
                }
                //设置隐藏域值
                jQueryOne("#FromCityCode").val(val);
                //设置文本框值
                jQueryOne("#txtFromCode").val(val);

            }
            else if(SourceId=="Middle_RightBox"||SourceId=="Middle_LeftBox"||SourceId=="txtMiddleCity2"||SourceId=="txtMiddleCity_02") {
                //中转联程列表
                var FromSel=document.getElementById("Middle_LeftBox");
                var LData=[],RData=[];
                //对左右两个列表排序
                LData=selOptionSort('Middle_LeftBox');
                RData=selOptionSort('Middle_RightBox');

                for(var i=0;i<LData.length;i++) {
                    val+=LData[i].value+"/";
                }
                //设置隐藏域值
                jQueryOne("#MiddleCityCode").val(val);
                //设置文本框值
                jQueryOne("#txtMiddleCity2").val(val);
            } else if(SourceId=="To_LeftBox"||SourceId=="To_RightBox"||SourceId=="txtToCode"||SourceId=="txtToCode_02") {
                //到达城市列表
                var FromSel=document.getElementById("To_LeftBox");
                var LData=[],RData=[];
                //对左右两个列表排序
                LData=selOptionSort('To_LeftBox');
                RData=selOptionSort('To_RightBox');

                for(var i=0;i<LData.length;i++) {
                    val+=LData[i].value+"/";
                }
                //设置隐藏域值
                jQueryOne("#ToCityCode").val(val);
                //设置文本框值
                jQueryOne("#txtToCode").val(val);
            }
        }
        //结束乘机日期同步到出票结束日期
        function TicketFocus(n) {
            var num=n.id.NewReplace('txtFlightEndDate_','');
            WdatePicker(
            {
                isShowClear: false,
                isShowWeek: false,
                minDate: '%y-%M-%d',
                autoPickDate: true,
                dateFmt: 'yyyy-MM-dd',
                onpicked: function () {
                    $dp.$('txtTicketEndDate_'+num).value=$dp.cal.getP('y')+'-'+$dp.cal.getP('M')+'-'+$dp.cal.getP('d');
                }
            });
        }
        //----------------------------------------------------------------------                        
   //-->
    </script>
    <script type="text/javascript" src="../js/Policy/NewAddPolicy.js"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <%--城市控件使用容器开始--%>
    <div id="jsContainer">
        <div id="jsHistoryDiv" class="hide">
            <iframe id="jsHistoryFrame" name="jsHistoryFrame" src="about:blank"></iframe>
        </div>
        <textarea id="jsSaveStatus" class="hide"></textarea>
        <div id="tuna_jmpinfo" style="visibility: hidden; position: absolute; z-index: 120;
            overflow: hidden;">
        </div>
        <div id="tuna_alert" style="display: none; position: absolute; z-index: 999; overflow: hidden;">
        </div>
        <%--日期容器--%>
        <div style="position: absolute; top: 0; z-index: 120; display: none" id="tuna_calendar"
            class="tuna_calendar">
        </div>
    </div>
    <div id="showmsg">
    </div>
    <div id="Div1">
    </div>
    <div class="AP_div">
    </div>
    <%--城市控件使用容器结束--%>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="title">
        <asp:Label ID="lblShow" Text="普通政策发布" runat="server" />
    </div>
    <table style="width: 100%; border-collapse: collapse; border-color: #DFF0FD;" border="1"
        cellpadding="0" cellspacing="0" id="tabAll">
        <tr class="hide">
            <th class="w_td">
                <font style="color: red;">*</font>机票类型：
            </th>
            <td class="r-td">
                <div>
                    <span id="span_jipiaoInnerType">
                        <label for="jipiaoInnerType">
                            <input type="radio" id="jipiaoInnerType" value="0" name="jipiaoType" checked="checked"
                                onclick="SetJipiaoType(this.value)" />国内机票
                        </label>
                    </span><span id="span_jipiaoOuterType">
                        <label for="jipiaoOuterType">
                            <input type="radio" id="jipiaoOuterType" value="1" name="jipiaoType" onclick="SetJipiaoType(this.value)" />国际机票
                        </label>
                    </span>
                </div>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>发布类型：
            </th>
            <td class="r-td">
                <div>
                    <span id="span_fromPublish">
                        <label for="fromPublish">
                            <input type="radio" value="1" id="FromPublish" name="PublishGroup" checked="true"
                                onclick="setPublishType(this.value)" />出港</label>
                    </span><span id="span_ToPublish">
                        <label for="ToPublish">
                            <input type="radio" value="2" id="ToPublish" name="PublishGroup" onclick="setPublishType(this.value)" />入港</label>
                    </span><span id="span_AllPublish">
                        <label for="AllPublish">
                            <input type="radio" id="AllPublish" value="3" name="PublishGroup" onclick="setPublishType(this.value)" />全国</label>
                    </span>
                </div>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>行程类型：
            </th>
            <td class="r-td">
                <div>
                    <span id="span_oneTravel">
                        <label for="oneTravel">
                            <input type="radio" value="1" id="oneTravel" name="TravelGroup" checked="true" onclick="setTravelType(this.value)" />单程</label></span>
                    <span id="span_TwoTravel">
                        <label for="TwoTravel">
                            <input type="radio" value="3" id="TwoTravel" name="TravelGroup" onclick="setTravelType(this.value)" />往返</label></span>
                    <span id="span_threeTravel">
                        <label for="threeTravel">
                            <input type="radio" id="threeTravel" value="2" name="TravelGroup" onclick="setTravelType(this.value)" />单程/往返</label></span>
                    <span id="span_fourTravel">
                        <label for="fourTravel">
                            <input type="radio" id="fourTravel" value="4" name="TravelGroup" onclick="setTravelType(this.value)" />联程</label></span>
                    <span id="IsLowerOpenDiv">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="color: #ff0066;">*</span>是否低开：
                        <label for="cklowerOpen">
                            <input type="checkbox" id="cklowerOpen" />低开</label>
                    </span>
                </div>
            </td>
        </tr>
        <%--<tr id="tr_etdzType">
            <td class="w_td">
                <font style="color: red;">*</font>出票方式：
            </td>
            <td>
                <div>
                    <label for="etdz0">
                        <input type="radio" value="0" id="etdz0" name="ETDZType" checked="true" />手动</label>
                    <label for="etdz1">
                        <input type="radio" value="1" id="etdz1" name="ETDZType" />半自动</label>
                    <label for="etdz2">
                        <input type="radio" value="2" id="etdz2" name="ETDZType" />自动</label>
                </div>
            </td>
        </tr>--%>
        <tr id="tr_gaofan" class="hide">
            <td class="w_td">
                高返政策：
            </td>
            <td style="text-align: left;">
                <label for="ckIsGaoFan">
                    <input type="checkbox" id="ckIsGaoFan" />高返</label>
            </td>
        </tr>
        <%--出发城市--%>
        <tr id="tr_from1">
            <th class="w_td">
                <font style="color: red;">*</font>出发城市：
            </th>
            <td class="r-td">
                <%--出发城市开始--%>
                <input name="txtStart" class="inputtxtdat" type="text" id="txtFromCityName" mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                    style="width: 150px;" mod_address_reference="FromCityCode" />
                <%--出发城市结束--%>
                <span id="SameFrom">
                    <label for="FromSameCitySharePolicy">
                        <input type="checkbox" id="FromSameCitySharePolicy" /><font style="color: green;">同城机场共享此政策</font></label>
                </span>
            </td>
        </tr>
        <tr id="tr_from2" class="hide">
            <th class="w_td">
                <font style="color: red;">*</font>出发城市：
            </th>
            <td class="r-td" align="left">
                <input type="text" id="txtFromCode" runat="server" style="width: 300px;" autocomplete="off"
                    class="inputBorder" />
                <span class="btn btn-ok-s">
                    <input type="button" id="btnAdd1" runat="server" value="加入" autocomplete="off" onclick="KeyBoardInput('txtFromCode','From_RightBox','From_LeftBox')" /></span>
                <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="msg0"></span></span>
            </td>
        </tr>
        <tr id="tr_from2_01" class="hide">
            <th class="w_td">
            </th>
            <td class="r-td" align="left">
                <input type="text" id="txtFromCode_01" runat="server" style="width: 300px;" autocomplete="off"
                    class="inputBorder" />
                <span class="btn btn-ok-s">
                    <input type="button" id="btnfromPaiChu" runat="server" value="排除" autocomplete="off"
                        onclick="KeyBoardInput('txtFromCode_01','From_LeftBox','From_RightBox')" /></span>
                <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="Span1"></span></span>
            </td>
        </tr>
        <tr id="tr_from3" class="hide">
            <th class="w_td">
            </th>
            <td class="r-td">
                <table cellspacing="0" cellpadding="0" border="0" id="fromtab">
                    <tr>
                        <td align="center">
                            <font style="color: green; font-size: 15px;">已选城市</font>
                        </td>
                        <td>
                        </td>
                        <td align="center">
                            <font style="color: green; font-size: 15px;">待选城市</font>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:ListBox ID="From_LeftBox" size="15" runat="server" Style="width: 150px; height: 110px;"
                                ondblclick="dblclick('From_RightBox','From_LeftBox','0','rl')" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                        <td valign="middle" align="center" width="150px" style="padding-right: 5px; padding-left: 5px;
                            text-align: center;">
                            <span class="btn btn-ok-s">
                                <input type="button" id="from_btnAdd" runat="server" value="<<添加" class="input" onclick="dblclick('From_RightBox','From_LeftBox','1','lr')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="from_btnAddAll" runat="server" value="<<全部添加" class="input"
                                    onclick="dblclick('From_RightBox','From_LeftBox','2','lr')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="from_btnDelAll" runat="server" value="全部删除>>" class="input"
                                    onclick="dblclick('From_RightBox','From_LeftBox','2','rl')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="from_btnDel" runat="server" value="删除>>" class="input" onclick="dblclick('From_RightBox','From_LeftBox','1','rl')" /></span>
                        </td>
                        <td align="left">
                            <asp:ListBox ID="From_RightBox" size="15" runat="server" Style="width: 150px; height: 110px;"
                                ondblclick="dblclick('From_RightBox','From_LeftBox','0','lr')" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--中专城市--%>
        <tr id="tr_middle1" class="hide">
            <th class="w_td">
                <font style="color: red;">*</font>中转城市：
            </th>
            <td class="r-td">
                <%--到达城市开始--%>
                <input name="txtMiddleCityName" class="inputtxtdat" type="text" id="txtMiddleCityName"
                    mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                    style="width: 150px;" mod_address_reference="MiddleCityCode" />
                <%--到达城市结束--%>
                <span id="Span3">
                    <label for="MiddleSameCitySharePolicy">
                        <input type="checkbox" id="MiddleSameCitySharePolicy" /><font style="color: green;">同城机场共享此政策</font></label>
                </span>
            </td>
        </tr>
        <tr id="tr_middle2" class="hide">
            <th class="w_td">
                <font style="color: red;">*</font>中转城市：
            </th>
            <td class="r-td">
                <input type="text" id="txtMiddleCity2" runat="server" style="width: 300px;" autocomplete="off"
                    class="inputBorder" />
                <span class="btn btn-ok-s">
                    <input type="button" id="btnAdd2" runat="server" value="加入" autocomplete="off" onclick="KeyBoardInput('txtMiddleCity2','Middle_RightBox','Middle_LeftBox')" /></span>
                <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="Span4"></span></span>
            </td>
        </tr>
        <tr id="tr_middle_02" class="hide">
            <th class="w_td">
            </th>
            <td class="r-td">
                <input type="text" id="txtMiddleCity_02" runat="server" style="width: 300px;" autocomplete="off"
                    class="inputBorder" />
                <span class="btn btn-ok-s">
                    <input type="button" id="btnPaichu2" runat="server" value="排除" autocomplete="off"
                        onclick="KeyBoardInput('txtMiddleCity_02','Middle_LeftBox','Middle_RightBox')" /></span>
                <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="Span5"></span></span>
            </td>
        </tr>
        <tr id="tr_middle3" class="hide">
            <th class="w_td">
            </th>
            <td class="r-td">
                <table cellspacing="0" cellpadding="0" border="0" id="middletab" style="display: block;">
                    <tr>
                        <td align="center">
                            <font style="color: green; font-size: 15px;">已选城市</font>
                        </td>
                        <td>
                        </td>
                        <td align="center">
                            <font style="color: green; font-size: 15px;">待选城市</font>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:ListBox ID="Middle_LeftBox" size="15" runat="server" Style="width: 150px; height: 110px;"
                                ondblclick="dblclick('Middle_RightBox','Middle_LeftBox','0','rl')" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                        <td valign="middle" align="center" width="150px" style="padding-right: 5px; padding-left: 5px;
                            text-align: center;">
                            <span class="btn btn-ok-s">
                                <input type="button" id="middle_btnAdd" runat="server" value="<<添加" class="input"
                                    onclick="dblclick('Middle_RightBox','Middle_LeftBox','1','lr')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="middle_btnAddAll" runat="server" value="<<全部添加" class="input"
                                    onclick="dblclick('Middle_RightBox','Middle_LeftBox','2','lr')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="middle_btnDelAll" runat="server" value="全部删除>>" class="input"
                                    onclick="dblclick('Middle_RightBox','Middle_LeftBox','2','rl')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="middle_btnDel" runat="server" value="删除>>" class="input"
                                    onclick="dblclick('Middle_RightBox','Middle_LeftBox','1','rl')" /></span>
                        </td>
                        <td align="left">
                            <asp:ListBox ID="Middle_RightBox" size="15" runat="server" Style="width: 150px; height: 110px;"
                                ondblclick="dblclick('Middle_RightBox','Middle_LeftBox','0','lr')" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--到达城市--%>
        <tr id="tr_to1" class="hide">
            <th class="w_td">
                <font style="color: red;">*</font>到达城市：
            </th>
            <td class="r-td">
                <%--到达城市开始--%>
                <input name="ToCityName" class="inputtxtdat" type="text" id="ToCityName" mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                    mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                    style="width: 150px;" mod_address_reference="ToCityCode" />
                <%--到达城市结束--%>
                <span id="SameTo">
                    <label for="ToSameCitySharePolicy">
                        <input type="checkbox" id="ToSameCitySharePolicy" /><font style="color: green;">同城机场共享此政策</font></label>
                </span>
            </td>
        </tr>
        <tr id="tr_to2">
            <th class="w_td">
                <font style="color: red;">*</font>到达城市：
            </th>
            <td class="r-td">
                <input type="text" id="txtToCode" runat="server" style="width: 300px;" autocomplete="off"
                    class="inputBorder" />
                <span class="btn btn-ok-s">
                    <input type="button" id="btnAdd3" runat="server" value="加入" autocomplete="off" onclick="KeyBoardInput('txtToCode','To_RightBox','To_LeftBox')" /></span>
                <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="msg1"></span></span>
            </td>
        </tr>
        <tr id="tr_to2_01">
            <th class="w_td">
            </th>
            <td class="r-td">
                <input type="text" id="txtToCode_02" runat="server" style="width: 300px;" autocomplete="off"
                    class="inputBorder" />
                <span class="btn btn-ok-s">
                    <input type="button" id="btnToPaiChu" runat="server" value="排除" autocomplete="off"
                        onclick="KeyBoardInput('txtToCode_02','To_LeftBox','To_RightBox')" /></span>
                <span><font style="color: green;">如:CTU/PEK/HGH</font><span id="Span2"></span></span>
            </td>
        </tr>
        <tr id="tr_to3">
            <th class="w_td">
            </th>
            <td class="r-td">
                <table cellspacing="0" cellpadding="0" border="0" id="totab" style="display: block;">
                    <tr>
                        <td align="center">
                            <font style="color: green; font-size: 15px;">已选城市</font>
                        </td>
                        <td>
                        </td>
                        <td align="center">
                            <font style="color: green; font-size: 15px;">待选城市</font>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:ListBox ID="To_LeftBox" size="15" runat="server" Style="width: 150px; height: 110px;"
                                ondblclick="dblclick('To_RightBox','To_LeftBox','0','rl')" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                        <td valign="middle" align="center" width="150px" style="padding-right: 5px; padding-left: 5px;
                            text-align: center;">
                            <span class="btn btn-ok-s">
                                <input type="button" id="to_btnAdd" runat="server" value="<<添加" class="input" onclick="dblclick('To_RightBox','To_LeftBox','1','lr')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="to_btnAddAll" runat="server" value="<<全部添加" class="input"
                                    onclick="dblclick('To_RightBox','To_LeftBox','2','lr')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="to_btnDelAll" runat="server" value="全部删除>>" class="input"
                                    onclick="dblclick('To_RightBox','To_LeftBox','2','rl')" /></span><br />
                            <span class="btn btn-ok-s">
                                <input type="button" id="to_btnDel" runat="server" value="删除>>" class="input" onclick="dblclick('To_RightBox','To_LeftBox','1','rl')" /></span>
                        </td>
                        <td align="left">
                            <asp:ListBox ID="To_RightBox" size="15" runat="server" Style="width: 150px; height: 110px;"
                                ondblclick="dblclick('To_RightBox','To_LeftBox','0','lr')" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <span style="color: #ff0066;">*</span>班期限制：
            </th>
            <td class="r-td">
                <div style="float: left">
                    <label for="day0">
                        <input type="checkbox" id="day0" name="banqiGroup" value="7" />星期日</label>
                    <label for="day1">
                        <input type="checkbox" id="day1" name="banqiGroup" value="1" />星期一</label>
                    <label for="day2">
                        <input type="checkbox" id="day2" name="banqiGroup" value="2" />星期二</label>
                    <label for="day3">
                        <input type="checkbox" id="day3" name="banqiGroup" value="3" />星期三</label>
                    <label for="day4">
                        <input type="checkbox" id="day4" name="banqiGroup" value="4" />星期四</label>
                    <label for="day5">
                        <input type="checkbox" id="day5" name="banqiGroup" value="5" />星期五</label>
                    <label for="day6">
                        <input type="checkbox" id="day6" name="banqiGroup" value="6" />星期六</label>
                </div>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <span style="color: Green;">说明:勾选上后限制政策不生效，默认全部生效</span>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                适用航班：
            </th>
            <td class="r-td">
                <div>
                    <label for="rdAll">
                        <input type="radio" id="rdAll" name="FlightNumGroup" value="1" checked="true" onclick="setFlightNumGroup(this)" />适用所有航班</label>
                    <label for="rdInclude">
                        <input type="radio" id="rdInclude" name="FlightNumGroup" value="2" onclick="setFlightNumGroup(this)" />仅适用于以下航班</label>
                    <label for="rdExclude">
                        <input type="radio" id="rdExclude" name="FlightNumGroup" value="3" onclick="setFlightNumGroup(this)" />不适用于以下航班</label>
                </div>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                &nbsp;
            </th>
            <td class="r-td">
                <textarea id="txtFlightValue" disabled="true" maxlength="800" class="textAreaFlight"
                    style="height: 30px; border: 1; background-color: #D4D0C8; overflow: hidden;"
                    onblur="FlightNumberVate()"></textarea>
                <br />
                <span style="color: Green;">航班号只能为数字多航班输入请用&quot;/&quot;隔开,如：1126/3695 <font style="color: Red;">
                    *</font>为通配符, 如:11**这样可以匹配1189，1147等</span>
            </td>
        </tr>
        <%--特价区域--%>
        <tr id="tr_GenerationType" class="hide">
            <td class="w_td">
                <span style="color: #ff0066;">*</span> 票价生成方式：
            </td>
            <td>
                <div>
                    <span id="span_tjtype0">
                        <label for="tjtype0">
                            <input type="radio" value="1" id="tjtype0" name="TJGenerationType" onclick="setGenerationType(this.value)"
                                checked="true" />正常价格<span style="color: Green;">（只以PAT:A价格为依据）</span></label></span>
                    <span id="span_tjtype1">
                        <label for="tjtype1">
                            <input type="radio" value="2" id="tjtype1" name="TJGenerationType" onclick="setGenerationType(this.value)" />动态特价<span
                                style="color: Green;">（只以PAT:A价格为依据）</span></label></span> <span id="span_tjtype2">
                                    <label for="tjtype2">
                                        <input type="radio" value="3" id="tjtype2" name="TJGenerationType" onclick="setGenerationType(this.value)" />固定特价<span
                                            style="color: Green;">（先PAT:A,没有票价时以设置的折扣为依据）</span></label></span>
                    <br />
                    <span style="color: Green;">如果您选择第二种票价生成方式,因价格错误而导致的损失,自行承担！</span>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div id="pGroup_0">
                    <table id="policyC_0" width="100%" border="0">
                        <%--政策区域开始--%>
                        <tr id="policy_tr0">
                            <th class="w_td">
                                舱位与政策:
                            </th>
                            <td class="r-td" style="text-align: left;">
                                <table id="tabContainer_0" width="100%" border="0">
                                    <tr>
                                        <td>
                                            <div id="Container_0" class="tdBorder">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td colspan="2" class="tdBorder">
                                                            <table style="width: 100%;">
                                                                <tr>
                                                                    <th>
                                                                        航空公司:
                                                                    </th>
                                                                    <td style="width: 200px;">
                                                                        <input type="text" id="txtAirCode_0" style="width: 30px;" autocomplete="off" onpropertychange="SetSel(this,0)"
                                                                            oninput="SetSel(this,0)" maxlength="2" class="inputBorder" />
                                                                        <select id="Select_0" style="width: 150px;" onchange="SetText(this,0)">
                                                                        </select>
                                                                    </td>
                                                                    <td style="width: 90px;">
                                                                        <div id="TJshow_0" class="hide">
                                                                            <label for="TJCK_0">
                                                                                <input type="checkbox" id="TJCK_0" name="TJCK_0" class="inputBorder" onclick="showDiv(0)" /><b>含有特价舱位</b></label>
                                                                        </div>
                                                                    </td>
                                                                    <th>
                                                                        Office：
                                                                    </th>
                                                                    <td>
                                                                        <input type="text" id="txtOffice_0" class="tdw120 inputBorder" maxlength="6" />
                                                                    </td>
                                                                    <th>
                                                                        政策类型:
                                                                    </th>
                                                                    <td>
                                                                        <select id="SelPolicyType_0" style="width: 120px;" onchange="SetAutoHide(this,0)">
                                                                            <option value="1" selected="true">B2B</option>
                                                                            <option value="2">BSP</option>
                                                                            <%-- <option value="3">B2B/BSP</option>--%>
                                                                        </select>
                                                                    </td>
                                                                    <td>
                                                                        &nbsp;
                                                                    </td>
                                                                </tr>
                                                                <tr id="tr_etdztype_0">
                                                                    <th>
                                                                        出票方式:
                                                                    </th>
                                                                    <td>
                                                                        <div id="divetdztype_0">
                                                                            <label for="etdz0_0">
                                                                                <input type="radio" value="0" id="etdz0_0" name="etdztype_0" checked="true" />手动</label>
                                                                            <label for="etdz1_0">
                                                                                <input type="radio" value="1" id="etdz1_0" name="etdztype_0" />半自动</label>
                                                                            <label for="etdz2_0">
                                                                                <input type="radio" value="2" id="etdz2_0" name="etdztype_0" />自动</label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr style="background-color: #DFF0FD;">
                                                        <td align="center" class="tdBorder">
                                                            <b>舱位</b>
                                                        </td>
                                                        <td align="center" class="tdBorder">
                                                            <b>政策</b>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 661px;" valign="top" class="tdBorder" id="td_cangwei_0">
                                                            <div id="seat_0" style="min-height: 70px!important; width: 100%;" class="tdBorder">
                                                            </div>
                                                            <div id="TJSpace_0" style="min-height: 50px!important; width: 100%; border-bottom: 0px solid #C5DBF2;"
                                                                class="tdBorder hide">
                                                            </div>
                                                        </td>
                                                        <td valign="top" class="tdBorder">
                                                            <div id="policy_0">
                                                                <table style="width: 100%;" class="tdBorder">
                                                                    <tr>
                                                                        <th class="tdBorder" width="25%">
                                                                            下级分销返点%
                                                                        </th>
                                                                        <th class="tdBorder" width="25%">
                                                                            下级分销后返%
                                                                        </th>
                                                                        <th class="tdBorder" width="50%">
                                                                            共享政策返点%
                                                                        </th>
                                                                    </tr>
                                                                    <tr>
                                                                        <th class="tdBorder" width="25%">
                                                                            <input id="DownPoint_0" type="text" value="0" style="width: 90px;" maxlength="9"
                                                                                fanwei="0-100" />
                                                                        </th>
                                                                        <th class="tdBorder" width="25%">
                                                                            <input id="LaterPoint_0" type="text" value="0" style="width: 90px;" maxlength="9"
                                                                                fanwei="0-100" />
                                                                        </th>
                                                                        <th class="tdBorder" width="50%">
                                                                            <input id="SharePoint_0" type="text" value="0" style="width: 90px;" maxlength="9"
                                                                                fanwei="0-100" />
                                                                        </th>
                                                                    </tr>
                                                                    <tr>
                                                                        <th class="tdBorder" width="25%">
                                                                            下级分销现返现金￥
                                                                        </th>
                                                                        <th class="tdBorder" width="25%">
                                                                            下级分销后返现金￥
                                                                        </th>
                                                                        <th class="tdBorder" width="50%">
                                                                            共享政策现返现金￥
                                                                        </th>
                                                                    </tr>
                                                                    <tr>
                                                                        <th class="tdBorder" width="25%">
                                                                            <input id="DownReturnMoney_0" type="text" value="0" style="width: 90px;" maxlength="9" />
                                                                        </th>
                                                                        <th class="tdBorder" width="25%">
                                                                            <input id="LaterReturnMoney_0" type="text" value="0" style="width: 90px;" maxlength="9" />
                                                                        </th>
                                                                        <th class="tdBorder" width="50%">
                                                                            <input id="ShareReturnMoney_0" type="text" value="0" style="width: 90px;" maxlength="9" />
                                                                        </th>
                                                                    </tr>
                                                                </table>
                                                                <table style="width: 100%;" class="tdBorder hide" id="tjTab_0">
                                                                    <tr>
                                                                        <th class="tdBorder" width="25%">
                                                                            固定价格￥
                                                                        </th>
                                                                        <th class="tdBorder" width="25%">
                                                                            参考价格￥
                                                                        </th>
                                                                        <td rowspan="2" width="50%">
                                                                            <div id="msg_0">
                                                                                &nbsp;
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <th class="tdBorder" width="25%">
                                                                            <input id="txtTJGuding_0" type="text" value="0" style="width: 90px;" maxlength="9" />
                                                                        </th>
                                                                        <th class="tdBorder" width="25%">
                                                                            <input id="txtTJcankao_0" type="text" value="0" style="width: 90px;" maxlength="9" />
                                                                        </th>
                                                                    </tr>
                                                                </table>
                                                                &nbsp;
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                                &nbsp;
                                                <table style="width: 100%;">
                                                    <tr class="tdBorder">
                                                        <th>
                                                            乘机日期：
                                                        </th>
                                                        <td class="tdBorder" style="text-align: left;">
                                                            <div>
                                                                <input type="text" id="txtFlightStartDate_0" style="width: 130px;" readonly="true"
                                                                    runat="server" class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />至
                                                                <input type="text" id="txtFlightEndDate_0" style="width: 130px;" readonly="true"
                                                                    runat="server" class="inputBorder" onfocus="TicketFocus(this)" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr class="tdBorder">
                                                        <th>
                                                            出票日期：
                                                        </th>
                                                        <td class="tdBorder" style="text-align: left;">
                                                            <table border="0">
                                                                <tr>
                                                                    <td>
                                                                        <div>
                                                                            <input type="text" id="txtTicketStartDate_0" style="width: 130px;" readonly="true"
                                                                                class="inputBorder" runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />至
                                                                            <input type="text" id="txtTicketEndDate_0" style="width: 130px;" readonly="true"
                                                                                runat="server" class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                                                                        </div>
                                                                    </td>
                                                                    <td>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;
                                                                    </td>
                                                                    <th>
                                                                        提前天数:
                                                                    </th>
                                                                    <td>
                                                                        <input type="text" id="txtAdvanceDay_0" style="width: 130px;" class="inputBorder"
                                                                            value="0" runat="server" maxlength="9" />
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                &nbsp;
                                            </div>
                                        </td>
                                        <td>
                                            <div id="optionDiv_0">
                                                <span class="btn btn-ok-s">
                                                    <input id="btnAdd_0" type="button" value="添加" onclick="AddTrGroup(this,event)" /></span>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <%--政策区域结束--%>
        <tr id="tr2" class="hide">
            <th class="w_td">
                <span style="color: #ff0066;">*</span>乘机日期：
            </th>
            <td class="r-td" style="text-align: left;">
                <div>
                    <input type="text" id="txtFlightStartDate" style="width: 130px;" readonly="true"
                        runat="server" class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />至
                    <input type="text" id="txtFlightEndDate" style="width: 130px;" readonly="true" runat="server"
                        class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd',onpicked:function() {$dp.$('txtTicketEndDate').value=$dp.cal.getP('y')+'-'+$dp.cal.getP('M')+'-'+$dp.cal.getP('d');}})" />
                </div>
            </td>
        </tr>
        <tr id="tr3" class="hide">
            <th class="w_td">
                <span style="color: #ff0066;">*</span>出票日期：
            </th>
            <td class="r-td" style="text-align: left;">
                <table border="0">
                    <tr>
                        <td>
                            <div>
                                <input type="text" id="txtTicketStartDate" style="width: 130px;" readonly="true"
                                    class="inputBorder" runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />至
                                <input type="text" id="txtTicketEndDate" style="width: 130px;" readonly="true" runat="server"
                                    class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                            </div>
                        </td>
                        <td>
                            提前天数:
                        </td>
                        <td>
                            <input type="text" id="txtAdvanceDay" style="width: 130px;" class="inputBorder" value="0"
                                runat="server" />
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="tr_HangReturn">
            <th class="w_td">
                航空公司返点：
            </th>
            <td class="r-td">
                <input type="text" id="txtAirReBate" value="0" class="inputBorder" fanwei="0-100" />%
            </td>
        </tr>
        <tr id="tr_HangMoney">
            <th class="w_td">
                航空公司现返：
            </th>
            <td class="r-td">
                <input type="text" id="txtAirReturnMoney" value="0" class="inputBorder" />￥
            </td>
        </tr>
        <tr id="tr_tuan">
            <th class="w_td">
                团队标志：
            </th>
            <td class="r-td">
                <%--团队标志 0.普通，1.团队--%>
                <label for="txtTeamFlagCK">
                    <input type="checkbox" id="txtTeamFlagCK" /></label>
            </td>
        </tr>
        <tr id="IsAudit">
            <th class="w_td">
                <span style="color: #ff0066;">*</span>是否审核：
            </th>
            <td class="r-td" style="text-align: left;">
                <label for="CkIsAudit">
                    <input type="checkbox" id="ck_AuditType" />审核</label>
            </td>
        </tr>
        <tr class="hide">
            <th class="w_td">
                <span style="color: #ff0066;">*</span>是否适用儿童：
            </th>
            <td class="r-td" style="text-align: left;">
                <label for="rdChild1">
                    <input id="rdChild1" type="radio" value="1" name="rdChild" runat="server" />适用</label>
                <label for="rdChild2">
                    <input id="rdChild2" type="radio" value="0" name="rdChild" runat="server" checked="true" />不适用</label>
            </td>
        </tr>
        <tr id="tr_IsApplyAir">
            <th class="w_td">
                <span style="color: #ff0066;">*</span>共享航班：
            </th>
            <td class="r-td" style="text-align: left;">
                <ul class="ulClass">
                    <li style="float: left;">
                        <label for="rdFlight1">
                            <input id="rdFlight1" type="radio" name="Flight" value="1" runat="server" onclick="ApplyCarry(1)" />适用</label></li>
                    <li style="float: left;">
                        <label for="rdFlight2">
                            <input id="rdFlight2" type="radio" name="Flight" value="0" runat="server" checked="true"
                                onclick="ApplyCarry(0)" />不适用</label>
                    </li>
                    <li class="hide" id="liApplyTo">
                        <label for="txtApplyToFlightNo">
                            &nbsp;&nbsp;&nbsp;共享航空二字码:
                            <input type="text" id="txtApplyToFlightNo" style="width: 400px;" class="TextBorder" /><font
                                style="color: Green;">如: CA/CZ/HU/MU 用"/"隔开</font>
                        </label>
                    </li>
                </ul>
            </td>
        </tr>
        <%--  <tr id="trhf0">
            <td class="w_td">
                <span style="color: #ff0066;">*</span>特殊标识：
            </td>
            <td>
                <table border="0">
                    <tr>
                        <td>
                            <label for="hfOpen">
                                <input type="checkbox" id="hfOpen" name="HouFanGroup" onclick="HFCheck(0)" />开启</label>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;&nbsp;<input type="text" id="inputHouFan" value="0" onkeyup="KeyUp(event,this.value)" />
                        </td>
                        <td>
                            <span id="LevelMsg"></span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>--%>
        <tr>
            <th class="w_td">
                政策备注：
            </th>
            <td class="r-td" style="text-align: left;">
                <textarea id="txtPolicyRemark" cols="60" rows="5" class="inputBorder" maxlength="500"
                    style="width: 600px;"></textarea>
            </td>
        </tr>
        <tr>
            <td class="r-td" align="center" colspan="2">
                <span class="btn btn-ok-s">
                    <input type="button" value="添加并继续" onclick="AddPolicy()" id="addAndNext" runat="server" /></span>
                <span class="btn btn-ok-s">
                    <input type="button" value="返回" onclick="ReGo()" /></span>
            </td>
        </tr>
    </table>
    <%--机票类型 0国内 1国际--%>
    <input type="hidden" id="Hid_jipiaoType" value="0" />
    <%--发布类型--%>
    <input type="hidden" id="ReleaseType" value="1" />
    <%--行程类型--%>
    <input type="hidden" id="TravelType" value="1" />
    <%--同城共享 1是 2否--%>
    <input type="hidden" id="CityNameSame" value="2" />
    <input type="hidden" id="FromCityCode" />
    <input type="hidden" id="MiddleCityCode" />
    <input type="hidden" id="ToCityCode" />
    <%--班期--%>
    <input type="hidden" id="Schedule" />
    <input type="hidden" id="FlightType" value="1" />
    <input type="hidden" id="FlightValue" value="" />
    <%--是否--%>
    <input type="hidden" id="IsApplyToChild" value="0" />
    <input type="hidden" id="IsApplyToShareFlight" value="0" />
    <input type="hidden" id="IsAuditVal" value="2" />
    <input type="hidden" id="IsLowerOpen" value="0" />
    <%--保存航空公司--%>
    <input type="hidden" id="Hid_AirCodeCache" runat="server" />
    <%--Office--%>
    <input type="hidden" id="Hid_Office" runat="server" />
    <%--策略组--%>
    <input type="hidden" id="Hid_StrValue" />
    <input type="hidden" id="Hid_AllCode" value="0" />
    <%--编辑部分 1编辑 0添加--%>
    <input type="hidden" id="Hid_IsEdit" value="0" runat="server" />
    <input type="hidden" id="Hid_PolicyId" value="0" runat="server" />
    <input type="hidden" id="Hid_EditSeat" runat="server" />
    <input type="hidden" id="Hid_FangShi" value="1" runat="server" />
    <input type="hidden" id="Hid_AirCode" runat="server" />
    <%--列表来源查询条件 用于返回列表--%>
    <input type="hidden" id="Hid_where" runat="server" />
    <input type="hidden" id="Hid_currPage" runat="server" />
    <input type="hidden" id="Hid_id" runat="server" />
    <input type="hidden" id="Hid_EditData" runat="server" />
    <%--显示权限--%>
    <input type="hidden" id="Hid_showStreay" value="0" />
    <input type="hidden" id="Hid_showHangReturn" value="0" />
    <%--政策类型--%>
    <input type="hidden" id="Hid_PolicyType" value="1" runat="server" />
    <%--国内城市数据 ctu-成都--%>
    <input type="hidden" id="Hid_InnerCityData" runat="server" />
    <%--国际城市数据 --%>
    <input type="hidden" id="Hid_OuterCityData" runat="server" />
    <%--是否高返--%>
    <input type="hidden" id="Hid_IsGaoFan" runat="server" />
    <%--自动出票方式--%>
    <input type="hidden" id="Hid_etdz" runat="server" value="0" />
    <%--后返--%>
    <input type="hidden" id="Hid_Houfan" runat="server" value="0" />
    <%--是普通政策页面还是高反政策页面 0普通 1高反--%>
    <input type="hidden" id="Hid_IsGaoPage" runat="server" value="0" />
    <%--后返权限--%>
    <input type="hidden" id="Hid_quanxian" runat="server" />
    <%--是否开启高返权限 1是 0否--%>
    <input type="hidden" id="Hid_IsOpenGF" runat="server" value="0" />
    <%--后返开关选项--%>
    <input type="hidden" id="Hid_hfOpen" runat="server" value="1" />
    <%--公司编号--%>
    <input type="hidden" id="Hid_CpyNo" runat="server" />
    <%--供应商名字--%>
    <input type="hidden" id="Hid_CpyName" runat="server" />
    <%--登录账号 --%>
    <input type="hidden" id="Hid_LoginName" runat="server" />
    <%--政策种类 1.普通，2.特价--%>
    <input type="hidden" id="Hid_PolicyKind" runat="server" value="1" />
    <%--票价生成方式 1.正常价格，2.动态特价，3.固定特价--%>
    <input type="hidden" id="Hid_GenerationType" runat="server" value="1" />
    <%--承运人 航空公司编号--%>
    <input type="hidden" id="Hid_CarryCode" runat="server" />
    <%--专属扣点组ID --%>
    <input type="hidden" id="Hid_GroupId" runat="server" />
    <%--发布页面类型 1普通政策 2特价政策 3默认政策 4散冲团政策 5团政策 6特殊舱位政策--%>
    <input type="hidden" id="Hid_PageType" runat="server" value="0" />
    <%--添加或者删除表格行id--%>
    <input type="hidden" id="Hid_dyncId" runat="server" value="1" />
    <%--脚本执行--%>
    <asp:Literal ID="LiterScript" runat="server"></asp:Literal>
    <asp:Literal ID="LiterScriptGobal" runat="server"></asp:Literal>
    <script type="text/javascript" language="javascript">
        //初始化数据
        jQueryOne(function () {
            ReLoad();
        });     
    </script>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddGroupPolicy.aspx.cs" Inherits="Policy_AddGroupPolicy" %>

<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<%@ Register Src="../UserContrl/TimeCtrl.ascx" TagName="TimeCtrl" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>散冲团政策</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">
    <!--
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
        function IsReTurnList() {
            //页面类型  1普通政策 2特价政策 3默认政策 4散冲团政策
            var PageType=jQuery("#Hid_PageType").val();
            //是否编辑 1是 0否
            var IsEdit=jQuery("#Hid_IsEdit").val();
            if(IsEdit=="1") {
                var Isgo=jQuery('#showmsg input[name="IsGoList"][type="radio"]:checked').val();
                if(Isgo=="1") {
                    //返回列表                               
                    var cid=jQuery("#Hid_id").val();
                    var pid=jQuery("#Hid_currPage").val();
                    var where=jQuery("#Hid_where").val();//条件
                    location.href="GroupPolicyList.aspx?PageType="+PageType+"&edit="+IsEdit+"&cid="+cid+"&pid="+pid+"&where="+where+"&currentuserid=<%=this.mUser.id.ToString() %>";
                }
            }
        }
        function showdialog(t,f) {
            jQuery("#showmsg").html(t);
            jQuery("#showmsg").dialog({
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
                        jQuery(this).dialog('close');
                    }
                }
            });
        }
        //数字验证【0-100.00】
        function NumVate() {
            try {
                var value=jQuery.trim(jQuery(this).val());
                if(value==null||value=='') {
                    showdialog("文本框不能为空,请输入正确的数字!");
                    jQuery(this).val("0");
                    return false;
                }
                var ctrlId=jQuery(this).attr("id");
                //数据范围0-100
                var NumFanWei=jQuery(this).attr("FanWei");
                if(!isNaN(value)) {
                    var userreg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
                    if(userreg.test(value)) {
                        //验证通过
                        if(parseFloat(value).toString().length>5) {
                            jQuery(this).val("0");
                            showdialog("输入数据超出范围!");
                            return false;
                        }
                        var idArr=[
                       'txtDownRebate',
                       'txtLaterRebate',
                       'txtShareRebate ',
                       'txtAirRebate',
                       'txtURebate'
                       ]
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
                                    jQuery(this).val("0");
                                    showdialog("输入数据超出范围["+NumFanWei+"]!");
                                    return false;
                                }
                            }
                        }

                    } else {
                        var numindex=parseInt(value.indexOf("."),10);
                        if(numindex==0) {
                            jQuery(this).val("0");
                            showdialog("输入的数字不规范");
                            return false;
                        }
                        var head=value.substring(0,numindex);
                        var bottom=value.substring(numindex,numindex+3);
                        var fianlNum=head+bottom;
                        jQuery(this).val(fianlNum);
                    }
                } else {
                    jQuery(this).val("0");
                    showdialog("请输入数字");
                    return false;
                }

                if(NumFanWei!=null) {
                    var x=parseFloat(value,10);
                    var s=ShowPoint(x,1);//保留一位小数   
                    jQuery(this).val(s);
                } else {
                    var x=parseInt(value,10);
                    jQuery(this).val(x);
                }
            } catch(e) {
                alert(e.message);
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
        //返回按钮
        function ReGo() {
            var PageType=jQuery("#Hid_PageType").val();
            location.href="GroupPolicyList.aspx?PageType="+PageType+"&currentuserid=<%=this.mUser.id.ToString() %>";
        }
        //优惠方式
        function setPriceType(_val) {
            var val=_val;
            if(val!=undefined&&val!=null) {
                if(val=="0") {
                    //按折扣
                    jQuery("#tr_rebate").show();
                    jQuery("#tr_price").hide();
                } else if(val=="1") {
                    //按价格
                    jQuery("#tr_rebate").hide();
                    jQuery("#tr_price").show();
                }
                jQuery("input[name='UPriceType'][type='radio'][value='"+val+"']").attr("checked",true);
            }
            else {
                val=jQuery("input[name='UPriceType'][type='radio']:checked").val();
            }
            jQuery("#Hid_PriceType").val(val);
        }
        function setPolicyType(_val) {
            var val=_val;
            if(val!=undefined&&val!=null) {
                //设置
                jQuery("input[name='UPolicyType'][type='radio'][value='"+val+"']").attr("checked",true);

            } else {
                //获取
                val=jQuery("input[name='UPolicyType'][type='radio']:checked").val();
            }
            jQuery("#Hid_UPolicyType").val(val);
        }
        //日期大小比较 返回true false
        function CompareDate(sdate,edate) {
            var strDate1=sdate.split('-');
            var date1;
            if(strDate1.length==3) {
                date1=new Date(strDate1[0],strDate1[1],strDate1[2]);
            }
            var strDate2=edate.split('-');
            var date2;
            if(strDate1.length==3) {
                date2=new Date(strDate2[0],strDate2[1],strDate2[2]);
            }
            return (date1>date2);
        }
        function AddPolicy() {
            try {
                var AirCode=jQuery("#Hid_AirCode").val();
                var fromCode=jQuery("#Hid_fromCode").val();
                var toCode=jQuery("#Hid_toCode").val();
                var Seat=jQuery("#Hid_Seat").val();
                var PriceType=jQuery("#Hid_PriceType").val();
                var PolicyType=jQuery("#Hid_UPolicyType").val();
                var PlaneType=jQuery("#Hid_PlaneType").val();

                //航班号
                var FlightNo=jQuery("#txtFlightNo").val();
                //起飞抵达时间
                var FlightTime=GetValue_TimeCtrl1();
                //机建
                var AircraftFare=jQuery("#txtAircraftFare").val();
                //燃油
                var RQFare=jQuery("#txtRQFare").val();
                //折扣
                var Rebate=jQuery("#txtURebate").val();
                //价格
                var Prices=jQuery("#txtPrices").val();
                //下级返点
                var DownRebate=jQuery("#txtDownRebate").val();
                //下级后返
                var LaterRebate=jQuery("#txtLaterRebate").val();
                //共享返点
                var ShareRebate=jQuery("#txtShareRebate").val();
                //航空公司返点
                var AirRebate=jQuery("#txtAirRebate").val();

                //政策有效期
                var StartDate=jQuery("#txtStartDate").val();
                var EndDate=jQuery("#txtEndDate").val();
                //出票日期
                var TicketStartDate=jQuery("#txtTicketStartDate").val();
                var TicketEndDate=jQuery("#txtTicketEndDate").val();

                var AdvanceDays=jQuery("#txtAdvanceDays").val();
                var SeatCount=jQuery("#txtSeatCount").val();
                var OfficeCode=jQuery("#txtOfficeCode").val();
                var Remark=jQuery("#txtUPolicyRemark").val();

                if(jQuery.trim(AirCode)=="") {
                    showdialog("请选择航空公司！");
                    return false;
                }
                if(jQuery.trim(fromCode)=="") {
                    showdialog("请选择出发城市！");
                    return false;
                }
                if(jQuery.trim(toCode)=="") {
                    showdialog("请选择到达城市！");
                    return false;
                }
                if(jQuery.trim(fromCode)==jQuery.trim(toCode)) {
                    showdialog("出发城市与到达城市不能相同！");
                    return false;
                }
                if(jQuery.trim(FlightNo)=="") {
                    showdialog("请输入航班号！");
                    return false;
                }
                if(jQuery.trim(Seat)=="") {
                    showdialog("请选择舱位！");
                    return false;
                }

                var flighttime=FlightTime.split('-');
                if(FlightTime=="00:00:00-00:00:00") {
                    showdialog("请选择起飞抵达时间！");
                    return false;
                } else {
                    if(flighttime.length==2) {
                        if(flighttime[0]=="00:00:00") {
                            showdialog("请选择起飞时间！！");
                            return false;
                        } else if(flighttime[1]=="00:00:00") {
                            showdialog("请选择抵达时间！");
                            return false;
                        }
                    }
                }
                if(jQuery.trim(PlaneType)=="") {
                    showdialog("请选择机型！");
                    return false;
                }
                if(jQuery.trim(AircraftFare)=="") {
                    showdialog("请输入机建费！");
                    return false;
                }
                if(jQuery.isNaN(AircraftFare)) {
                    showdialog("输入机建费格式错误！");
                    return false;
                }
                if(jQuery.trim(RQFare)=="") {
                    showdialog("请输入燃油！");
                    return false;
                }
                if(jQuery.isNaN(RQFare)) {
                    showdialog("输入燃油费格式错误！");
                    return false;
                }
                if(PriceType=="0") {
                    if(jQuery.trim(Rebate)=="") {
                        showdialog("请输入折扣！");
                        return false;
                    }
                    if(jQuery.isNaN(Rebate)) {
                        showdialog("输入折扣格式错误！");
                        return false;
                    }
                } else if(PriceType=="1") {
                    if(jQuery.trim(Prices)=="") {
                        showdialog("请输入价格！");
                        return false;
                    }
                    if(jQuery.isNaN(Prices)) {
                        showdialog("输入价格格式错误！");
                        return false;
                    }
                }
                //整数
                var patern=/^\d+$/;
                //小数
                var reg=/^[0-9]+([.]{1}[0-9]{1,2})?$/;
                if(!reg.test(DownRebate)) {
                    showdialog("输入下级返点格式错误！");
                    return false;
                }
                if(!reg.test(LaterRebate)) {
                    showdialog("输入下级后返格式错误！");
                    return false;
                }
                if(!reg.test(ShareRebate)) {
                    showdialog("输入共享返点格式错误！");
                    return false;
                }
                if(parseFloat(DownRebate,10)<0||parseFloat(DownRebate,10)>=100) {
                    showdialog("输入下级返点数据超出范围[0-100]！");
                    return false;
                }
                if(parseFloat(LaterRebate,10)<0||parseFloat(LaterRebate,10)>=100) {
                    showdialog("输入下级后返数据超出范围[0-100]！");
                    return false;
                }
                if(parseFloat(ShareRebate,10)<0||parseFloat(ShareRebate,10)>=100) {
                    showdialog("输入共享返点数据超出范围[0-100]！");
                    return false;
                }

                if(!reg.test(AirRebate)) {
                    showdialog("输入航空公司返点格式错误！");
                    return false;
                }
                if(!patern.test(AdvanceDays)) {
                    showdialog("输入提前天数格式错误！");
                    return false;
                }
                if(!patern.test(SeatCount)) {
                    showdialog("输入舱位格式错误！");
                    return false;
                }
                if(jQuery.trim(StartDate)==""||jQuery.trim(EndDate)=="") {
                    showdialog("输入政策有效期不能为空！");
                    return false;
                }
                if(jQuery.trim(TicketStartDate)==""||jQuery.trim(TicketEndDate)=="") {
                    showdialog("输入出票日期不能为空！");
                    return false;
                }
                var patOffice=/^[A-Za-z]{3}\d{3}$/;
                if(OfficeCode!=""&&!patOffice.test(OfficeCode)) {
                    showdialog("输入出票Office格式错误!");
                    return false;
                }
                //政策有效日期
                var d1=CompareDate(StartDate+':00',EndDate+':00');
                if(d1) {
                    showdialog('政策开始有效日期不能大于结束有效日期！');
                    return false;
                }
                //政策出票日期
                var d1=CompareDate(TicketStartDate,TicketEndDate);
                if(d1) {
                    showdialog('政策开始有效日期不能大于结束有效日期！');
                    return false;
                }
                var val_Id=jQuery("#Hid_id").val();
                var IsEdit=jQuery("#Hid_IsEdit").val();
                var val_OpType="0";
                if(IsEdit=="1") {
                    //修改
                    val_OpType="1";
                }

                var val_CpyNo=jQuery("#Hid_CpyNo").val();
                var val_CpyName=jQuery("#Hid_CpyName").val();
                var val_LoginName=jQuery("#Hid_LoginName").val();
                var val_UserName=jQuery("#Hid_UserName").val();
                var val_OpFunction="GroupPolicy";

                jQuery("#addAndNext").attr("disabled",true);
                //操作
                jQuery.post("../AJAX/CommonAjAx.ashx",
                {
                    //参数区域
                    CpyNo: escape(val_CpyNo),
                    CpyName: escape(val_CpyName),
                    LoginName: escape(val_LoginName),
                    UserName: escape(val_UserName),
                    AirCode: escape(AirCode),
                    FromCode: escape(fromCode),
                    ToCode: escape(toCode),
                    FlightNo: escape(FlightNo),
                    Class: escape(Seat),
                    FlightTime: escape(FlightTime),
                    PlaneType: escape(PlaneType),
                    PolicyType: escape(PolicyType),
                    FlightStartDate: escape(StartDate),
                    FlightEndDate: escape(EndDate),
                    PrintStartDate: escape(TicketStartDate),
                    PrintEndDate: escape(TicketEndDate),
                    AdvanceDays: escape(AdvanceDays),
                    SeatCount: escape(SeatCount),
                    Prices: escape(Prices),
                    Rebate: escape(Rebate),
                    PriceType: escape(PriceType),
                    OilPrice: escape(RQFare),
                    BuildPrice: escape(AircraftFare),
                    AirRebate: escape(AirRebate),
                    DownRebate: escape(DownRebate),
                    LaterRebate: escape(LaterRebate),
                    ShareRebate: escape(ShareRebate),
                    OfficeCode: escape(OfficeCode),
                    Remarks: escape(Remark),

                    OpFunction: escape(val_OpFunction),
                    OpType: escape(val_OpType),
                    OpPage: escape("AddGroupPolicy.aspx"),
                    Id: escape(val_Id),
                    num: Math.random(),
                    currentuserid: '<%=this.mUser.id.ToString() %>'
                },
                function (data) {
                    jQuery("#addAndNext").attr("disabled",false);
                    //结果处理  
                    var strReArr=data.split('##');
                    if(strReArr.length==3) {
                        //错误代码
                        var errCode=strReArr[0];
                        //错误描述
                        var errDes=strReArr[1];
                        //错误结果
                        var result=jQuery.trim(unescape(strReArr[2]));
                        if(errCode=="1") {//处理成功
                            var html='';
                            if(val_OpType=="1") {
                                html='是否返回列表:<ul class="ulClass"><li><label for="rbYes"><input type="radio" id="rbYes" value="1" name="IsGoList" />是</label></li><li><label for="rbNO"><input type="radio" id="rbNO"  value="2" checked="checked" name="IsGoList" />否</label></li></ul>';
                            }
                            //处理
                            showdialog(errDes+"<br>"+html);
                        } else {//处理失败
                            showdialog(errDes);
                        }
                    }

                },"text");


            } catch(e) {
                alert(e.message);
            }
            return false;
        }          
    //-->    
    </script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
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
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
        }
    </style>
</head>
<body>
    <div id="showmsg">
    </div>
    <form id="form1" runat="server">
    <div class="title">
        <asp:Label ID="lblShow" Text="散冲团政策发布" runat="server" />
    </div>
    <table id="Group_Tab" style="width: 100%; border-collapse: collapse; border-color: #DFF0FD;"
        border="1" cellpadding="0" cellspacing="50">
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>航空公司:
            </th>
            <td>
                <uc1:SelectAirCode ID="ddlAirCode" IsDShowName="false" IsShowAll="true" DefaultOptionValue=""
                    ChangeFunctionName="GetCode" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>出发城市:
            </th>
            <td>
                <uc1:SelectAirCode ID="ddlFromCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--出发城市--"
                    ChangeFunctionName="GetFromCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>到达城市:
            </th>
            <td>
                <uc1:SelectAirCode ID="ddlToCity" IsDShowName="false" IsShowAll="true" DefaultOptionText="--到达城市--"
                    ChangeFunctionName="GetToCode" DefaultOptionValue="" InputMaxLength="3" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>航班号:
            </th>
            <td>
                <asp:TextBox ID="txtFlightNo" runat="server" MaxLength="4"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>舱位:
            </th>
            <td>
                <uc1:SelectAirCode ID="ddlClass" IsDShowName="false" IsShowAll="true" DefaultOptionText="--选择舱位--"
                    ChangeFunctionName="GetClass" DefaultOptionValue="" InputMaxLength="1" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>起飞抵达时间:
            </th>
            <td>
                <uc2:TimeCtrl ID="TimeCtrl1" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>机型:
            </th>
            <td>
                <uc1:SelectAirCode ID="ddlAircraft" IsDShowName="false" IsShowAll="true" DefaultOptionText="--选择机型--"
                    ChangeFunctionName="GetPlaneType" DefaultOptionValue="" IsCompareLength="false"
                    InputMaxLength="4" runat="server" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>机场建设费￥:
            </th>
            <td>
                <asp:TextBox ID="txtAircraftFare" runat="server" Text="0"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                燃油费￥:
            </th>
            <td>
                <asp:TextBox ID="txtRQFare" runat="server" Text="0"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font> 政策类型:
            </th>
            <td>
                <label for="UPolicyType_1">
                    <input type="radio" name="UPolicyType" id="UPolicyType_1" value="1" checked="checked"
                        onclick="setPolicyType(this.value)" />B2B
                </label>
                <label for="UPolicyType_2">
                    <input type="radio" name="UPolicyType" id="UPolicyType_2" value="2" onclick="setPolicyType(this.value)" />BSP
                </label>
                <label for="UPolicyType_3" class="hide">
                    <input type="radio" name="UPolicyType" id="UPolicyType_3" value="3" onclick="setPolicyType(this.value)" />B2B/BSP
                </label>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                优惠方式:
            </th>
            <td>
                <label for="UPriceType_1">
                    <input type="radio" name="UPriceType" id="UPriceType_1" value="0" onclick="setPriceType(this.value)"
                        checked="checked" />按折扣
                </label>
                <label for="UPriceType_2">
                    <input type="radio" name="UPriceType" id="UPriceType_2" value="1" onclick="setPriceType(this.value)" />按价格
                </label>
            </td>
        </tr>
        <tr id="tr_rebate">
            <th class="w_td">
                <font style="color: red;">*</font>折扣:
            </th>
            <td>
                <asp:TextBox ID="txtURebate" runat="server" Text="0" FanWei="0-300"></asp:TextBox>
            </td>
        </tr>
        <tr class="hide" id="tr_price">
            <th class="w_td">
                <font style="color: red;">*</font>价格:
            </th>
            <td>
                <asp:TextBox ID="txtPrices" runat="server" Text="0"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>下级返点:
            </th>
            <td>
                <asp:TextBox ID="txtDownRebate" runat="server" Text="0" FanWei="0-100"></asp:TextBox>%
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>下级后返:
            </th>
            <td>
                <asp:TextBox ID="txtLaterRebate" runat="server" Text="0" FanWei="0-100"></asp:TextBox>%
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>共享返点:
            </th>
            <td>
                <asp:TextBox ID="txtShareRebate" runat="server" Text="0" FanWei="0-100"></asp:TextBox>%
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>航空公司返点:
            </th>
            <td>
                <asp:TextBox ID="txtAirRebate" runat="server" Text="0" FanWei="0-100"></asp:TextBox>%
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>政策适用时间:
            </th>
            <td>
                <input type="text" id="txtStartDate" style="width: 130px;" readonly="true" runat="server"
                    class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d %H:%m',autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" />至
                <input type="text" id="txtEndDate" style="width: 130px;" readonly="true" runat="server"
                    class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d %H:%m',autoPickDate:true,dateFmt:'yyyy-MM-dd HH:mm'})" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>出票日期:
            </th>
            <td>
                <input type="text" id="txtTicketStartDate" style="width: 130px;" readonly="true"
                    class="inputBorder" runat="server" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />至
                <input type="text" id="txtTicketEndDate" style="width: 130px;" readonly="true" runat="server"
                    class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <th class="w_td">
                提前天数:
            </th>
            <td>
                <asp:TextBox ID="txtAdvanceDays" runat="server" Text="0"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>座位数:
            </th>
            <td>
                <asp:TextBox ID="txtSeatCount" runat="server" Text="0"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                <font style="color: red;">*</font>出票Office号:
            </th>
            <td>
                <asp:TextBox ID="txtOfficeCode" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="w_td">
                出票备注:
            </th>
            <td>
                <textarea id="txtUPolicyRemark" cols="60" rows="5" class="inputBorder" maxlength="500"
                    style="width: 600px;" runat="server"></textarea>
            </td>
        </tr>
        <tr>
            <td class="r-td" align="center" colspan="2">
                <span class="btn btn-ok-s">
                    <asp:Button ID="addAndNext" runat="server" Text="添 加" OnClientClick="return AddPolicy();" />
                </span>&nbsp;<span class="btn btn-ok-s"><input type="button" value="返回" onclick="ReGo()" /></span>
            </td>
        </tr>
    </table>
    <%--优惠类型（0=折扣，1=价格）--%>
    <input type="hidden" id="Hid_PriceType" runat="server" value="0" />
    <%--政策类型 1=B2B，2=BSP，3=BSP/B2B--%>
    <input type="hidden" id="Hid_UPolicyType" runat="server" value="1" />
    <%--//是否编辑 1是 0否--%>
    <input type="hidden" id="Hid_IsEdit" runat="server" value="0" />
    <%--公司编号--%>
    <input type="hidden" id="Hid_CpyNo" runat="server" />
    <%--公司名称--%>
    <input type="hidden" id="Hid_CpyName" runat="server" />
    <%--供应商名字--%>
    <input type="hidden" id="Hid_UserName" runat="server" />
    <%--登录账号 --%>
    <input type="hidden" id="Hid_LoginName" runat="server" />
    <%--航空公司二字码 --%>
    <input type="hidden" id="Hid_AirCode" runat="server" />
    <%--出发城市三字码 --%>
    <input type="hidden" id="Hid_fromCode" runat="server" />
    <%--到达城市三字码 --%>
    <input type="hidden" id="Hid_toCode" runat="server" />
    <%--舱位 --%>
    <input type="hidden" id="Hid_Seat" runat="server" />
    <%--机型 --%>
    <input type="hidden" id="Hid_PlaneType" runat="server" />
    <input type="hidden" id="Hid_id" runat="server" />
    <input type="hidden" id="Hid_PageType" runat="server" value="4" />
    <%--编辑--%>
    <input type="hidden" id="Hid_where" runat="server" />
    <input type="hidden" id="Hid_currPage" runat="server" />
    <script language="javascript" type="text/javascript">
        //航空公司选择
        function GetCode(text,val,sel) {
            jQuery("#Hid_AirCode").val(val);
        }
        //出发城市选择
        function GetFromCode(text,val,sel) {
            jQuery("#Hid_fromCode").val(val);
        }
        //到达城市选择
        function GetToCode(text,val,sel) {
            jQuery("#Hid_toCode").val(val);
        }
        //舱位选择
        function GetClass(text,val,sel) {
            jQuery("#Hid_Seat").val(val);
        }
        //机型选择
        function GetPlaneType(text,val,sel) {
            jQuery("#Hid_PlaneType").val(text);
            if(jQuery.trim(val)!="") {
                jQuery("#txtAircraftFare").val(val.split('-')[0]);
            }
        }
        function Edit() {
            var PriceType=jQuery("#Hid_PriceType").val();
            setPriceType(PriceType);

            var PolicyType=jQuery("#Hid_UPolicyType").val();
            setPolicyType(PolicyType);
        }

        jQuery(function () {
            try {
                var IsEdit=jQuery("#Hid_IsEdit").val();
                if(IsEdit=="1") {
                    Edit();
                }
                else {
                    jQuery("#txtAircraftFare").blur(NumVate);
                    jQuery("#txtRQFare").blur(NumVate);

                    jQuery("#txtDownRebate").blur(NumVate);
                    jQuery("#txtLaterRebate").blur(NumVate);
                    jQuery("#txtShareRebate").blur(NumVate);
                    jQuery("#txtAirRebate").blur(NumVate);

                    jQuery("#txtAdvanceDays").blur(NumVate);
                    jQuery("#txtSeatCount").blur(NumVate);

                    jQuery("#txtURebate").blur(NumVate);

                    var val0=jQuery("#txtAdvanceDays").val();
                    var val1=jQuery("#txtSeatCount").val();
                    var pattern=/^\d+$/;
                    if(!pattern.test(val0)) {
                        showdialog("提前天数输入格式错误!");
                        return false;
                    }
                    if(!pattern.test(val1)) {
                        showdialog("舱位个数输入格式错误!");
                        return false;
                    }
                    jQuery("#txtOfficeCode").blur(function () {
                        var val=jQuery.trim(jQuery(this).val());
                        var patOffice=/^[A-Za-z]{3}\d{3}$/;
                        if(val!=""&&!patOffice.test(val)) {
                            showdialog("输入出票Office格式错误!");
                            return false;
                        }
                    });
                }
            } catch(e) {
                alert(e.message);
            }
        });
    </script>
    </form>
</body>
</html>

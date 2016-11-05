<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FlyerList.aspx.cs" Inherits="Air_Buy_FlyerList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 

"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>乘客查询</title>
    <link href="../css/List.css" rel="stylesheet" type="text/css" />
    <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../Css/JPstep.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        #PassengerDiv
        {
            border: 1px solid #FFC674;
            padding: 15px;
            margin-bottom: 10px;
        }
        #PassengerDiv .PrototypeDiv
        {
            border: 1px solid #FFC674;
            width: 99%;
            margin: 5px 0;
        }
        .Passengertab
        {
        }
        .Passengertab td
        {
            height: 30px;
            text-align: center;
        }
        #PriceDiv
        {
            width: 100%;
            height: 25px;
            border: 1px solid #FF9966;
            background-color: #FFFFCC;
            text-align: center;
            line-height: 25px;
            vertical-align: middle;
        }
        .DivDistribution
        {
            border: 1px solid #FFC674;
            padding: 15px;
            margin-bottom: 10px;
            background-color: White;
        }
        #tabDistribution
        {
            width: 99%;
            display: none;
            margin-top: 5px;
            line-height: 30px;
        }
        .normal-content h5
        {
            font-size: 14px;
            color: #606060;
            line-height: 30px;
            text-align: left;
        }
        .normal-content h5 b
        {
            color: #909090;
            font-size: 12px;
        }
        .flighttab tr
        {
            height: 30px;
            line-height: 30px;
        }
        .options
        {
            height: 20px;
            width: 100%;
            overflow: hidden;
        }
        .options li
        {
            float: left;
            float: left;
            margin-right: 10px;
        }
        .ui-widget-header
        {
            height: 30px;
            line-height: 30px;
        }
        #btnAdd, #btnFind
        {
            border: 1px solid #FF9D4E;
            background: #FF9D4E url(../CSS/smoothness/images/ui-bg_glass_75_3b97d6_1x400.png) 50% 50% repeat-x;
            font-weight: normal;
            color: white;
            -moz-border-radius: 4px;
            -webkit-border-radius: 4px;
            border-radius: 4px;
            padding: 2px 6px;
            text-decoration: none;
            position: relative;
        }
        .table_info_open
        {
            background-color: White;
            border-top: 1px dashed #E6E6E6;
        }
        .table_info_open th
        {
            width: 80px;
            text-align: right;
        }
        .table_info_open td
        {
            text-align: left;
            padding-left: 10px;
        }
        .MoreConditionA
        {
            background: url("../img/ArrowDown2.gif") no-repeat scroll right center transparent;
            float: left;
            height: 30px;
            line-height: 30px;
            padding: 0 15px 0 7px;
        }
        .MoreCondition
        {
            background: url("../img/ArrowUp2.gif") no-repeat scroll right center transparent;
            float: left;
            height: 30px;
            line-height: 30px;
            padding: 0 15px 0 7px;
        }
        .btn-ok-s
        {
            padding: 1px;
            margin-right: 3px;
            border: 1px solid #D74C00;
        }
        .btn-ok-s input
        {
            background: url("../img/bg-btn.png") repeat-x scroll 0px -75px;
            cursor: pointer;
            font-family: Tahoma;
            outline: none;
            color: White;
            padding: 0 5px;
            border: none;
        }
        .gvhead
        {
            background-color: #FF9D4E;
            color: White;
            height: 30px;
            line-height: 30px;
        }
        .tb-all-trade td
        {
            height: 25px;
            font-family: 微软雅黑,tahoma,arial,sans-serif;
        }
    </style>
    <script type="text/javascript">
        function showdialog(t) {
            jQuery("#showDiv").html(t);
            jQuery("#showDiv").dialog({
                title: '提示',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        jQuery(this).dialog('close');
                    }
                }
            });
        }
        //全选
        function SelAll(obj) {
            jQuery("input[type='checkbox'][name='ck_name']").attr("checked",obj.checked);
        }
        function dataLidate() {
            var txtName=jQuery.trim(jQuery("#txtName").val());
            var txtNo=jQuery.trim(jQuery("#txtNo").val());
            var txtPhone=jQuery.trim(jQuery("#txtPhone").val());
            if(txtName==""&&txtNo==""&&txtPhone=="") {
                showdialog("请输入查询条件！");
                return false;
            }
            return true;
        }

        function AddPas(obj) {
            var IsSuc=false;
            var strVal=unescape(jQuery("#Info").val());
            var PasFg=jQuery.trim(jQuery("#Hid_FgNum").val());
            var BigNum=jQuery.trim(jQuery("#Hid_BigNum").val());
            BigNum=parseInt((BigNum=="")?"0":BigNum);
            var pList=window.parent.GetSelModel();

            var selNum=pList.PNum;//已有人数
            var List=pList.List;//乘客列表
            var PasType=pList.PasType;//乘客类型

            var listModel=null;
            var model=null;
            if(strVal!="") {
                listModel=eval("("+strVal+")");
            }
            var id=jQuery(obj).attr("value");
            for(var i=0;i<listModel.length;i++) {
                if(listModel[i]._id==id) {
                    model=listModel[i];
                    break;
                }
            }

            IsSuc=window.parent.AddOne(model,PasFg);
            /*
            if(selNum>BigNum) {
            showdialog("可用座位数（"+BigNum+"）不足！");
            }
            else if(selNum>9) {
            showdialog("乘客最多可以选择9人！");
            } else {
            if(jQuery.inArray(model._name,List)!= -1) {
            showdialog("乘客"+model._name+"已添加在列表中！");
            } else {
            PasType.push(model._flyertype);
            //成人 儿童 婴儿个数
            var adultNum=0,childNum=0,YNum=0;
            for(var i=0;i<PasType.length;i++) {
            if(PasType[i]=="1") {
            adultNum++;
            } else if(PasType[i]=="2") {
            childNum++;
            } else if(PasType[i]=="3") {
            YNum++;
            }
            }
            if(adultNum>0) {
            //成人 婴儿个数比较
            if(adultNum<YNum) {
            showdialog("一个成人只能带一个婴儿，请重新选择！");
            } else {
            if((adultNum*2)<childNum) {
            showdialog("一个成人只能带两个儿童，请重新选择！");
            } else {

            IsSuc=true;
            }
            }
            } else {
            if(YNum>0) {
            showdialog("婴儿必须有一个成人陪伴！");
            } else {
            IsSuc=true;
            }
            }
            }
            if(IsSuc) {
            IsSuc=window.parent.AddOne(model,PasFg);
            }
            }
            */
            return IsSuc;
        }


        function SaveData() {
            try {

                var Account=jQuery("#Hid_LoginAccount").val();
                var LoginID=jQuery("#Hid_LoginID").val();
                var BigNum=jQuery.trim(jQuery("#Hid_BigNum").val());
                BigNum=parseInt((BigNum=="")?"0":BigNum);
                var cks=jQuery("input[type='checkbox'][name='ck_name']:checked");
                var strVal=unescape(jQuery("#Info").val());
                var listModel=null;
                if(strVal!="") {
                    listModel=eval("("+strVal+")");
                }
                if(cks.length==0) {
                    showdialog("请选择常旅客！");
                } else {
                    if(cks.length>BigNum) {
                        showdialog("可用座位数（"+BigNum+"）不足！");
                    }
                    else if(cks.length>9) {
                        showdialog("乘客最多可以选择9人！");
                    }
                    else {
                        if(listModel!=null) {
                            //成人 儿童 婴儿个数
                            var adultNum=0,childNum=0,YNum=0;
                            var strArr=[];//储存每一个实体
                            for(var i=0;i<cks.length;i++) {
                                for(var j=0;i<listModel.length;j++) {
                                    var model=listModel[j];
                                    if(cks[i].value==model._id) {
                                        if(model._flyertype=="1") {
                                            adultNum++;
                                        } else if(model._flyertype=="2") {
                                            childNum++;
                                        } else if(model._flyertype=="3") {
                                            YNum++;
                                        }
                                        strArr.push(model);
                                        break;
                                    }
                                }
                            }
                            if(strArr.length>0) {
                                if(adultNum>0) {
                                    //成人 婴儿个数比较
                                    if(adultNum<YNum) {
                                        showdialog("一个成人只能带一个婴儿，请重新选择！");
                                    } else {
                                        if((adultNum*2)<childNum) {
                                            showdialog("一个成人只能带两个儿童，请重新选择！");
                                        } else {

                                            //添加常旅客
                                            window.parent.addPerson(strArr);
                                            //关闭对话框
                                            window.parent.CloseDg();
                                        }
                                    }
                                } else {
                                    if(YNum>0) {
                                        showdialog("婴儿必须有一个成人陪伴！");
                                    } else {
                                        //添加常旅客
                                        window.parent.addPerson(strArr);
                                        //关闭对话框
                                        window.parent.CloseDg();
                                    }
                                }
                            } else {
                                showdialog("请选择常旅客！");
                            }
                        }
                    }
                }
            } catch(e) {
                showdialog("数据加载错误！");
            }
            return false;
        }
        
    </script>
    <%--<script type="text/javascript" src="../js/js_Pager.js"></script>--%>
</head>
<body style="margin: 0; padding: 0;">
    <form id="form1" runat="server">
    <div style="text-align: center;">
        <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0" width="100%" >            
            <tr>
                <td style="height: 40px;">
                    乘客姓名:<input type="text" style="width: 110px" id="txtName" runat="server" />
                    &nbsp;&nbsp;&nbsp; 证件号:<input type="text" style="width: 130px" id="txtNo" runat="server" />
                    &nbsp;&nbsp;&nbsp; 手机号:<input type="text" style="width: 90px" id="txtPhone" size="11"
                        maxlength="11" runat="server" />
                    <span class="btn-ok-s">
                        <asp:Button ID="btnSel" runat="server" Text=" 查 询 " OnClick="btnSel_Click" OnClientClick="return dataLidate(); ">
                        </asp:Button></span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Repeater ID="gvflyer" runat="server">
                        <HeaderTemplate>
                            <table cellspacing="0" rules="rows" id="gvflyer" style="border-color: #CCCCCC; border-width: 1px;
                                border-style: solid; width: 100%; border-collapse: collapse;">
                                <thead>
                                    <tr class="gvhead" id="trHead">
                                        <th scope="col">
                                            <%-- <input type="checkbox" id="ckall" onclick="SelAll(this)" />--%>
                                            选择
                                        </th>
                                        <th scope="col">
                                            序号
                                        </th>
                                        <th scope="col">
                                            乘客姓名
                                        </th>
                                        <th scope="col">
                                            乘客类型
                                        </th>
                                        <th scope="col">
                                            证件类型
                                        </th>
                                        <th scope="col">
                                            证件号码
                                        </th>
                                        <th scope="col">
                                            手机号码
                                        </th>
                                    </tr>
                                </thead>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tbody>
                                <tr>
                                    <td>
                                        <label for='ck_<%# Eval("Id") %>'>
                                            <%-- <input type="radio" id='ck_<%# Eval("Id") %>' name="ck_name" value='<%# Eval("Id") %>'
                                                onclick="AddPas(this)" />选择</label>--%>
                                            <a href="#" id='ck_<%# Eval("Id") %>' style="color: Blue;" name="ck_name" value='<%# Eval("Id") %>'
                                                onclick="AddPas(this)">选择</a>
                                    </td>
                                    <td>
                                        <%# Container.ItemIndex+1 %>
                                    </td>
                                    <td>
                                        <%# Eval("Name")%>
                                    </td>
                                    <td>
                                        <%# ShowPasType(Eval("Flyertype").ToString())%>
                                    </td>
                                    <td>
                                        <%# ShowCardType(Eval("CertificateType").ToString())%>
                                    </td>
                                    <td>
                                        <%# Eval("CertificateNum")%>
                                    </td>
                                    <td>
                                        <%# Eval("Tel")%>
                                    </td>
                                </tr>
                            </tbody>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div id="pagerDiv">
                    </div>                   
                        <input type="hidden" id="hid_PageSize" value="9" runat="server" />
                        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="9" PagingButtonSpacing="3px"
                            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
                        </webdiyer:AspNetPager>
                    
                </td>
            </tr>
        </table>
    </div>
    <div id="show" style="width: 100%; text-align: center; font-size: 12px; line-height: 20px;
        color: Gray; margin-top: 0;" runat="server" visible="false">
        没有符合你要搜索的条件,请输入正确的查询条件！
    </div>
    <div id="showDiv">
    </div>
    <div style="text-align: center; margin: 6px 0;" class="hide">
        <span id="btnQueRen" class="btn-ok-s">
            <asp:Button ID="btnOk" runat="server" Text=" 确 认 " OnClientClick="return SaveData();">
            </asp:Button></span></div>
    <%--隐藏域--%>
    <input id="Hid_LoginAccount" type="hidden" runat="server" />
    <input id="Hid_LoginID" type="hidden" runat="server" />
    <input id="Hid_BigNum" type="hidden" runat="server" />
    <input id="Hid_FgNum" type="hidden" runat="server" />
    <%--当前页所有乘客--%>
    <input id="Info" runat="server" type="hidden" />
    </form>
</body>
</html>

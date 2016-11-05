<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Create.aspx.cs" Inherits="Buy_Create" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>查询预订</title>
    <link href="../css/List.css" rel="stylesheet" type="text/css" />
    <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/json2.js"></script>
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
        .red
        {
            color: Red;
        }
        .green
        {
            color: green;
        }
        .white
        {
            color: White;
        }
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
            float: left;
        }
        .flighttab tr
        {
            height: 30px;
            line-height: 30px;
        }
        .TextBorder
        {
            border: 1px solid #999;
        }
        /*自动提示样式*/
        .pTop
        {
            background-color: #5285B9;
            color: White;
        }
        .pButtom
        {
            background-color: Gray;
        }
        .suggec
        {
            background-color: #A9E7BA;
        }
        #suggestions
        {
            border: solid 1px #7F9DB9;
        }
        #suggestions, #suggestions ul
        {
            margin: 0;
            padding: 0;
        }
        #suggestions ul li
        {
            margin: 0;
            padding: 0;
            width: 100%;
            height: 20px;
            line-height: 20px;
            text-indent: 10px;
            cursor: pointer;
            list-style: none;
            border-bottom: solid 1px gray;
        }
    </style>
    <script type="text/javascript" src="../js/js_CompareDate.js" language="javascript"></script>
    <asp:Literal runat="server" ID="Script"></asp:Literal>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <div id="divDG">
    </div>
    <div id="IFrame">
    </div>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div class="main-normal">
        <div class="listing">
            <div class="titleheight">
                <h3>
                    填写机票预订单<font></font></h3>
                <ul>
                    <li><em class="current"></em>预订</li>
                    <li><em class="current"></em>填写订单</li>
                    <li><em class="bg"></em>确认</li>
                    <li><em class="bg"></em>支付</li>
                    <li><em></em>完成</li>
                </ul>
            </div>
            <div class="normal-content mainnav">
                <div id="div_ViewState">
                    <div class="DivDistribution">
                        <table class="flighttab" style="width: 100%; text-align: center;">
                            <tr class="info-head">
                                <td style="width: 20%">
                                    航程信息
                                </td>
                                <td style="width: 10%">
                                    起飞日期
                                </td>
                                <td style="width: 10%">
                                    起抵时间
                                </td>
                                <td style="width: 8%">
                                    承运人
                                </td>
                                <td style="width: 7%">
                                    航班号
                                </td>
                                <td style="width: 7%">
                                    机型
                                </td>
                                <td style="width: 7%">
                                    舱位
                                </td>
                                <td style="width: 7%">
                                    舱位价
                                </td>
                                <td style="width: 7%">
                                    机建费
                                </td>
                                <td style="width: 7%">
                                    燃油费
                                </td>
                                <td style="width: 10%">
                                    票面总价
                                </td>
                            </tr>
                            <asp:Literal runat="server" ID="TrShow"></asp:Literal>
                        </table>
                    </div>
                    <div id="PassengerDiv" class="DivDistribution">
                        <div>
                            <h5>
                                乘机人信息 <b>（请准确填写登机人信息，以免在购票过程中发生问题，非中文名请在姓和名之间加上 “/”）</b></h5>
                            <%--<span id="span_Flyer" runat="server">
                                <input id="btnFind" type="button" value="选择常旅客" onclick="SelectPassenger()" class="btn big1 cp" /></span>--%>
                            乘机人数:
                            <select id="passengers" name="passengers" onchange="setShowGroup(this.value)">
                                <option value="1">1</option>
                                <option value="2">2</option>
                                <option value="3">3</option>
                                <option value="4">4</option>
                                <option value="5">5</option>
                                <option value="6">6</option>
                                <option value="7">7</option>
                                <option value="8">8</option>
                                <option value="9">9</option>
                            </select>
                            &nbsp;&nbsp;<span style="color: Red;"><b>注：如果名字中包含生僻字,请把生僻字及之后汉字使用拼音代替</b></span>
                        </div>
                        <div id="divNewDiv" class="PrototypeDiv">
                            <table class="Passengertab" rules="rows" style="border-color: #CCCCCC; border-width: 0px;
                                border-style: solid; width: 100%; border-collapse: collapse;" id="tab_Pas">
                                <thead>
                                    <tr style="line-height: 30px;" class="info-head">
                                        <th style="width: 4%; text-align: center;" scope="col">
                                            序号
                                        </th>
                                        <th style="width: 13%; text-align: center;" scope="col">
                                            乘机人
                                        </th>
                                        <th style="width: 9%; text-align: center;" scope="col">
                                        </th>
                                        <th style="width: 7%; text-align: center;">
                                            乘客类型
                                        </th>
                                        <th style="width: 10%; text-align: center;" scope="col">
                                            证件类型
                                        </th>
                                        <th style="width: 20%; text-align: center;" scope="col">
                                            证件号
                                        </th>
                                        <th style="width: 9%; text-align: center;" scope="col">
                                        </th>
                                        <th style="width: 10%; text-align: center;" scope="col">
                                            乘客手机
                                        </th>
                                        <th style="width: 9%; text-align: center;" scope="col">
                                        </th>
                                        <th>
                                            航空公司卡号
                                        </th>
                                        <th style="width: 8%; text-align: center;" scope="col">
                                            常旅客
                                        </th>
                                        <th style="width: 9%; text-align: center;" scope="col">
                                            操作
                                        </th>
                                    </tr>
                                    <tr id="tr_Pas_0" class="hide">
                                        <td>
                                            <span id="xuhao_0">1</span>
                                        </td>
                                        <td>
                                            <input id="txtPasName_0" type="text" size="15" />
                                        </td>
                                        <td style="width: 9%; text-align: left;">
                                            <span id="span_PasName_0"><font class="red">*</font></span>
                                        </td>
                                        <td>
                                            <select id="SelPasType_0" style="width: 100px;" runat="server">
                                            </select>
                                        </td>
                                        <td>
                                            <select id="SelCardType_0" style="width: 100px;" runat="server">
                                            </select>
                                        </td>
                                        <td>
                                            <span id="msgCardNum_0">
                                                <input id="txtCardNum_0" type="text" maxlength="18" /></span> <span id="msgBirthday_0"
                                                    class="hide">
                                                    <input type="text" id="txtBirthday_0" style="width: 130px;" readonly="readonly" class="inputBorder"
                                                        onfocus="WdatePicker({isShowClear:false,isShowWeek:false,maxDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                                                </span><span id="chddate_0" class="hide">
                                                    <br />
                                                    <b>出生日期:</b>
                                                    <input type="text" id="txtchlddate_0" style="width: 93px;" readonly="readonly" class="inputBorder"
                                                        onfocus="WdatePicker({isShowClear:false,isShowWeek:false,maxDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                                                </span>
                                        </td>
                                        <td style="width: 9%; text-align: left;">
                                            <span id="span_CardNum_0"><font class="red">*</font></span>
                                        </td>
                                        <td>
                                            <input id="txtMobile_0" type="text" size="14" maxlength="11" />
                                        </td>
                                        <td style="width: 9%; text-align: left;">
                                            <span id="span_Mobile_0"><font class="red"></font></span>
                                        </td>
                                        <td>
                                            <input id="txtcpyandno_0" type="text" size="18" maxlength="20" />
                                        </td>
                                        <td style="width: 8%; text-align: center;">
                                            <input id="ck_Isflyer_0" type="checkbox" />
                                            <input id="flyerremark_0" type="hidden" />
                                        </td>
                                        <td style="width: 13%; text-align: center;">
                                            <div id="op_div_0">
                                                <%-- <a href="#" onclick="return addGroup(event); ">添加</a>--%>
                                                <span id="span_Flyer" runat="server">
                                                    <input id="btnFind" type="button" value="选择常旅客" onclick="SelectPassenger(0)" class="btn big1 cp" /></span>
                                            </div>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <%--功能区域--%>
                    <div id="divList">
                        <h5>
                            &nbsp;&nbsp;&nbsp;&nbsp;温馨提示 <b>(换编码或能匹配更好政策）&nbsp; </b>
                        </h5>
                        <ul class="ulClass" style="margin-left: 50px;">
                            <%--是否儿童出成人票--%>
                            <span id="CHDToAdult" runat="server">
                                <label for="ckIsETDZAudltTK">
                                    <input type="checkbox" id="ckIsETDZAudltTK" runat="server" /><b class="red">允许儿童出成人票</b></label></span>
                            <span id="_IsChangPnr" runat="server">
                                <asp:CheckBox runat="server" ID="chkChangePnr" Text="允许换编码" Font-Bold="true" ForeColor="Red"
                                    Checked="false" /></span> <span id="_IsRobTicketOrder" runat="server">
                                        <asp:CheckBox runat="server" ID="ckIsRobTicketOrder" Text="是否抢票" Font-Bold="true"
                                            ForeColor="Red" /></span>
                        </ul>
                        <div style="clear: both;">
                        </div>
                    </div>
                    <div id="AdultDiv" class="DivDistribution hide">
                        <h5>
                            关联信息<b>（订单编号中必须含有成人信息） </b>
                        </h5>
                        <table>
                            <tr>
                                <td>
                                    关联成人订单号：
                                </td>
                                <td>
                                    <asp:TextBox ID="txtAdultOrder" runat="server" MaxLength="20"></asp:TextBox>
                                    <span id="span1" style="color: Red;"><b>*</b></span>
                                </td>
                                <td class="hide">
                                    关联成人编码：
                                </td>
                                <td class="hide">
                                    <asp:TextBox ID="txtPnr" runat="server" MaxLength="6"></asp:TextBox>
                                    <span id="span2" style="color: Red;"><b>*</b></span>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="DivDistribution">
                        <h5>
                            联系人信息 <b>（请准确填写联系人信息，重要信息我们会通过手机短信等方式通知您）&nbsp; </b>
                        </h5>
                        <table>
                            <tr>
                                <td style="width: 6em; text-align: right;">
                                    姓名：
                                </td>
                                <td style="text-align: left;">
                                    <asp:TextBox ID="txtLinkName" CssClass="inputtxtdat" runat="server" MaxLength="30"
                                        Width="110px"></asp:TextBox>
                                    <span id="spanName" style="color: Red;"><b>*</b></span>
                                </td>
                                <td style="width: 6em; text-align: right;">
                                    手机号码：
                                </td>
                                <td style="text-align: left;">
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="inputtxtdat" MaxLength="11"
                                        Width="110px"></asp:TextBox>
                                    <span id="spanMobile" style="color: Red;"><b>*</b></span>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <table width="100%">
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnSub" runat="server" Text="生成订单" OnClientClick="return OK(this,event);"
                                    OnClick="btnSub_Click" CssClass="btn big1 cp"></asp:Button>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="overlay" class="hide">
    </div>
    <div id="loading" class="hide">
        请稍等，<br />
        PNR正在生成中……<br />
        <img src="../img/loading.gif"></div>
    <%--隐藏域--%>
    <%--有效座位数--%>
    <input id="Hid_BigNum" type="hidden" runat="server" value="0" />
    <%--生僻字--%>
    <input id="Hid_Rare" type="hidden" runat="server" />
    <%--承运人--%>
    <input id="Hid_Carriy" type="hidden" runat="server" />
    <%--证件号类型--%>
    <input id="Hid_CardType" type="hidden" runat="server" />
    <input id="Hid_LoginAccount" type="hidden" runat="server" />
    <input id="Hid_LoginID" type="hidden" runat="server" />
    <input id="Hid_RoleType" type="hidden" runat="server" />
    <%--乘客数据--%>
    <input id="Hid_PasData" type="hidden" runat="server" />
    <%--是否需要关联成人订单号 0否 1是--%>
    <input id="Hid_IsAsAdultOrder" type="hidden" runat="server" value="0" />
    <%--是否开启儿童必须关联成人订单号 1开启 0关闭--%>
    <input id="Hid_CHDOPENAsAdultOrder" type="hidden" runat="server" value="0" />
    <%--保存视图状态--%>
    <input id="Hid_ViewState" type="hidden" runat="server" />
    <input id="Hid_Global" type="hidden" runat="server" />
    <input id="Hid_IP" type="hidden" runat="server" />
    <input id="Hid_Port" type="hidden" runat="server" />
    <input id="Hid_Office" type="hidden" runat="server" />
    <input id="Hid_Space" type="hidden" runat="server" />
    <input id="Hid_CurrTime" type="hidden" runat="server" />
    <%--常旅客数据--%>
    <input id="Hid_FlyerList" type="hidden" runat="server" />
    </form>
</body>
</html>

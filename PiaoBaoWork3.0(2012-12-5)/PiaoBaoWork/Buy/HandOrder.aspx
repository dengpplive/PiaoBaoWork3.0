<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HandOrder.aspx.cs" Inherits="Buy_HandOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>手工订单</title>
    <script type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <link type="text/css" href="../CSS/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../CSS/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../JS/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../JS/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        #loading
        {
            margin-top: 10px;
            width: 420px;
            border: 0 none;
            text-align: center;
            padding: 40px 30px 40px 30px;
            color: #707070;
            font-size: 18px;
            line-height: 180%;
            position: fixed;
            left: 30%;
            top: 30%;
            z-index: 1000;
            background: url(../images/mainbg.gif);
        }
        #overlay
        {
            background-color: #333333;
            left: 0;
            filter: alpha(opacity=50); /* IE */
            -moz-opacity: 0.5; /* 老版Mozilla */
            -khtml-opacity: 0.5; /* 老版Safari */
            opacity: 0.5;
            position: fixed;
            top: 0;
            width: 100%;
            z-index: 999;
            height: 100%;
        }
        
        .bgcSty
        {
            background: url("../img/today01a688.gif") repeat-x scroll 0 -162px #000000;
            border-color: #7C7C7C #C3C3C3 #C3C3C3 #9A9A9A;
            border-radius: 3px 3px 3px 3px;
            border-style: solid;
            border-width: 1px;
            font-size: 12px;
            line-height: 22px;
            margin-left: 5px;
            outline: medium none;
            padding: 2px 3px;
            vertical-align: middle;
            color: #00FF00;
            overflow: auto;
        }
        .table_info td
        {
            white-space: nowrap;
        }
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
        .tab_hj td
        {
            padding: 5px;
        }
    </style>
    <script language="javascript" type="text/javascript" src="../js/js_PnrImport.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="tabs">
        <div>
            <div class="title">
                <span>手工订单</span>
            </div>
            <table class="tab_hj">
                <tr>
                    <td align="left" colspan="3">
                        <table style="width: 100%;">
                            <tr id="tr_gy" class="hide">
                                <th style="width: 150px;" align="right">
                                    落地运营商：
                                </th>
                                <td>
                                    <input name="txtCompany_0" type="text" id="txtCompany_0" autocomplete="off" onpropertychange="txtSetSel(this,'selGY',0)"
                                        style="width: 160px;" />
                                    <select id="selGY_0" style="width: 300px;" onchange="ddlSetText(this,'txtCompany',0)"
                                        runat="server">
                                        <option value="">---请选择落地运营商---</option>
                                    </select>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr id="tr_kh">
                                <th align="right" style="width: 150px;">
                                    客户账号/名称：
                                </th>
                                <td>
                                    <input name="txtkehu_0" type="text" id="txtkehu_0" autocomplete="off" onpropertychange="txtSetSel(this,'selKH',0)"
                                        style="width: 160px;" />
                                    <select id="selKH_0" name="selKH_0" onchange="ddlSetText(this,'txtkehu',0)" style="width: 300px;"
                                        runat="server">
                                        <option>---请选择客户名称---</option>
                                    </select>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <th align="right">
                                    PNR编码：
                                </th>
                                <td>
                                    <input id="txtH_PNR" type="text" maxlength="6" runat="server" />&nbsp;&nbsp<span
                                        class="btn btn-ok-s">
                                        <asp:Button ID="btnH_PNRImport" runat="server" Text="PNR导入" OnClick="btnH_PNRImport_Click"
                                            OnClientClick="return returnFG({source:1});" /></span><label for="ckH_Big"><input
                                                id="ckH_Big" type="checkbox" value="0" />大编码</label>
                                </td>
                                <td align="left">
                                </td>
                            </tr>
                            <tr>
                                <th align="right">
                                    <span id="span_Insys">PNR入库</span>：
                                </th>
                                <td>
                                    <input id="txtH_PNR3" type="text" maxlength="6" runat="server" />&nbsp;&nbsp<span
                                        class="btn btn-ok-s">
                                        <asp:Button ID="btnH_PNRImport1" runat="server" Text="PNR入库"
                                            OnClientClick="return returnFG({source:3});" 
                                        onclick="btnH_PNRImport1_Click" /></span><b class="red">入库记账，无需支付</b>
                                </td>
                                <td align="left">
                                </td>
                            </tr>
                            <tr id="tr0_IsMerge0" runat="server">
                                <th align="right">
                                    PNR内容：
                                </th>
                                <td colspan="2" align="left">
                                    <textarea cols="100" rows="8" id="pnrCon" runat="server" class="bgcSty"></textarea>
                                </td>
                            </tr>
                            <tr id="tr0_IsMerge1" runat="server">
                                <th align="right">
                                    PAT内容：
                                </th>
                                <td colspan="2" align="left">
                                    <textarea cols="100" rows="8" id="patCon" runat="server" class="bgcSty"></textarea>
                                </td>
                            </tr>
                            <tr id="tr1_IsMerge" runat="server">
                                <td align="left" colspan="3">
                                    <table border="0" style="width: 100%;">
                                        <tr>
                                            <th align="left">
                                                &nbsp;PNR和PATA内容：
                                            </th>
                                            <th align="left">
                                                &nbsp;以下是编码内容示例：
                                            </th>
                                        </tr>
                                        <tr>
                                            <td class="td">
                                                <asp:TextBox ID="txtPNRAndPata" runat="server" CssClass="bgcSty" TextMode="MultiLine"
                                                    Height="300px" Width="600px"></asp:TextBox>
                                            </td>
                                            <td style="text-align: left; padding-left: 5px;">
                                                <asp:TextBox ID="txtPNRAndPata1" runat="server" CssClass="bgcSty" ReadOnly="true"
                                                    TextMode="MultiLine" Height="300px" Width="600px" onselectstart="return false"
                                                    oncopy="return false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnH_PNRConImport" runat="server" Text="PNR内容导入" OnClick="btnH_PNRConImport_Click"
                                OnClientClick="return returnFG({source:2});" /></span>
                    </td>
                    <td>
                    </td>
                    <td align="left">
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="overlay" runat="server" style="display: none;">
    </div>
    <div id="loading" style="display: none;">
        请稍等，PNR数据正在处理中<br />
        <span id="spastr">……</span><br />
        <img src="../img/loading.gif"></div>
    <div id="show">
    </div>
    <%--pnr导入通道--%>
    <input id="Hid_ImportTongDao" type="hidden" runat="server" value="0" />
    <%--pnr内容是否合并一起 0否 1是--%>
    <input id="Hid_PnrConIsAll" type="hidden" runat="server" value="1" />
    <%--指定的Office导入 默认空--%>
    <input id="Hid_Office" type="hidden" runat="server" value="" />
    <%--角色类型 1=平台，2=落地运营商，3=供应商，4=分销商，5=采购商--%>
    <input id="Hid_UserRoleType" type="hidden" runat="server" value="" />
    <%--两个下拉列表 选择供应或者客户值--%>
    <input id="Hid_GY" type="hidden" runat="server" value="" />
    <input id="Hid_KH" type="hidden" runat="server" value="" />
    <%--是否导入大编码1是 0否--%>
    <input id="Hid_IsBigCode" type="hidden" runat="server" value="0" />
    <%--儿童编码关联成人订单号 后台--%>
    <input id="Hid_OrderId" type="hidden" runat="server" value="" />
    <%--是否开启儿童必须关联成人订单号 1开启 0关闭--%>
    <input id="Hid_CHDOPENAsAdultOrder" type="hidden" runat="server" value="0" />

    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    </form>
</body>
</html>

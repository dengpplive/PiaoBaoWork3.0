<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PnrImport.aspx.cs" Inherits="Buy_PnrImport" %>

<%@ Register Src="../UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pnr导入</title>
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
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <div id="tabs-1">
            <div>
                <table class="table_info" border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td colspan="2" align="right">
                            <div class="title">
                                <span>PNR导入</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                       
                           
                        
                        <td align="left" colspan="2" style="  font-weight:bold; padding:0 0 0 5px">
                         PNR：
                            <span id="spanContainer">
                                <asp:TextBox ID="txtPNR" CssClass="txt" Width="60px" runat="server" MaxLength="6"></asp:TextBox>
                                <span id="Span_CHD" class="hide">
                                    <input type="text" id="txtPernsOrder" class="txt" runat="server" maxlength="20" value="成人订单号" />
                                    &nbsp;<span style="color: Red;"></span></span> <span class="btn btn-ok-s">
                                        <asp:Button ID="btnPnrImport" runat="server" Text=" 导 入 " OnClientClick="return BuyImport();" /></span>
                                <asp:CheckBox runat="server" ID="chkChangePnr" Text="允许换编码" />&nbsp;<font style="font-size:12px;" color="red">温馨提示: (换编码或能匹配更好政策） </font>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        
                        <td align="left" colspan="2" style=" padding:0 0 0 40px">
                            <span id="spanCon">
                                <label for="rd1">
                                    <input id="rd1" name="ImportType" type="radio" value="0" checked="checked" onclick="SelectImportType()" />普通编码</label>
                                <label for="rd2">
                                    <input id="rd2" name="ImportType" type="radio" value="2" onclick="SelectImportType()" />儿童编码</label>
                                <label for="rd3">
                                    <input id="rd3" name="ImportType" type="radio" value="1" onclick="SelectImportType()" />团队编码</label>
                                <label for="rd4">
                                    <input id="rd4" name="ImportType" type="radio" value="3" onclick="SelectImportType()" />航空公司大编码</label>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <div class="title">
                                <span>PNR内容导入</span><span style="display: none; color: Red; font-size: 12px; font-weight: normal;">用PNR内容导入可能享受特价政策</span>
                            </div>
                        </td>
                    </tr>
                    <tr id="trPNRAndPata" runat="server">
                        <td colspan="2">
                            <table border="0">
                                <tr>
                                    <th align="left">
                                        &nbsp;PNR内容和PATA内容一起导入：
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
                                    <td colspan="7" style="text-align: left; padding-left: 5px;">
                                        <asp:TextBox ID="txtPNRAndPata1" runat="server" CssClass="bgcSty" ReadOnly="true"
                                            TextMode="MultiLine" Height="300px" Width="600px" onselectstart="return false"
                                            oncopy="return false"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="trPNRInfo" runat="server">
                        <th class="td">
                            PNR内容：
                        </th>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtPNRInfo" runat="server" CssClass="bgcSty" TextMode="MultiLine"
                                Height="150px" Width="700px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="trPATAInfo" runat="server">
                        <th class="td">
                            PATA内容：
                        </th>
                        <td  style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtPATAInfo" runat="server" CssClass="bgcSty" TextMode="MultiLine"
                                Height="100px" Width="700px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="left" style="padding-left: 10px;">
                            <span class="btn btn-ok-s">
                                <asp:Button ID="btnPnrInfo" runat="server" Text="PNR内容导入" OnClientClick="return StartImport('4');" /></span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="title">
                                <span>升舱换开通道</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: left;">
                            <table>
                                <tr>
                                    <th>
                                        新PNR编码:
                                    </th>
                                    <td>
                                        <asp:TextBox ID="txtNewPNR" CssClass="txt" Width="60px" runat="server"></asp:TextBox>
                                    </td>
                                    <th>
                                        原订单号:
                                    </th>
                                    <td>
                                        <asp:TextBox ID="txtOldOrderNo" CssClass="txt" Width="150px" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="btn btn-ok-s">
                                            <asp:Button ID="btnSCHK" runat="server" Text=" 导 入 " OnClientClick="return StartImport(5);" /></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
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
    <%--pnr内容是否合并一起 0否 1是--%>
    <input id="Hid_PnrConIsAll" type="hidden" runat="server" value="1" />
    <%--指定的Office导入 默认空--%>
    <input id="Hid_Office" type="hidden" runat="server" value="" />
    <%--角色类型 1=平台，2=落地运营商，3=供应商，4=分销商，5=采购商--%>
    <input id="Hid_UserRoleType" type="hidden" runat="server" value="" />
    <%--儿童编码关联成人订单号 后台--%>
    <input id="Hid_OrderId" type="hidden" runat="server" value="" />
     <%--是否开启儿童必须关联成人订单号 1开启 0关闭--%>
    <input id="Hid_CHDOPENAsAdultOrder" type="hidden" runat="server" value="0" />
    </form>
</body>
</html>

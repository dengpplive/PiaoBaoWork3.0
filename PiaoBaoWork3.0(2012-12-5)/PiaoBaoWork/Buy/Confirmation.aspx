<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Confirmation.aspx.cs" Inherits="Buy_Confirmation"
    ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>确认订单</title>
    <link href="../css/List.css" rel="stylesheet" type="text/css" />
    <link href="../Css/main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/json2.js"></script>
    <script type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../Css/JPstep.css" rel="stylesheet" type="text/css" />
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
            left: 50%;
            top: 30%;
            z-index: 1000;
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
        #tabDistribution
        {
            width: 99%;
            display: none;
            margin-top: 5px;
            line-height: 30px;
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
        .tb
        {
            background-color: #DEECFF;
            border: 1px solid #B2DBFF;
            padding: 3px;
        }
        .tbs
        {
            background-color: #FFFFFF;
        }
        .tbstr
        {
            border-bottom: 1px dashed #D4D4D4;
        }
        .tbs tr
        {
            line-height: 28px;
        }
        .clk
        {
            background-color: #F1F1F1;
            border: 1px solid #CCCCCC;
            padding: 3px;
        }
        .clktab
        {
            background-color: #FFFFFF;
        }
        .clktab tr td
        {
            line-height: 30px;
            border-bottom: 1px dashed #D4D4D4;
        }
        .main-normal
        {
            padding: 0px;
        }
        .listing
        {
            background: none;
            border: none;
        }
        .sec2
        {
            background: url(../img/all_pic.gif);
            background-position: -79px -36px;
            text-align: center;
            cursor: hand;
            color: #fff;
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
            float: left;
        }
        .red
        {
            color: Red;
        }
        .green
        {
            color: Green;
        }
        
        /*选项卡样式start---------------------------------------------------------------------------------*/
        div.card div
        {
            padding: 0px;
            padding-bottom: 0;
            clear: both;
            width: 100%;
        }
        div.card a
        {
            color: black;
            font-size: 14px;
            text-decoration: none;
            float: left;
            width: 100px;
            text-align: center;
            margin-right: 1px;
        }
        /*选中的样式*/
        div.card a.sel
        {
            background-color: #5285B8;
            border: 1px solid #7BADF8;
            color: White;
        }
        /*没有选中的样式*/
        div.card a.nosel
        {
            background-color: #E6E6E6;
            border: 1px solid #7BADF8;
            color: Black;
        }
        div.card div.content
        {
            /* height: 100%;
            background-color: #DEECFF;
            border: 1px solid #B2DBFF;
            */
        }
        /*选项卡样式end---------------------------------------------------------------------------------*/
    </style>
    <script type="text/javascript" src="../js/js_Confirmation.js" language="javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div>
        <div class="main-normal">
            <div class="listing">
                <div style="padding: 10px;" class="normal-content">
                    <div class="titleheight">
                        <h3>
                            <asp:Label ID="lbltitles" runat="server">确认订单</asp:Label><asp:Label ID="lblPnr" runat="server"></asp:Label></h3>
                        <ul id="que" runat="server">
                            <li><em class="current"></em>预订</li>
                            <li><em class="current"></em>填写订单</li>
                            <li><em class="current"></em>确认</li>
                            <li><em class="bg"></em>支付</li>
                            <li><em></em>完成</li>
                        </ul>
                    </div>
                    <div class="normal-content mainnav" id="divstate">
                        <div class="DivDistribution">
                            <table style="width: 100%; text-align: center;" class="flighttab">
                                <tbody>
                                    <tr style="height: 30px; line-height: 30px;">
                                        <td align="left" colspan="11">
                                            <h5>
                                                航班信息<b>【温馨提示】请仔细确认您的航班信息，以免耽误您的行程。<span style="color: Red;">该项为单人价格。</span>订单支付总金额 =
                                                    单人结算价 * 人数</b></h5>
                                        </td>
                                    </tr>
                                    <tr class="info-head">
                                        <td style="width: 22%">
                                            航程
                                        </td>
                                        <td style="width: 15%">
                                            日期
                                        </td>
                                        <td style="width: 15%">
                                            起抵时间
                                        </td>
                                        <td style="width: 12%">
                                            承运人
                                        </td>
                                        <td style="width: 12%">
                                            航班号
                                        </td>
                                        <td style="width: 11%">
                                            <span id="JX_Header">机型</span>
                                        </td>
                                        <td style="width: 13%">
                                            舱位/折扣
                                        </td>
                                        <%--                      <td style="width: 14%">
                                            舱位价/原价
                                        </td>
                                        <td style="width: 10%">
                                            机建/燃油
                                        </td>
                                        <td style="width: 10%">
                                            票面总价
                                        </td>--%>
                                    </tr>
                                    <asp:Literal ID="literSKY" runat="server"></asp:Literal>
                                </tbody>
                            </table>
                        </div>
                        <div class="DivDistribution">
                            <h5>
                                政策选择 <b>【温馨提示】舱位价为PAT:A查询结果。</b> <b><span style="color: Red">该项为单人价格。</span> 订单支付总金额
                                    = 单人结算价 * 人数 </b><b><span style="color: Red">供应商政策可能有变动,请以当前页面政策为准</span> </b>
                                <input id="btnAgainGetPolicy" type="button" value="重新获取政策" onclick="Refresh()" class="hide btn big1 cp" />
                            </h5>
                            <%--政策列表--%>
                            <div id="PolicyDiv">
                                <div id="TempDiv" style="text-align: center;">
                                    政策信息加载中......<br />
                                    <img src="../img/loading.gif">
                                </div>
                                <div id="policyCon" class="hide">
                                    <div class="tb hide" id="div_Adult">
                                        <div class="card">
                                            <%--选项卡容器开始--%>
                                            <div onclick="MenuClick(event)" id="header">
                                                <a id="btnpt" href="#"  class="sel" onclick="return switchTab(this,'0');">普通政策</a>
                                                <a id="btngf" href="#"  class="nosel" onclick="return switchTab(this,'1');">特殊政策</a>
                                            </div>
                                            <div class="content">
                                                <table style="width: 100%;" class="tbs" id="tabPolicy">
                                                    <tr id="trHead" style="background-color: #FFFADA;" class="hide tbstr">
                                                        <th>
                                                            选择
                                                        </th>
                                                        <th>
                                                            舱位价
                                                        </th>
                                                        <th>
                                                            机建/燃油
                                                        </th>
                                                        <th id="thpmfare_0">
                                                            票面价
                                                        </th>
                                                        <th id="thfandian_0">
                                                            返点/佣金
                                                        </th>
                                                        <th id="thshifu_0">
                                                            实付金额
                                                        </th>
                                                        <th id="thcpxl_0">
                                                            出票速度
                                                        </th>
                                                        <th>
                                                            上班时间
                                                        </th>
                                                        <th>
                                                            废票改签时间
                                                        </th>
                                                        <th>
                                                            支付方式
                                                        </th>
                                                        <th>
                                                            政策类型
                                                        </th>
                                                    </tr>
                                                    <tr name="policy_0" ptype="0" title="" class="hide tbstr">
                                                        <td id="tdop_0" rowspan="2" class="tbstr">
                                                            <input id="rbtn_0" name="rbtnPolicy" type="radio" value="0" />
                                                        </td>
                                                        <td id="tdSeatPrice_0" title="0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                        </td>
                                                        <td id="tdABFee_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                        </td>
                                                        <td id="tdPMPrice_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                            <%--票面价--%>
                                                        </td>
                                                        <td id="tdfandian_0" style="color: #0055AA; font-weight: bold; font-size: 14px;">
                                                        </td>
                                                        <td id="tdshifu_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                        </td>
                                                        <td id="tdcpxl_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                        </td>
                                                        <td id="tdworktime_0" style="color: #666666; font-size: 14px;">
                                                        </td>
                                                        <td id="tdfpgq_0" style="color: #ff0000; font-size: 14px;">
                                                        </td>
                                                        <td id="tdzhifutype_0" style="color: #ff0000; font-size: 14px;">
                                                        </td>
                                                        <td id="tdpolicytype_0">
                                                        </td>
                                                    </tr>
                                                    <tr name="policy_0" ptype="0" style="color: #666666; font-size: 12px;" title="" class="hide">
                                                        <td id="tdbeizhuHead_0" class="tbstr">
                                                        </td>
                                                        <td id="beizhu_0" colspan="11" style="text-align: left;" class="tbstr">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                        <%--选项卡容器结束--%>
                                    </div>
                                    <div class="tb hide" id="div_CHD">
                                        <h5>
                                            儿童政策</h5>
                                        <table style="width: 100%;" class="tbs" id="tabChildPolicy">
                                            <tr style="background-color: #FFFADA;">
                                                <th>
                                                    选择
                                                </th>
                                                <th>
                                                    舱位价
                                                </th>
                                                <th>
                                                    机建/燃油
                                                </th>
                                                <th>
                                                    票面价
                                                </th>
                                                <th id="thCHDfandian">
                                                    返点/佣金
                                                </th>
                                                <th id="thCHDshifu">
                                                    实付金额
                                                </th>
                                                <th id="thCHDcpxl">
                                                    出票速度
                                                </th>
                                                <th>
                                                    上班时间
                                                </th>
                                                <th>
                                                    废票改签时间
                                                </th>
                                                <th>
                                                    支付方式
                                                </th>
                                                <th>
                                                    政策类型
                                                </th>
                                            </tr>
                                            <tr>
                                                <td id="tdopCHD_0">
                                                    <input id="rbtnchild_0" name="rCHD" type="radio" checked="checked" />
                                                </td>
                                                <td id="tdCHDSeatPrice_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDABfare_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDPMPrice_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                    <%--票面价--%>
                                                </td>
                                                <td id="tdCHDfandian_0" style="color: #0055AA; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDshifu_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDcpxl_0" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDworktime_0" style="color: #666666; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDdpgqtime_0" style="color: #ff0000; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDzffs_0" style="color: #ff0000; font-size: 14px;">
                                                </td>
                                                <td id="tdCHDPolicyType_0">
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="tb hide" id="div_INF">
                                        <h5>
                                            婴儿政策</h5>
                                        <table style="width: 100%;" class="tbs" id="tabINFPolicy">
                                            <tr style="background-color: #FFFADA;">
                                                <th>
                                                    选择
                                                </th>
                                                <th>
                                                    舱位价
                                                </th>
                                                <th>
                                                    机建/燃油
                                                </th>
                                                <th>
                                                    票面价
                                                </th>
                                                <th id="thINFfandian">
                                                    返点/佣金
                                                </th>
                                                <th id="thINFSF">
                                                    实付金额
                                                </th>
                                                <th>
                                                    上班时间
                                                </th>
                                                <th>
                                                    废票改签时间
                                                </th>
                                                <th>
                                                    政策类型
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <input id="rbtn_INF" name="rINF" type="radio" checked="checked" />
                                                </td>
                                                <td id="tdINFSeatPrice" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdINFABFare" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdINFPMPrice" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                    <%--票面价--%>
                                                </td>
                                                <td id="tdINFfandian" style="color: #0055AA; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdINFshifu" style="color: #FF6600; font-weight: bold; font-size: 14px;">
                                                </td>
                                                <td id="tdINFworktime" style="color: #666666; font-size: 14px;">
                                                </td>
                                                <td id="tdINFfpgqtime" style="color: #ff0000; font-size: 14px;">
                                                </td>
                                                <td id="tdINFPolicyType">
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="DivDistribution" id="PassengerDiv">
                            <div>
                                <h5>
                                    乘机人信息 <b>【温馨提示】请确定登机人信息，以免在购票过程中发生问题。</b></h5>
                            </div>
                            <div class="clk">
                                <table cellspacing="0" cellpadding="0" border="0" width="100%" class="clktab">
                                    <tbody>
                                        <asp:Literal ID="literPessStr" runat="server"></asp:Literal>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="DivDistribution">
                            <h5>
                                联系人信息 <b>【温馨提示】请准确填写联系人信息，重要信息我们会通过手机短信等方式通知您。</b>
                            </h5>
                            <table style="width: 99%">
                                <tbody>
                                    <tr>
                                        <td style="width: 6em; text-align: right;">
                                            姓名：
                                        </td>
                                        <td style="text-align: left;">
                                            <asp:Label ID="lblLinkMan" runat="server"></asp:Label>
                                            <span style="color: Red;" id="spanName"><b>*</b></span>
                                        </td>
                                        <td style="width: 6em; text-align: right;">
                                            手机号码：
                                        </td>
                                        <td style="text-align: left;">
                                            <asp:Label ID="lblLinkPhone" runat="server"></asp:Label>
                                            <span style="color: Red;" id="spanMobile"><b>*</b></span>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="DivDistribution">
                            <h5>
                                备注信息
                            </h5>
                            <asp:TextBox ID="txtRemak" runat="server" TextMode="MultiLine" Width="80%" Height="50px">
                            </asp:TextBox>
                        </div>
                        <%--<div id="PriceDiv">
                            总价：<span style="color: #FF3300; font-weight: bold;"><asp:Label ID="lblallp" runat="server"></asp:Label></span>元
                            共<asp:Label ID="lblprenum" runat="server"></asp:Label>人</div>--%>
                        <table width="100%">
                            <tbody>
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btnSub" name="btnSub" class="btn big1 cp" runat="server" Text="确认订单"
                                            OnClick="btnSub_Click" OnClientClick="return DataValidate(this);" disabled="disabled">
                                        </asp:Button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <%--使用的div层--%>
            <div id="show">
            </div>
            <div id="opshow">
            </div>
            <div id="overlay" style="display: none;">
            </div>
            <div id="loading" runat="server" style="display: none; background: url(../images/mainbg.gif) no-repeat;">
                请稍等，<br />
                <span id="spastr">订单正在生成中……</span><br />
                <img src="../img/loading.gif">
            </div>
        </div>
    </div>
    <%--选择的政策索引 成人--%>
    <input id="Hid_SelIndex" type="hidden" runat="server" value="0" />
    <%--选择的政策索引 儿童--%>
    <input id="Hid_SelChildIndex" type="hidden" runat="server" value="0" />
    <%--是否隐藏价格 返点佣金 实付价格 默认0显示 1隐藏--%>
    <input id="hidShowPrice" type="hidden" runat="server" value="0" />
    <%--是否隐藏政策返点  0 默认显示，1 隐藏 --%>
    <input id="Hid_IsPolicy" type="hidden" runat="server" value="0" />
    <%--平台接口是否低开 1是0 否--%>
    <input id="Hid_IsLower" type="hidden" runat="server" value="0" />
    <%--不出儿童票 是否显示 0显示 1不显示--%>
    <input id="Hid_NotChildTicketShow" type="hidden" runat="server" value="0" />
    <%--订单标志 0成人订单 1儿童订单 2成人和儿童两个订单--%>
    <input id="Hid_OrderFlag" type="hidden" runat="server" value="0" />
    <%--订单中是否有婴儿 0没有婴儿 1有--%>
    <input id="Hid_HasINF" type="hidden" runat="server" value="0" />
    <%--脚本执行语句--%>
    <asp:Literal ID="literText" runat="server"></asp:Literal>
    <%-- 成人高低价格--%>
    <input id="Hid_AdultGDPrice" type="hidden" runat="server" value="0" />
    <%--成人PAT有几个价格--%>
    <input id="Hid_AdultPriceCount" type="hidden" runat="server" value="1" />
    <%-- 儿童高低价格--%>
    <input id="Hid_ChildGDPrice" type="hidden" runat="server" value="0" />
    <%--儿童PAT有几个价格--%>
    <input id="Hid_ChildPriceCount" type="hidden" runat="server" value="1" />
    <%-- 婴儿高低价格--%>
    <input id="Hid_INFGDPrice" type="hidden" runat="server" value="0|0|0@0|0|0@0|0|0" />
    <%--婴儿PAT有几个价格--%>
    <input id="Hid_INFPriceCount" type="hidden" runat="server" value="1" />
    <%--是否开启大配置--%>
    <input id="Hid_IsOpenBigConfig" type="hidden" runat="server" value="0" />
    <%--政策数据--%>
    <input id="Hid_Data" type="hidden" runat="server" />
    <%--传递航段参数--%>
    <input id="Hid_FromCode" type="hidden" runat="server" />
    <input id="Hid_ToCode" type="hidden" runat="server" />
    <input id="Hid_MiddleCode" type="hidden" runat="server" />
    <input id="Hid_TravelType" type="hidden" runat="server" />
    <input id="Hid_CacheGUID" type="hidden" runat="server" />
    <input id="Hid_FromDate" type="hidden" runat="server" />
    <input id="Hid_ToDate" type="hidden" runat="server" />
    <input id="Hid_Pnr" type="hidden" runat="server" />
    <input id="Hid_BigPnr" type="hidden" runat="server" />
    <input id="Hid_CarrayCode" type="hidden" runat="server" />
    <input id="Hid_Space" type="hidden" runat="server" />
    <input id="Hid_Office" type="hidden" runat="server" />
    <%--政策组ID--%>
    <input id="Hid_GroupId" type="hidden" runat="server" />
    <%--成人订单号--%>
    <input id="Hid_OrderID" type="hidden" runat="server" />
    <%--是否为单击确认按钮标识 默认0否 1是--%>
    <input id="Hid_Isbtn" type="hidden" runat="server" value="0" />
    <%--确认按钮后保存页面政策显示状态--%>
    <input id="Hid_ViewState" type="hidden" runat="server" />
    <%--订单来源  1=白屏预订，2=PNR编码导入，3=PNR内容生成--%>
    <input id="Hid_OrderSourceType" type="hidden" runat="server" />
    <%--是否开启后返权限 1开启 0关闭--%>
    <input id="Hid_IsHouFanOpen" type="hidden" runat="server" value="0" />
    <%--是否儿童出成人票 1是0否--%>
    <input id="Hid_IsCHDETAdultTK" type="hidden" runat="server" value="0" />
    <%--匹配政策是否在落地运营商设置的工作时间内 1是 0否--%>
    <input id="Hid_IsInWorkTime" type="hidden" runat="server" value="1" />
    <%--共享政策是否有账户余额支付权限 1是0否--%>
    <input id="Hid_zhanghu" type="hidden" runat="server" value="0" />
    <%--共享政策是否有收银权限 1是 0否--%>
    <input id="Hid_shouying" type="hidden" runat="server" value="0" />
    </form>
</body>
</html>

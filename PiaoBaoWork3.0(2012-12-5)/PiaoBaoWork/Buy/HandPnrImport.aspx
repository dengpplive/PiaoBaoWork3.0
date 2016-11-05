<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HandPnrImport.aspx.cs" Inherits="Buy_HandPnrImport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../JS/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../JS/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../JS/My97DatePicker/WdatePicker.js"></script>
    <link type="text/css" href="../CSS/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../CSS/table.css" rel="stylesheet" />
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
        .ulClass
        {
            list-style-type: none;
            margin: 0 0;
            padding: 0 0;
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
        .blank
        {
            color: green;
            background-color: black;
            width: 800px;
        }
        .inputW150
        {
            width: 150px;
        }
        .inputW160
        {
            width: 160px;
        }
        .inputW95
        {
            width: 95px;
        }
         .inputW70
        {
            width: 70px;
        }
        .inputW50
        {
            width: 50px;
        }
    </style>
    <script type="text/javascript" src="../js/json2.js"></script>
    <script language="javascript" type="text/javascript" src="../js/js_HandImport.js"> </script>
    
</head>
<body>
    <div id="dialog">
    </div>
    <form id="form1" defaultbutton="btnCreate" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div>
        <div class="title">
            <span>手工订单</span></div>
        <div style="font-size: 18px; width: 100%;">
            <%--显示内容--%>
            <table id="tab2" class="detail" width="100%" cellspacing="0" cellpadding="5" border="0"
                align="center" style="padding: 5px;">
                <tr>
                    <td>
                        <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 0 0 5px 0;
                            border: 1px #e2f1f6 solid">
                            <img align="absmiddle" src="../Images/point.gif" />
                            <strong style="color: #ff9900">客户信息</strong>
                        </div>
                        <table id="table1" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                            border="0">
                            <thead>
                                <tr>
                                    <th style="width: 122px;">
                                        客户名称：
                                    </th>
                                    <td align="left" style="width: 162px; padding: 0 0 0 6px">
                                        <asp:Label runat="server" ID="lblCustomer"></asp:Label>
                                    </td>
                                    <th style="width: 122px;">
                                        公司编号：
                                    </th>
                                    <td align="left" style="width: 122px; padding: 0 0 0 6px">
                                        <asp:Label runat="server" ID="lblCpyNo"></asp:Label>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </thead>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 5px 0 5px 0;
                            border: 1px #e2f1f6 solid">
                            <img align="absmiddle" src="../Images/point.gif" />
                            <strong style="color: #ff9900">订单信息</strong><span runat="server" id="span_OrderId"
                                class="green" style="font-size: 20px;"></span>
                        </div>
                        <table id="tab_order" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                            border="0">
                            <thead>
                                <tr>
                                    <th>
                                        编码
                                    </th>
                                    <th>
                                        大编码
                                    </th>
                                    <th>
                                        预订Office号
                                    </th>
                                    <th>
                                        出票Office号
                                    </th>
                                    <th>
                                        创建公司
                                    </th>
                                    <th>
                                        订单所属公司
                                    </th>
                                    <th>
                                        政策类型
                                    </th>
                                    <th>
                                        订单状态
                                    </th>
                                    <th>
                                        订单来源
                                    </th>
                                    <th>
                                        创建时间
                                    </th>
                                    <th>
                                        票价(￥)
                                    </th>
                                    <th>
                                        机建(￥)
                                    </th>
                                    <th>
                                        燃油(￥)
                                    </th>
                                    <th>
                                        婴儿票面价(￥)
                                    </th>
                                    <th>
                                        政策(%)
                                    </th>
                                    <%--<th>
                                        结算价
                                    </th>--%>
                                </tr>
                            </thead>
                            <tr>
                                <td>
                                    <input id="o_orderpnr" type="text" size="8" maxlength="6"  class="inputW70"/>
                                </td>
                                <td>
                                    <input id="o_orderbigpnr" type="text" size="8" maxlength="6" class="inputW70"/>
                                </td>
                                <td>
                                    <input id="o_orderoffice" type="text" size="8" maxlength="6" class="inputW70"/>
                                </td>
                                <td>
                                    <input id="o_orderprintoffice" type="text" size="8" maxlength="6" class="inputW70" />
                                </td>
                                <td>
                                    <input id="o_ordercreatecompany" type="text" readonly="readonly" size="5" maxlength="100" />
                                </td>
                                <td>
                                    <input id="o_orderowncompany" type="text" readonly="readonly" size="5" maxlength="100" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="o_ddlPolicyType" Width="80px" runat="server">
                                        <asp:ListItem Value="1">B2B</asp:ListItem>
                                        <asp:ListItem Value="2">BSP</asp:ListItem>
                                        <%--  <asp:ListItem Value="3">B2B/BSP</asp:ListItem>--%>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="o_orderstatuscode" runat="server" Width="117px">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="o_ordersource" runat="server" Width="117px">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <input type="text" name="o_createtime" readonly="readonly" id="o_createtime" runat="server"
                                        class="Wdate inputtxtdat" style="width: 100px;" onfocus="WdatePicker({isShowClear:false,minDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd'})" />
                                </td>
                                <td>
                                    <input id="o_pmfee" type="text" size="4" class="inputW50"/>
                                </td>
                                <td>
                                    <input id="o_abfee" type="text" size="4" class="inputW50"/>
                                </td>
                                <td>
                                    <input id="o_fuelfee" type="text" size="4"  class="inputW50"/>
                                </td>
                                <td>
                                    <input id="o_babyfee" type="text" size="4" class="inputW50" />
                                </td>
                                <td>
                                    <input id="o_policy" type="text" size="4"  class="inputW50"/>
                                </td>
                                <%--<td>
                                    <input id="o_paymoney" type="text" size="4" />
                                </td>--%>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 5px 0 5px 0;
                            border: 1px #e2f1f6 solid">
                            <img align="absmiddle" src="../Images/point.gif" />
                            <strong style="color: #ff9900">航段信息</strong>
                        </div>
                        <table id="tab_sky" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                            border="0">
                            <thead>
                                <tr>
                                    <th>
                                        航空二字码
                                    </th>
                                    <th>
                                        航班号
                                    </th>
                                    <th>
                                        机型
                                    </th>
                                    <th>
                                        起飞城市
                                    </th>
                                    <th>
                                        到达城市
                                    </th>
                                    <th>
                                        起飞日期
                                    </th>
                                    <th>
                                        到达日期
                                    </th>
                                    <th>
                                        航站楼
                                    </th>
                                    <th>
                                        舱位
                                    </th>
                                    <th>
                                        Y舱票价(￥)
                                    </th>
                                    <th>
                                        舱位价(￥)
                                    </th>
                                    <th>
                                        机建(￥)
                                    </th>
                                    <th>
                                        燃油(￥)
                                    </th>
                                    <th>
                                        折扣
                                    </th>
                                    <th>
                                        操作
                                    </th>
                                </tr>
                                <tr id="trsky_0" class="hide">
                                    <td>
                                        <input id="sky_carrycode_0" type="text" size="4" maxlength="2" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="sky_flight_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="sky_aircraft_0" type="text" size="8" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <input name="txtFromCity" type="text" maxlength="3" id="txtFromCity_0" autocomplete="off"
                                            onpropertychange="txtSetSel(this,'ddlFromCity',0)" style="width: 30px; margin-bottom: 2px;
                                            margin-top: -2px;" />
                                        <select name="ddlFromCity" id="ddlFromCity_0" runat="server" onchange="ddlSetText(this,'txtFromCity',0)"
                                            style="width: 118px;">
                                            <%--<option value="">--出发城市--</option>--%>
                                        </select>
                                    </td>
                                    <td>
                                        <input name="txtToCity" type="text" maxlength="3" id="txtToCity_0" autocomplete="off"
                                            onpropertychange="txtSetSel(this,'ddlToCity',0)" style="width: 30px; margin-bottom: 2px;
                                            margin-top: -2px;" />
                                        <select name="ddlToCity" id="ddlToCity_0" runat="server" onchange="ddlSetText(this,'txtToCity',0)"
                                            style="width: 118px;">
                                            <%--<option value="">--出发城市--</option>--%>
                                        </select>
                                    </td>
                                    <td>
                                        <input type="text" name="sky_flystartdate_0" readonly="readonly" id="sky_flystartdate_0"
                                            runat="server" class="Wdate inputtxtdat" style="width: 130px;" onfocus="WdatePicker({isShowClear:false,autoPickDate:true,minDate:'%y-%M-%d %H:%m',dateFmt:'yyyy-MM-dd HH:mm'})" />
                                    </td>
                                    <td>
                                        <input type="text" name="sky_flyenddate_0" readonly="readonly" id="sky_flyenddate_0"
                                            runat="server" class="Wdate inputtxtdat" style="width: 130px;" onfocus="WdatePicker({isShowClear:false,autoPickDate:true,minDate:'%y-%M-%d %H:%m',dateFmt:'yyyy-MM-dd HH:mm'})" />
                                    </td>
                                    <td>
                                        <input id="sky_startterminal_0" type="text" size="4" class="inputW50"/>
                                    </td>
                                    <td>
                                        <input id="sky_space_0" type="text" size="4" maxlength="2" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="sky_yfareprice_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="sky_spaceprice_0" type="text" size="4" maxlength="6" class="inputW50"/>
                                    </td>
                                    <td>
                                        <input id="sky_abfee_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="sky_rqfee_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="sky_discount_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <div id="sky_opdiv_0">
                                            <a onclick="return addGroup(event,'sky',null)" href="#">添加</a>
                                        </div>
                                    </td>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <div>
                            <p align="center" style="padding: 3px 0 0 0">
                                <span class="btn btn-ok-s">
                                    <input id="btnAddSky" type="button" value="添加航段" onclick="addGroup(event,'sky',null)" />
                                </span><span class="btn btn-ok-s">
                                    <input id="btnDelSky" type="button" value="删除航段" onclick="removeGroup(event,'sky',null)" />
                                </span>
                            </p>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 5px 0 5px 0;
                            border: 1px #e2f1f6 solid">
                            <img align="absmiddle" src="../Images/point.gif" />
                            <strong style="color: #ff9900">乘机人信息</strong>
                        </div>
                        <table id="tab_pas" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                            border="0">
                            <thead>
                                <tr>
                                    <th>
                                        乘客姓名
                                    </th>
                                    <th>
                                        乘客类型
                                    </th>
                                    <th>
                                        证件类型
                                    </th>
                                    <th>
                                        证件号码
                                    </th>
                                    <th>
                                        手机
                                    </th>
                                    <th>
                                        舱位价(￥)
                                    </th>
                                    <th>
                                        机建费(￥)
                                    </th>
                                    <th>
                                        燃油费(￥)
                                    </th>
                                    <th>
                                        操作
                                    </th>
                                </tr>
                                <tr id="trpas_0" class="hide">
                                    <td>
                                        <input id="pas_pasname_0" type="text" maxlength="50" class="inputW150" />
                                    </td>
                                    <td>
                                        <select id="pas_type_0" style="width: 100px;" runat="server">
                                        </select>
                                    </td>
                                    <td>
                                        <select id="pas_cardtype_0" style="width: 100px;" runat="server">
                                        </select>
                                    </td>
                                    <td align="center">
                                        <input id="pas_cardnum_0" type="text" class="show inputW160" maxlength="20" />
                                        <input type="text" name="txtBirday_0" readonly="readonly" id="txtBirday_0" runat="server"
                                            class="hide Wdate inputtxtdat" style="width: 130px;" onfocus="WdatePicker({isShowClear:false,dateFmt:'yyyy-MM-dd'})" />
                                    </td>
                                    <td>
                                        <input id="pas_phone_0" type="text" maxlength="11" size="11" class="inputW95" />
                                    </td>
                                    <td>
                                        <input id="pas_seatprice_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="pas_abprice_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <input id="pas_rqprice_0" type="text" size="4" maxlength="6" class="inputW50" />
                                    </td>
                                    <td>
                                        <div id="pas_opdiv_0">
                                            <a onclick="return addGroup(event,'pas',null)" href="#">添加</a> &nbsp; <a href="#"
                                                onclick="return removeGroup(event,'pas','0')">删除</a>
                                        </div>
                                    </td>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <div>
                            <p align="center" style="padding: 3px 0 0 0">
                                <span class="btn btn-ok-s">
                                    <input id="btnAddPas" type="button" value="添加乘客" onclick="addGroup(event,'pas',null)" />
                                </span><span class="btn btn-ok-s">
                                    <input id="btnDelPas" type="button" value="删除乘客" onclick="removeGroup(event,'pas',null)" />
                                </span>
                            </p>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 5px 0 5px 0;
                            border: 1px #e2f1f6 solid">
                            <img align="absmiddle" src="../Images/point.gif" />
                            <strong style="color: #ff9900">编码信息</strong>
                        </div>
                        <table id="table5" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                            border="0">
                            <tr>
                                <td style="width: 80px;">
                                    PNR信息
                                </td>
                                <td align="left" style="padding: 5px">
                                    <textarea id="txtrtdata" cols="250" rows="5" class="blank" readonly="readonly"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 80px;">
                                    PAT信息
                                </td>
                                <td align="left" style="padding: 5px">
                                    <textarea id="txtpatdata" cols="250" rows="5" class="blank" readonly="readonly"></textarea>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="background: #eff8ff; padding: 3px 5px 3px 10px; font-size: 13px; margin: 5px 0 5px 0;
                            border: 1px #e2f1f6 solid">
                            <img align="absmiddle" src="../Images/point.gif" />
                            <strong style="color: #ff9900">订单备注</strong>
                        </div>
                        <table id="tab_remark" class="else-table" width="100%" cellspacing="0" cellpadding="0"
                            border="0">
                            <tr>
                                <td style="width: 80px;">
                                    备注：
                                </td>
                                <td align="left" style="padding: 5px">
                                    <textarea id="txtRemark" cols="250" rows="5" style="width: 800px;" runat="server"></textarea>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center; padding: 3px 0 0 0">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnCreate" runat="server" Text="生成订单" OnClientClick="return SaveHidden();" /></span>&nbsp;&nbsp;&nbsp;
                        <span class="btn btn-ok-s">
                            <input type="button" id="btnCancel" value="取消订单" onclick="return GoUrl('PnrImport.aspx?currentuserid=<%=this.mUser.id.ToString() %>');" />
                            </span>
                    <span id="JZ"></span>
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
    <%--所有信息--%>
    <input id="Hid_ALLInfo" type="hidden" runat="server" />
    <%--初始化航段实体--%>
    <input id="Hid_SkyModel" type="hidden" runat="server" />
    <%--初始化乘机人实体--%>
    <input id="Hid_PasModel" type="hidden" runat="server" />
    <%--0成人订单1儿童订单--%>
    <input id="Hid_OrderType" type="hidden" runat="server" />
    </form>
</body>
</html>

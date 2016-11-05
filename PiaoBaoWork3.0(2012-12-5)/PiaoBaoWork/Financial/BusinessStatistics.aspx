<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BusinessStatistics.aspx.cs" Inherits="Person_BusinessStatistics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<meta http-equiv="pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta http-equiv="expires" content="0" />

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>业务员统计</title>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/Statements.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <style type="text/css">
        #moreSearch th
        {
            width: 80px;
        }
        .Search th
        {
            width: 80px;
        }
        TABLE
        {
            font-size: 12px;
            line-height: 30px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            // Tabs
            $('#tabs').tabs();
        });
        function showdialog(t) {
            $("#dd").html(t);
            $("#dd").dialog({
                title: '标题',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $(this).dialog('close');
                    }
                }
            });
        }
        function getPageUrl(name) {
            var stime = "";
            var etime = "";
            try {
                if (document.getElementById("txtCPTime3") != null) {
                    stime = document.getElementById("txtCPTime3").value;
                }

                if (document.getElementById("txtCPTime4") != null) {
                    etime = document.getElementById("txtCPTime4").value;
                }
            } catch (e) { }
            window.location.href = "SalesStatisticsDetail.aspx?name=" + name + "&stime=" + stime + "&etime=" + etime;
        }

        var xmlHttp = null;

        function createXMLHttpRequest() {
            if (window.ActiveXObject) {
                xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            else if (window.XMLHttpRequest) {
                xmlHttp = new XMLHttpRequest();
            }
        }

        var pos = function (str) {
            //获取元素绝对位置
            var Left = 0, Top = 0;
            do { Left += str.offsetLeft, Top += str.offsetTop; }
            while (str = str.offsetParent);
            return { "Left": Left, "Top": Top };
        }

        function showDiv(str, type) {
            with (pos(str)) {
                //str.value = Top + "-" + Left;
                document.getElementById("divSearch").style.width = parseInt(str.style.width) + 2;
                document.getElementById("divSearch").style.height = "auto";
                document.getElementById("divSearch").style.top = Top + parseInt(str.style.height) + 1;
                document.getElementById("divSearch").style.left = Left + 1;                

                createXMLHttpRequest();

                xmlHttp.open("post", "../Ajax/ECSearch.aspx?type=" + type + "&val=" + encodeURI(str.value) + "&cpyno=" + document.getElementById("hidcpyno").value + "&currentuserid=" + document.getElementById("hiduserid").value, true);

                xmlHttp.onreadystatechange = callback;

                xmlHttp.send(null); //设置响应消息体,并发出请求,get方法没有消息体,所以为null
            }
        }

        function callback() {
            if (xmlHttp.readyState == 4) {  
                if (xmlHttp.status == 200) {
                    var sret = xmlHttp.responseText;
                    document.getElementById("divSearch").innerHTML = sret;

                    document.getElementById("divSearch").style.display = "";
                }
            }
        }


        function mouseOver(oId, Id, e) {
            var showDiv = document.getElementById("showDiv");

            createXMLHttpRequest();

            xmlHttp.open("post", "../Ajax/CustomerInfo.aspx?cpyname=" + encodeURI(Id), true);

            xmlHttp.onreadystatechange = callback1;

            xmlHttp.send(null); //设置响应消息体,并发出请求,get方法没有消息体,所以为null
        }

        function mouseOut() {
            document.getElementById("showDiv").style.display = "none";
        }

        function callback1() {
            if (xmlHttp.readyState == 4) {
                if (xmlHttp.status == 200) {
                    var sret = xmlHttp.responseText;

                    $("#showOne").html(sret);
                    $("#showOne").dialog({
                        title: '客户信息',
                        bgiframe: true,
                        width: 230,
                        height: 320,
                        modal: true,
                        overlay: {
                            backgroundColor: '#000',
                            opacity: 0.5
                        },
                        buttons: {
                            '确定': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                }
            }
        }

        function fnRowClick(objTD) {
            if (typeof (objTD.index) != "undefined") {
                document.getElementById("txtYG").value = objTD.innerHTML;
                document.getElementById("hfYG").value = objTD.index;
            } else {
                document.getElementById("txtTo").value = objTD.innerHTML;
            }
        }

        function fnClick() {
            if (event.srcElement != document.getElementById("txtYG") && event.srcElement != document.getElementById("txtTo")) {
                document.getElementById("divSearch").style.display = "none";
            } 
            else if (event.srcElement == document.getElementById("txtYG") && document.getElementById("txtYG").value != "") {
                document.getElementById("divSearch").innerHTML = "";
                showDiv(document.getElementById("txtYG"), 1);
            }
            else if (event.srcElement == document.getElementById("txtTo") && document.getElementById("txtTo").value != "") {
                document.getElementById("divSearch").innerHTML = "";
                showDiv(document.getElementById("txtTo"), 2);
            }
        }
    </script>
</head>
<body onclick="fnClick();">
    <form id="form1" runat="server" autocomplete="off">
    <div id="tabs">
        <div class="title">
            <span>业务员统计</span>
        </div>
        <div id="tabs-1">
            <table width="100%">
                <tr>
                    <td>
                        <div class="c-list-filter">
                            <div class="container">
                                <table class="Search" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <th runat="server" id="thYG" visible="false">
                                            员工：
                                        </th>
                                        <td runat="server" id="tdYG" visible="false">
                                        
                                            <asp:TextBox ID="txtYG" Width="110px" Height="20px" onchange=""  CssClass="inputtxtdat" runat="server" onkeyup="showDiv(this,1);"></asp:TextBox>
                                             <%--<asp:TextBox ID="txtYG" Width="110px" Height="20px" onchange=""  CssClass="inputtxtdat" runat="server"></asp:TextBox>--%>
                                            <asp:HiddenField ID="hfYG" runat="server" />
                                        </td>
                                        <th>
                                            客户名称：
                                        </th>
                                        <td>
                                            <asp:TextBox ID="txtTo" Width="110px" Height="20px" CssClass="inputtxtdat" runat="server" onkeyup="showDiv(this,2);"></asp:TextBox>
                                            <%--<asp:TextBox ID="txtTo" Width="110px" Height="20px" CssClass="inputtxtdat" runat="server"></asp:TextBox>--%>
                                        </td>
                                        <th>
                                            客户帐号：
                                        </th>
                                        <td>
                                            <asp:TextBox ID="txtAccount" Width="110px" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                                        </td>
                                        <th>
                                            出票时间：
                                        </th>
                                        <td>
                                            <input id="txtCPTime3" type="text" readonly="readonly" runat="server" class="Wdate inputtxtdat"
                                                onfocus="WdatePicker({isShowClear:false})" />-<input id="txtCPTime4" type="text" readonly="readonly"
                                                    runat="server" class="Wdate inputtxtdat" onfocus="WdatePicker({isShowClear:false})" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th colspan="2">
                                        是否有出票数据:
                                        </th>
                                        <td colspan="2">
                                            <asp:RadioButtonList ID="rblcp" runat="server" EnableViewState="false"
                                                RepeatDirection="Horizontal">
                                            <asp:ListItem Value="1" Selected="True">有</asp:ListItem>
                                            <asp:ListItem Value="0">无</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <th>
                                            排序：
                                        </th>
                                        <td>
                                            <asp:DropDownList runat="server" ID="ddlSort">
                                                <asp:ListItem Text="业务员" Value="业务员"></asp:ListItem>
                                                <asp:ListItem Text="出票" Value="出票数量" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="出票总价" Value="出票票价"></asp:ListItem>
                                                <asp:ListItem Text="退票" Value="退票数量"></asp:ListItem>
                                                <asp:ListItem Text="退票总价" Value="退票票价"></asp:ListItem>
                                                <asp:ListItem Text="废票" Value="废票数量"></asp:ListItem>
                                                <asp:ListItem Text="废票总价" Value="废票票价"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="c-list-filter-go">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tbody>
                                        <tr>
                                            <td align="left" colspan="4">
                                                <span class="btn btn-ok-s">
                                                    <asp:Button ID="btnQuery1" runat="server" Text=" 查 询 " OnClick="btnQuery1_Click">
                                                    </asp:Button>
                                                </span><span class="btn btn-ok-s" style="display: none">
                                                    <asp:Button ID="btnClear2" runat="server" Text="重置数据" OnClick="btnClear2_Click">
                                                    </asp:Button>
                                                </span><span class="btn btn-ok-s">
                                                    <asp:Button ID="btnOut" runat="server" Text="导出Excel" OnClick="btnOut_Click"></asp:Button>
                                                </span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                            <div>
                       <asp:GridView ID="gvBusinessInfo" Width="100%" runat="server" EnableViewState="false"
                            EmptyDataText="查无信息！" CssClass="tb-all-trade"
                                    onrowdatabound="gvBusinessInfo_RowDataBound" >
                        </asp:GridView>
                        <div style="display: none">
                            <asp:GridView ID="gvBusinessInfoNew" Width="100%" runat="server" EmptyDataText="查无信息！" EnableViewState="false"
                                CssClass="tb-all-trade">
                            </asp:GridView>
                        </div>
                       </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table width="100%" border="0" class="sugges">
        <tr>
            <td class="sugtitle">
                温馨提示：<br />
            </td>
        </tr>
        <tr>
            <td class="sugcontent">
                <span style="color: Red">统计时间跨度不能超过一个月。</span><br />
                请使用IE浏览器导出报表。
            </td>
        </tr>
    </table>
    <div id="dd">
    </div>
    <div id="showOne">
    </div>
    <div id="showDiv" style="display:none; z-index: 9999; position: absolute;">
    </div>
    <div id="divSearch" style="border:1px solid #000000;text-align:left;position:absolute;z-index:9999;display:none;background:white;">my div</div>
      <asp:HiddenField ID="hidcpyno" runat="server" />
      <asp:HiddenField ID="hiduserid" runat="server" />
    </form>
</body>
</html>

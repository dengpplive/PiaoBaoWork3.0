<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LowerFillList.aspx.cs" Inherits="Financial_LowerFillList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .ui-corner-all
        {
            padding: 1px 6px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            // Tabs
            $('#tabs').tabs();
        });
        function showdialog(t) {
            $("#show").html(t);
            $("#show").dialog({
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
                        $(this).dialog('close');
                    }
                }
            });
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
        function mouseOver(oId, Id, e) {
            var showDiv = document.getElementById("showDiv");

            createXMLHttpRequest();

            xmlHttp.open("post", "../Ajax/lowerFillCustomer.aspx?cpyno=" + encodeURI(Id), true);

            xmlHttp.onreadystatechange = callback1;

            xmlHttp.send(null); //设置响应消息体,并发出请求,get方法没有消息体,所以为null
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

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="show"></div>
     <div id="tabs">
        <div class="title">
            <span>客户账户管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter" runat="server" id="showtop">
                <div class="container" style="padding-bottom: 10px;">
                    <table class="Search" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            
                            <th>
                                客户名称：
                            </th>
                            <td>
                                <asp:TextBox ID="txtUninAllNAME" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                               <th>
                                客户账号：
                            </th>
                             <td>
                                <asp:TextBox ID="txtLoginName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                         
                            <td>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnQuery" runat="server" Text=" 查 询" OnClick="btnQuery_Click" CausesValidation="false">
                                    </asp:Button></span> <span class="btn btn-ok-s">
                                            <asp:Button runat="server" ID="btnPrint" Text="导出Excel" OnClick="btnPrint_Click">
                                            </asp:Button></span>
                                            <span class="btn btn-ok-s">
                                        <asp:Button ID="btnreset" runat="server" Text=" 重 置 " CausesValidation="false" OnClick="btnreset_Click">
                                        </asp:Button></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="0" cellpadding="0"
                border="0">
                <thead>
                    <tr>
                        <th>
                            客户名称
                        </th>
                        <th>
                            客户账号
                        </th>
                        <th>
                            账户余额
                        </th>
                        <th>
                            最大欠款额度
                        </th>
                        <th>
                            最大欠款天数
                        </th>
                    
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repPosList" runat="server" 
                OnItemCommand="repPosList_ItemCommand"
                    OnItemDataBound="repPosList_ItemDataBound">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                               <a href="javascript:mouseOver(null,'<%# Eval("unincode") %>',window.event)"> <%#Eval("uninAllNAME")%></a>
                            </td>
                            <td>
                                <%#Eval("LoginName")%>
                            </td>
                            <td>
                                <%# Eval("AccountMoney").ToString()%>
                            </td>
                            <td>
                               <%# Eval("MaxDebtMoney").ToString()%>
                            </td>
                            <td>
                               <%# Eval("MaxDebtDays").ToString()%>
                            </td>
                       
                        <td class="Operation">
                  
                              <div runat="server" id="divmake" visible="false"><a href='AccountChange.aspx?unincode=<%#Eval("unincode") %>&AccountMoney=<%#Eval("AccountMoney") %>&MaxDebtMoney=<%#Eval("MaxDebtMoney") %>&MaxDebtDays=<%# Eval("MaxDebtDays") %>&uninAllNAME=<%# encodeName(Eval("uninAllNAME").ToString()) %>&currentuserid=<%=this.mUser.id.ToString() %>'>账户操作</a> </div> 
                              <div runat="server" id="divclear" visible="false">
                             <asp:LinkButton ID="lnkbtnClear" runat="server" CommandName="Clear" CommandArgument='<%#Eval("unincode") %>'
                                        OnClientClick="return confirm('确定清空？')">清空支付密码</asp:LinkButton>
                             </div>
                             <div runat="server" id="divshowpwd" visible="false">
                             <div id="Div1" runat="server" visible='<%# Eval("AccountPwd") !=null && Eval("AccountPwd").ToString().Length != 0 ? true: false %>'>
                              <a href='CpyAccountPwd.aspx?cpyid=<%#Eval("id")%>&type=updatepwd&currentuserid=<%=this.mUser.id.ToString() %>'>修改密码</a>
                             </div>
                              <div id="Div2" runat="server" visible='<%# Eval("AccountPwd")==null || Eval("AccountPwd").ToString().Length == 0 ? true: false  %>'>
                              <a href='CpyAccountPwd.aspx?cpyid=<%#Eval("id")%>&type=setpwd&currentuserid=<%=this.mUser.id.ToString() %>'>设置密码</a>
                             </div>
                             </div>
                             <a href='PaymentRecord.aspx?cpyname=<%# encodeName(Eval("uninAllNAME").ToString())%>&paytype=all&currentuserid=<%=this.mUser.id.ToString() %>'>查看明细</a>
                             <br />
                             <a href='PaymentRecord.aspx?cpyname=<%# encodeName(Eval("uninAllNAME").ToString())%>&paytype=zh&currentuserid=<%=this.mUser.id.ToString() %>'>余额流水帐</a>
                             <br />
                             <a href='PaymentRecord.aspx?cpyname=<%#encodeName(Eval("uninAllNAME").ToString())%>&paytype=zx&currentuserid=<%=this.mUser.id.ToString() %>'>在线流水帐</a>  
                             <br />
                              <a href='PaymentRecord.aspx?cpyname=<%#encodeName(Eval("uninAllNAME").ToString())%>&paytype=pos&currentuserid=<%=this.mUser.id.ToString() %>'>pos机流水帐</a>  
                             <br />
                             <asp:Button ID="Button1" runat="server" 
                             Text='<%# returntext(Eval("SetValue").ToString()) %>' 
                             CommandArgument='<%# Eval("unincode") %>' 
                             CommandName='<%# returntext(Eval("SetValue").ToString()) %>' Visible="false" />
                             <%--<span runat="server" class="btn btn-ok-s" id="spanyck" visible='<%=  %>'> <asp:Button ID="btyck" runat="server" Text='<%# returntext(Eval("unincode").ToString()) %>' CommandArgument='<%# Eval("unincode") %>' CommandName='<%# returntext(Eval("unincode").ToString()) %>' /></span>           --%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                
            </table>
        </div>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
            CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
            PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
            AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
            EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
            ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
            SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
        </webdiyer:AspNetPager>
    </div>
     <div style="display:none" >
                            <asp:GridView ID="GridView1" Width="100%" runat="server" EmptyDataText="查无信息！"
                                CssClass="tb-all-trade">
                            </asp:GridView>
                        </div>
    <div id="showOne">
    </div>
    <div id="showDiv" style="display:none; z-index: 9999; position: absolute;">
    </div>
    </form>
</body>
</html>

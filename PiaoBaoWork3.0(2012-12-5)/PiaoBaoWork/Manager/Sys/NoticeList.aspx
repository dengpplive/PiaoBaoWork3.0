<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NoticeList.aspx.cs" Inherits="Sys_NoticeList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>公告管理</title>
    <style type="text/css">
        .s
        {
            border: 1px solid #000000;
            background-color: #666666;
        }
        .paginator
        {
            font: 11px Arial;
            padding: 10px 20px 10px 0; /*&#19978;&#21491;&#19979;&#24038;*/
            margin: 0px;
        }
        .paginator a
        {
            padding: 1px 6px;
            border: solid 1px #ddd;
            background: #fff;
            text-decoration: none;
            margin-right: 2px;
        }
        .paginator .cpb
        {
            padding: 1px 6px;
            font-weight: bold;
            font-size: 13px;
            border: none;
        }
        .paginator a:hover
        {
            color: #fff;
            background: #ffa501;
            border-color: #ffa501;
            text-decoration: none;
        }
    </style>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../../js/area.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showimg(obj) {
            $("#"+obj).show();
        }
        function hideimg(obj) {
            $("#"+obj).hide();
        }
        function showdialog(t,p) {
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
                        if(p!=null) {
                            if(p=="1") {
                                $("input[id*='btnUpdate']").show();
                                $("#SelButton").click();
                            } else if(p=="2") {
                                history.go(-1);
                            }
                        }
                    }
                }
            });
        }
        function showdialog2(t) {
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
        $(function () {
            // Tabs
            $('#tabs').tabs();
            //&#25353;&#29275;&#26679;&#24335;
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); }
				);

        });
    </script>
    <script type="text/javascript">
        function moreSearchOrder() {
            if($("#moreSearchOrder").css("display")=="none") {
                $("#moreSearchOrderA").attr("class","MoreCondition");
            }
            else {
                $("#moreSearchOrderA").attr("class","MoreConditionA");
            }
            $("#moreSearchOrder").toggle('slow');
        }
        //批量操作
        function PatchUpdate(obj) {
            var flag=false;
            var hidArr=[];
            $("input[name='ckbox'][type='checkbox']:checked").each(function (index,ck) {
                var val=$.trim($(ck).val());
                hidArr.push("'"+val+"'");
            });
            if(hidArr.length>0) {
                $("#hid_ids").val(hidArr.join('#####'));
                flag=true;
                $(obj).hide();
            } else {
                showdialog("请选择需要操作的公告复选框！");
                $("#hid_ids").val("");
            }
            return flag;
        }
        function selAll(obj) {
            $("input[name='ckbox'][type='checkbox']").attr("checked",obj.checked);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div id="tabs">
        <div class="title">
            <span>公告管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container">
                    <table border="0" cellpadding="0" cellspacing="0" class="Search">
                        <tr>
                            <th>
                                公告标题：
                            </th>
                            <td>
                                <asp:TextBox ID="NoticeTitle" CssClass="inputtxtdat" runat="server" MaxLength="25"></asp:TextBox>
                            </td>
                            <th id="dd2">
                                公告状态：
                            </th>
                            <td>
                                <asp:DropDownList ID="CallboardType" runat="server" Width="115px">
                                    <asp:ListItem Value="==请选择状态==">==请选择状态==</asp:ListItem>
                                    <asp:ListItem Value="1">已审</asp:ListItem>
                                    <asp:ListItem Value="2">未审</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <th id="dd1">
                                发布人姓名：
                            </th>
                            <td>
                                <asp:TextBox ID="ReleaseName" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                发布时间始：
                            </th>
                            <td>
                                <asp:TextBox ID="ReleaseDateSta" CssClass="Wdate inputtxtdat" runat="server" class="Wdate"
                                    EnableViewState="False" onfocus="WdatePicker({isShowWeek:false})"></asp:TextBox>
                            </td>
                            <th>
                                发布时间止：
                            </th>
                            <td>
                                <asp:TextBox ID="ReleaseDateStp" CssClass="Wdate inputtxtdat" runat="server" class="Wdate"
                                    EnableViewState="False" onfocus="WdatePicker({isShowWeek:false})"></asp:TextBox>
                            </td>
                            <td colspan="2">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="SelButton" runat="server" Text=" 查 询 " OnClick="SelButton_Click" /></span>
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnUpdate1" runat="server" Text=" 批量审核 " OnClientClick="return PatchUpdate(this);"
                                        OnClick="btnUpdate1_Click" /></span> <span class="btn btn-ok-s">
                                            <asp:Button ID="btnUpdate0" runat="server" Text=" 批量撤审 " OnClientClick="return PatchUpdate(this);"
                                                OnClick="btnUpdate0_Click" /></span> <span class="btn btn-ok-s">
                                                    <asp:Button ID="Reset" runat="server" Text="重置数据" OnClick="Reset_Click" /></span>
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
                            <input type="checkbox" id="ck_all" onclick="selAll(this)" />
                        </th>
                        <th>
                            公告标题
                        </th>
                        <th>
                            发布人
                        </th>
                        <th>
                            发布时间
                        </th>
                        <th>
                            生效时间
                        </th>
                        <th>
                            失效时间
                        </th>
                        <th>
                            状态
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="Repeater" runat="server" OnItemCommand="Repeater_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td>
                                <input type="checkbox" id="ck_box" name="ckbox" value='<%#Eval("id")%>' />
                            </td>
                            <td>
                                <a href='NoticeDetail.aspx?id=<%#Eval("id")%>' target="_blank" style="text-decoration: none;">
                                    <%# Eval("Title") %></a>
                            </td>
                            <td>
                                <%#Eval("ReleaseName")%>
                            </td>
                            <td>
                                <%#Eval("ReleaseTime")%>
                            </td>
                            <td>
                                <%#Eval("StartDate")%>
                            </td>
                            <td>
                                <%#Eval("ExpirationDate")%>
                            </td>
                            <td>
                                <%# Eval("CallboardType").ToString()=="1" ? "已审":"未审" %>
                            </td>
                            <td class="Operation">
                                <a href="NoticeEdit.aspx?id=<%#Eval("id")%>&currentuserid=<%=this.mUser.id.ToString() %>">修 改 </a>
                                <br />
                                <asp:LinkButton ID="lbdelete" runat="server" Text=" 删 除 " CommandArgument='<%#Eval("id")%>'
                                    CommandName="Del" OnClientClick=" return  confirm('确定要删除?');" /><br />
                                <asp:LinkButton ID="UpDateButton" runat="server" Text='<%# Eval("CallboardType").ToString()=="1" ? "撤 审":"审 核" %>'
                                    CommandArgument='<%#Eval("id")%>' CommandName="Update" Visible="<%# mCompany.UninCode.Length == 6 ? false : true %>"
                                    OnClientClick=" return  confirm('确定要修改状态?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
            </webdiyer:AspNetPager>
            <input type="hidden" id="hid_ids" runat="server" />
        </div>
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BaseDictionaryList.aspx.cs"
    Inherits="Manager_Base_BaseDictionaryList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>字典表管理</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
        .Mleft
        {
            text-align: left;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            // Tabs
            // $('#tabs').tabs();

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

        //修改
        function EditBtn(id) {

            $("#HidValue").val(id);
            $("#"+id).hide();
            var otherid="";
            otherid=id.replace("lbtn_Edit","lbtn_Del");
            $("#"+otherid).hide(); //隐藏删除

            otherid=id.replace("lbtn_Edit","lbtn_Update");
            $("#"+otherid).show(); //显示修改
            otherid=id.replace("lbtn_Edit","lbtn_Cannl");
            $("#"+otherid).show(); //显示取消

            otherid=id.replace("lbtn_Edit","txt_ParentID");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Edit","txt_ParentName");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Edit","txt_ChildID");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Edit","txt_ChildName");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Edit","txt_ChildDescription");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Edit","txt_Remark");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Edit","txt_A1");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Edit","txtXePnr");
            $("#"+otherid).show();

            otherid=id.replace("lbtn_Edit","lbl_ParentID");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Edit","lbl_ParentName");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Edit","lbl_ChildID");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Edit","lbl_ChildName");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Edit","lbl_ChildDescription");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Edit","lbl_Remark");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Edit","lbl_A1");
            $("#"+otherid).hide();


            otherid=id.replace("lbtn_Edit","lbl_Xepnr");
            $("#"+otherid).hide();

            //RepBaseDictionary_lbtn_Cannl_0


            return false;
        }

        //取消
        function CannlBtn(id) {

            $("#HidValue").val(id);

            $("#"+id).hide();
            var otherid="";
            otherid=id.replace("lbtn_Cannl","lbtn_Update");
            $("#"+otherid).hide(); //隐藏更新

            otherid=id.replace("lbtn_Cannl","lbtn_Edit");
            $("#"+otherid).show(); //显示修改
            otherid=id.replace("lbtn_Cannl","lbtn_Del");
            $("#"+otherid).show(); //显示删除

           
            $("#"+otherid).hide(); 
           
            otherid=id.replace("lbtn_Cannl","txt_ParentID");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Cannl","txt_ParentName");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Cannl","txt_ChildID");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Cannl","txt_ChildName");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Cannl","txt_ChildDescription");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Cannl","txt_Remark");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Cannl","txt_A1");
            $("#"+otherid).hide();
            otherid=id.replace("lbtn_Edit","txtXePnr");
            $("#"+otherid).hide();


            otherid=id.replace("lbtn_Cannl","lbl_ParentID");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Cannl","lbl_ParentName");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Cannl","lbl_ChildID");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Cannl","lbl_ChildName");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Cannl","lbl_ChildDescription");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Cannl","lbl_Remark");
            $("#"+otherid).show();
            otherid=id.replace("lbtn_Cannl","lbl_A1");
            $("#"+otherid).show();

            otherid=id.replace("lbtn_Edit","lbl_Xepnr");
            $("#"+otherid).show();

            return false;
        }
    </script>
</head>
<body>
    <form id="form2" runat="server">
    <div id="tabs">
        <div class="title">
            <span>字典表管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                        border="1" style="border-collapse: collapse;">
                        <tr>
                            <td valign="top" style="width: 20%">
                                <table class="tb-all-trade" width="100%" cellspacing="" cellpadding="0" border="1"
                                    style="border-collapse: collapse;">
                                    <tr>
                                        <td>
                                            父级信息
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:BulletedList ID="blsList" runat="server" DisplayMode="LinkButton" OnClick="blsList_Click">
                                            </asp:BulletedList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            父级名称:
                                            <asp:TextBox ID="txtParentName" runat="server" Text='' Width="90px"></asp:TextBox>
                                            <span class="btn btn-ok-s">
                                                <asp:Button ID="btnAddParentName" runat="server" Text="添加父级" OnClick="btnAddParentName_Click" /></span>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top" style="width: 70%">
                                <table class="tb-all-trade" width="100%" cellspacing="" cellpadding="0" border="1"
                                    style="border-collapse: collapse;">
                                    <tr>
                                        <td valign="top">
                                            <asp:Label runat="server" ID="lblTitle" Style="color: #0C88BE; font-size: 12px; font-weight: bolder;"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table cellspacing="0" rules="rows" style="border-color: #CCCCCC; border-width: 1px;
                                                border-style: solid; width: 100%; border-collapse: collapse;">
                                                <tr align="center" style="color: White; background-color: #60B3F6; font-weight: bold;
                                                    height: 30px;">
                                                    <th scope="col">
                                                        父级序号
                                                    </th>
                                                    <th scope="col">
                                                        父级名称
                                                    </th>
                                                    <th scope="col">
                                                        子级序号
                                                    </th>
                                                    <th scope="col">
                                                        子级名称
                                                    </th>
                                                    <th scope="col">
                                                        子级描述
                                                    </th>
                                                    <th scope="col">
                                                        备注
                                                    </th>
                                                    <th scope="col">
                                                        A1备用
                                                    </th>
                                                    <th scope="col">
                                                        A3备用
                                                    </th>
                                                    <th scope="col">
                                                        操作
                                                    </th>
                                                </tr>
                                                <asp:Repeater ID="RepBaseDictionary" runat="server" OnItemCommand="RepBaseDictionary_ItemCommand"
                                                    OnItemDataBound="RepBaseDictionary_ItemDataBound">
                                                    <ItemTemplate>
                                                        <tr align="center" style="background-color: White; height: 25px;">
                                                            <td>
                                                                <asp:Label ID="lbl_ParentID" runat="server" Text='<%#Eval("ParentID") %>' ToolTip='<%#Eval("ParentID").ToString()%>'></asp:Label>
                                                                <asp:TextBox ID="txt_ParentID" runat="server" Text='<%#Eval("ParentID")%>' Style="display: none;
                                                                    width: 90%;"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lbl_ParentName" runat="server" Text='<%#Eval("ParentName") %>'></asp:Label>
                                                                <asp:TextBox ID="txt_ParentName" runat="server" Text='<%#Eval("ParentName")%>' Style="display: none;
                                                                    width: 90%;"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lbl_ChildID" runat="server" Text='<%#Eval("ChildID") %>'></asp:Label>
                                                                <asp:TextBox ID="txt_ChildID" runat="server" Text='<%#Eval("ChildID")%>' Style="display: none;
                                                                    width: 90%;"></asp:TextBox>
                                                            </td>
                                                            <td title='<%#Eval("ChildName")%>'>
                                                                <asp:Label ID="lbl_ChildName" runat="server" Text='<%# SubChar(Eval("ChildName"), 20, "...")%>'></asp:Label>
                                                                <asp:TextBox ID="txt_ChildName" runat="server" Text='<%#Eval("ChildName")%>' Style="display: none;
                                                                    width: 90%;"></asp:TextBox>
                                                            </td>
                                                            <td title='<%#Eval("ChildDescription")%>'>
                                                                <asp:Label ID="lbl_ChildDescription" runat="server" Text='<%# SubChar(Eval("ChildDescription"), 20, "...")%>'></asp:Label>
                                                                <asp:TextBox ID="txt_ChildDescription" runat="server" Text='<%#Eval("ChildDescription")%>'
                                                                    Style="display: none; width: 90%;"></asp:TextBox>
                                                            </td>
                                                            <td title='<%#Eval("Remark")%>'>
                                                                <asp:Label ID="lbl_Remark" runat="server" Text='<%#SubChar(Eval("Remark"), 20, "...")%>'></asp:Label>
                                                                <asp:TextBox ID="txt_Remark" runat="server" Text='<%#Eval("Remark")%>' Style="display: none;
                                                                    width: 90%;"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <% if (ParentID == "21" || ParentID == "27")
                                                                   { %>
                                                                <asp:CheckBox ID="zkIsZY" runat="server" Text="自愿" />
                                                                <asp:HiddenField ID="Hid_IsZY" runat="server" Value='<%#Eval("A1")%>' />
                                                                <%}
                                                                   else
                                                                   { %>
                                                                <asp:Label ID="lbl_A1" runat="server" Text='<%#Eval("A1") %>'></asp:Label>
                                                                <asp:TextBox ID="txt_A1" runat="server" Text='<%#Eval("A1")%>' Style="display: none;
                                                                    width: 90%;"></asp:TextBox>
                                                                <%} %>
                                                            </td>
                                                            <td>
                                                                <% if (ParentID == "21" || ParentID == "27")
                                                                   { %>
                                                                <asp:CheckBox ID="ckXePnr" runat="server" Text="操作编码" />
                                                                <asp:HiddenField ID="Hid_XePnr" runat="server" Value='<%#Eval("A3")%>' />
                                                                <%}
                                                                   else
                                                                   { %>
                                                                <asp:Label ID="lbl_Xepnr" runat="server" Text='<%#Eval("A3") %>'></asp:Label>
                                                                <asp:TextBox ID="txtXePnr" runat="server" Text='<%#Eval("A3")%>' Style="display: none;
                                                                    width: 90%;"></asp:TextBox>
                                                                <%} %>
                                                            </td>
                                                            <td>
                                                                <asp:LinkButton ID="lbtn_Edit" runat="server" OnClientClick="return EditBtn(this.id)">修改</asp:LinkButton>
                                                                <asp:LinkButton ID="lbtn_Del" runat="server" CommandArgument='<%#Eval("Id")%>' OnClientClick="return confirm('确定要删除吗？请谨慎操作！');"
                                                                    CommandName="Del">删除</asp:LinkButton>
                                                                <asp:LinkButton ID="lbtn_Update" runat="server" CommandArgument='<%#Eval("Id")%>'
                                                                    CommandName="Update" Style="display: none;">更新</asp:LinkButton>
                                                                <asp:LinkButton ID="lbtn_Cannl" runat="server" OnClientClick="return CannlBtn(this.id)"
                                                                    Style="display: none;">取消</asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdAddChildName" runat="server" visible="false">
                                            <table class="tb-all-trade" width="100%" cellspacing="" cellpadding="0" border="1"
                                                style="border-collapse: collapse;">
                                                <tr>
                                                    <td>
                                                        子级名称
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtChildName" runat="server" Text='' Width="90px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        子级描述
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtChildDescription" runat="server" Text='' Width="90px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        备注
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtRemark" runat="server" Text='' Width="90px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        A1备用
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtA1" runat="server" Text='' Width="90px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="10">
                                                        <span class="btn btn-ok-s">
                                                            <asp:Button ID="btnAddChildName" runat="server" Text="添加子级" OnClick="btnAddChildName_Click" /></span>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div id="dd">
        </div>
        <input id="HidValue" runat="server" type="hidden" />
        <script type="text/javascript" src="../../js/Tooltip/ToolTip.js"> </script>
        <script language="javascript">
          <!--
            initToolTip("td");
          -->
        </script>
    </div>
    </form>
</body>
</html>

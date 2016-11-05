<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BasePageList.aspx.cs" Inherits="Manager_Base_BasePageList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>页面权限管理</title>
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
            $("#" + id).hide();
            var otherid = "";
            otherid = id.replace("lbtn_Edit", "lbtn_Del");
            $("#" + otherid).hide(); //隐藏删除

            otherid = id.replace("lbtn_Edit", "lbtn_Update");
            $("#" + otherid).show(); //显示修改
            otherid = id.replace("lbtn_Edit", "lbtn_Cannl");
            $("#" + otherid).show(); //显示取消

            otherid = id.replace("lbtn_Edit", "txt_ModuleIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_ModuleName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_OneMenuIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_OneMenuName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_TwoMenuIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_TwoMenuName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_PageIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_PageName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_PageURL");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Edit", "txt_Remark");
            $("#" + otherid).show();


            otherid = id.replace("lbtn_Edit", "lbl_ModuleIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_ModuleName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_OneMenuIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_OneMenuName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_TwoMenuIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_TwoMenuName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_PageIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_PageName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_PageURL");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Edit", "lbl_Remark");
            $("#" + otherid).hide();

            return false;
        }

        //取消
        function CannlBtn(id) {

            $("#HidValue").val(id);

            $("#" + id).hide();
            var otherid = "";
            otherid = id.replace("lbtn_Cannl", "lbtn_Update");
            $("#" + otherid).hide(); //隐藏更新

            otherid = id.replace("lbtn_Cannl", "lbtn_Edit");
            $("#" + otherid).show(); //显示修改
            otherid = id.replace("lbtn_Cannl", "lbtn_Del");
            $("#" + otherid).show(); //显示删除

            otherid = id.replace("lbtn_Cannl", "txt_ModuleIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_ModuleName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_OneMenuIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_OneMenuName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_TwoMenuIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_TwoMenuName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_PageIndex");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_PageName");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_PageURL");
            $("#" + otherid).hide();
            otherid = id.replace("lbtn_Cannl", "txt_Remark");
            $("#" + otherid).hide();

            otherid = id.replace("lbtn_Cannl", "lbl_ModuleIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_ModuleName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_OneMenuIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_OneMenuName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_TwoMenuIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_TwoMenuName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_PageIndex");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_PageName");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_PageURL");
            $("#" + otherid).show();
            otherid = id.replace("lbtn_Cannl", "lbl_Remark");
            $("#" + otherid).show();

            return false;
        }
    </script>
</head>
<body>
    <form id="form2" runat="server">
    <div id="tabs">
        <div class="title">
            <span>页面权限管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                        border="1" style="border-collapse: collapse;">
                        <tr>
                            <td style="width: 8%">
                                模块编号
                            </td>
                            <td style="width: 8%">
                                <asp:TextBox ID="txtModuleIndex" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td style="width: 8%">
                                模块名称
                            </td>
                            <td style="width: 8%">
                                <asp:TextBox ID="txtModuleName" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td style="width: 8%">
                                一级菜单编号
                            </td>
                            <td style="width: 8%">
                                <asp:TextBox ID="txtOneMenuIndex" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td style="width: 8%">
                                一级菜单名称
                            </td>
                            <td style="width: 8%">
                                <asp:TextBox ID="txtOneMenuName" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td style="width: 8%">
                                二级菜单编号
                            </td>
                            <td style="width: 8%">
                                <asp:TextBox ID="txtTwoMenuIndex" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td style="width: 8%">
                                二级菜单名称
                            </td>
                            <td style="width: 8%">
                                <asp:TextBox ID="txtTwoMenuName" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                用户类型:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlType" runat="server">
                                    <asp:ListItem Text="平台" Value="1" Selected="true"></asp:ListItem>
                                    <asp:ListItem Text="运营商" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="供应商" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="分销商" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="采购商" Value="5"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                页面编号
                            </td>
                            <td>
                                <asp:TextBox ID="txtPageIndex" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td>
                                页面名称
                            </td>
                            <td>
                                <asp:TextBox ID="txtPageName" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td>
                                页面地址
                            </td>
                            <td>
                                <asp:TextBox ID="txtPageURL" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>
                            <td>
                                备注
                            </td>
                            <td >
                                <asp:TextBox ID="txtRemark" runat="server" Text='' Width="90px"></asp:TextBox>
                            </td>

                            <td></td>
                             <td></td>

                              </tr>
                        <tr>

                            <td colspan="12">
                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnSel" runat="server" Text=" 查 询 " OnClick="btnSel_Click" /></span>


                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnAdd" runat="server" Text=" 添 加 " OnClick="btnAdd_Click" /></span>


                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnUpdateUserPage" runat="server" Text="更新用户页面权限" 
                                    onclick="btnUpdateUserPage_Click"  /></span>

                                <span class="btn btn-ok-s">
                                    <asp:Button ID="btnClear" runat="server" Text="清空" 
                                    onclick="btnClear_Click"  /></span>

                            </td>
                        </tr>
                        <tr>
                            <td colspan="12">
                                <table cellspacing="0" rules="rows" style="border-color: #CCCCCC; border-width: 1px;
                                    border-style: solid; width: 100%; border-collapse: collapse;">
                                    <tr align="center" style="color: White; background-color: #60B3F6; font-weight: bold;
                                        height: 30px;">
                                        <th scope="col">
                                            模块编号
                                        </th>
                                        <th scope="col">
                                            模块名称
                                        </th>
                                        <th scope="col">
                                            一级菜单编号
                                        </th>
                                        <th scope="col">
                                            一级菜单名称
                                        </th>
                                        <th scope="col">
                                            二级菜单编号
                                        </th>
                                        <th scope="col">
                                            二级菜单名称
                                        </th>
                                        <th scope="col">
                                            页面编号
                                        </th>
                                        <th scope="col">
                                            页面名称
                                        </th>
                                        <th scope="col">
                                            页面地址
                                        </th>
                                        <th scope="col">
                                            备注
                                        </th>
                                        <th scope="col">
                                            操作
                                        </th>
                                    </tr>
                                    <asp:Repeater ID="RepBasePage" runat="server" OnItemCommand="RepBasePage_ItemCommand">
                                        <ItemTemplate>
                                            <tr align="center" style="background-color: White; height: 25px;">
                                                <td>
                                                    <asp:Label ID="lbl_ModuleIndex" runat="server" Text='<%#Eval("ModuleIndex") %>' ToolTip='<%#Eval("ModuleIndex").ToString()%>'></asp:Label>
                                                    <asp:TextBox ID="txt_ModuleIndex" runat="server" Text='<%#Eval("ModuleIndex")%>'
                                                        Style="display: none; width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_ModuleName" runat="server" Text='<%#Eval("ModuleName") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_ModuleName" runat="server" Text='<%#Eval("ModuleName")%>' Style="display: none;
                                                        width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_OneMenuIndex" runat="server" Text='<%#Eval("OneMenuIndex") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_OneMenuIndex" runat="server" Text='<%#Eval("OneMenuIndex")%>'
                                                        Style="display: none; width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_OneMenuName" runat="server" Text='<%#Eval("OneMenuName") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_OneMenuName" runat="server" Text='<%#Eval("OneMenuName")%>'
                                                        Style="display: none; width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_TwoMenuIndex" runat="server" Text='<%#Eval("TwoMenuIndex") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_TwoMenuIndex" runat="server" Text='<%#Eval("TwoMenuIndex")%>'
                                                        Style="display: none; width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_TwoMenuName" runat="server" Text='<%#Eval("TwoMenuName") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_TwoMenuName" runat="server" Text='<%#Eval("TwoMenuName")%>'
                                                        Style="display: none; width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_PageIndex" runat="server" Text='<%#Eval("PageIndex") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_PageIndex" runat="server" Text='<%#Eval("PageIndex")%>' Style="display: none;
                                                        width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_PageName" runat="server" Text='<%#Eval("PageName") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_PageName" runat="server" Text='<%#Eval("PageName")%>' Style="display: none;
                                                        width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_PageURL" runat="server" Text='<%#Eval("PageURL") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_PageURL" runat="server" Text='<%#Eval("PageURL")%>' Style="display: none;
                                                        width: 90%;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lbl_Remark" runat="server" Text='<%#Eval("Remark") %>'></asp:Label>
                                                    <asp:TextBox ID="txt_Remark" runat="server" Text='<%#Eval("Remark")%>' Style="display: none;
                                                        width: 90%;"></asp:TextBox>
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
                    </table>
                </div>
            </div>
        </div>
        <div id="dd">
        </div>
        <input id="HidValue" runat="server" type="hidden" />
    </div>
    </form>
</body>
</html>

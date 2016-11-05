<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserPermissionsEdit.aspx.cs"
    Inherits="User_UserPermissionsEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title></title>
    <meta http-equiv="x-ua-compatible" content="ie=7" />
    <link href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
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
        function OnTreeNodeChecked()  //选择时：上级选中（但是不能同步取消），下级同步选中及取消选择
        {
            var ele = event.srcElement;
            if (ele.type == 'checkbox') {
                var childrenDivID = ele.id.replace('CheckBox', 'Nodes');
                var div = document.getElementById(childrenDivID);

                check(ele.id); //把该节点的父亲节点都选上

                if (div != null)  //有子节点
                {
                    var checkBoxs = div.getElementsByTagName('INPUT');
                    for (var i = 0; i < checkBoxs.length; i++) {
                        if (checkBoxs[i].type == 'checkbox') {
                            checkBoxs[i].checked = ele.checked;
                        }
                    }
                }
                else {
                    //无子节点，
                }
            }
        }

        function check(fid) //父级id (选择节点时把节点的所有父级都选上)
        {
            var ele = document.getElementById(fid);
            if (ele.type == 'checkbox') {
                var div = GetParentByTagName(ele, 'DIV');
                var checkBoxs = div.getElementsByTagName('INPUT');
                var parentCheckBoxID = div.id.replace('Nodes', 'CheckBox');
                var parentCheckBox = document.getElementById(parentCheckBoxID);
                for (var i = 0; i < checkBoxs.length; i++) {
                    if (checkBoxs[i].type == 'checkbox' && checkBoxs[i].checked) {
                        parentCheckBox.checked = true;
                        break;
                    } else {
                        parentCheckBox.checked = false;
                    }
                }

                if (parentCheckBox.checked != true) {
                    parentCheckBox.checked = false;
                }

                if (parentCheckBox.type == "checkbox") {
                    check(parentCheckBoxID);
                }
            }
        }

        function GetParentByTagName(element, tagName) {
            var parent = element.parentNode;
            var upperTagName = tagName.toUpperCase();
            while (parent && (parent.tagName.toUpperCase() != upperTagName)) {
                parent = parent.parentNode ? parent.parentNode : parent.parentElement;
            }
            return parent;
        }
    </script>
</head>
<body>
    <form id="form2" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="dd">
    </div>
    <div class="infomain">
        <div class="title">
            <span>页面权限</span>
        </div>
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info">
                <tr>
                    <td style="width: 250">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <span style="color: Black"><b>功能页面</b> </span>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left">
                                    <div style="margin-left: 20px">
                                        <asp:TreeView ID="trvPagePower" runat="server" ShowCheckBoxes="All" ImageSet="Msdn"
                                            NodeIndent="10" Width="113px" Font-Size="12px" onclick="javascript: OnTreeNodeChecked();">
                                            <LevelStyles>
                                                <asp:TreeNodeStyle Font-Size="14px" />
                                                <asp:TreeNodeStyle Font-Size="13px" />
                                                <asp:TreeNodeStyle Font-Size="12px" />
                                            </LevelStyles>
                                            <ParentNodeStyle Font-Bold="False" Font-Size="15px" />
                                            <HoverNodeStyle Font-Underline="True" />
                                            <SelectedNodeStyle BackColor="White" Font-Underline="False" />
                                            <NodeStyle Font-Names="Verdana" Font-Size="12px" ForeColor="Black" />
                                        </asp:TreeView>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="text-align: center" valign="top">
                        <table style="text-align: center" width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="text-align: right; width: 100px">
                                    权限名称:
                                </td>
                                <td style="text-align: left">
                                    <asp:TextBox ID="txtDeptName" Width="180px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right">
                                    备注说明:
                                </td>
                                <td style="text-align: left">
                                    <asp:TextBox ID="txtRemark" Width="300px" runat="server" Rows="5" TextMode="MultiLine"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td style="text-align: left">
                                    <span class="btn btn-ok-s">
                                        <asp:Button ID="btnAdd" runat="server" Text="保 存" OnClick="btnAdd_Click" /></span>
                                    <span class="btn btn-ok-s">
                                        <input type="button" id="btnGo" value="返 回" onclick="location.href=document.getElementById('Hid_GoUrl').value" />
                                    </span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <%-- <asp:HiddenField ID="hidId" runat="server" />
     <asp:HiddenField ID="Hid_GoUrl" runat="server" />--%>
     <input type="hidden" id="hidId" runat="server" />
    <input type="hidden" id="Hid_GoUrl" runat="server" />
    </form>
    
</body>
</html>

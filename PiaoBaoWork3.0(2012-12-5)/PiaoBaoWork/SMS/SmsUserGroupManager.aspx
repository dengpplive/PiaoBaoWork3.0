<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SmsUserGroupManager.aspx.cs"
    Inherits="SMS_SmsUserGroupManager" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        #txtgroupnum
        {
            height: 100px;
            width: 300px;
        }
    </style>
    <script type="text/javascript">
        function checkThis() {
            var cbAll = document.getElementById("cbAll");
            var oArray = new Array();
            oArray = document.getElementsByTagName("input");
            for (var i = 0; i < oArray.length; i++) {
                if (oArray[i].type == "checkbox" && oArray[i].id != "cbAll") {
                    if (cbAll.checked)
                        oArray[i].checked = true;
                    else
                        oArray[i].checked = false;
                }
            }
        }
        function checkItem(obj) {
            var cbAll = document.getElementById("cbAll");
            var oArray = new Array();
            var cbArray = new Array();
            oArray = document.getElementsByTagName("input");
            for (var i = 0; i < oArray.length; i++) {
                if (oArray[i].type == "checkbox" && oArray[i].id != "cbAll")
                    cbArray.push(oArray[i]);
            }

            var flag = true;
            if (obj.checked) { //当选中状态时，判断是否所有都选中；如果是，“全选”勾选框选中
                for (var i = 0; i < cbArray.length; i++) {
                    if (!cbArray[i].checked) {
                        flag = false;
                        break;
                    }
                }

                cbAll.checked = flag;
            } else {    //不是选中状态时，判断是否都选中；如果不是，“全选”勾选框曲线选中
                cbAll.checked = false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
   <div id="tabs">
    <div id="tabs-1">
     <div class="container">
              <table border="0" cellpadding="0" cellspacing="0" class="Search">
                    <tr>
                        <th>
                            姓名：
                        </th>
                        <td>
                            <asp:TextBox ID="txtName" Width="130px" CssClass="inputtxtdat" runat="server" MaxLength="16"></asp:TextBox>
                        </td>
                        <th>
                            手机号：
                        </th>
                        <td>
                            <asp:TextBox ID="txtTel" CssClass="inputtxtdat" runat="server" MaxLength="11"></asp:TextBox>
                        </td>
                        <td>
                         <span class="btn btn-ok-s">
            <asp:Button ID="Button3" runat="server" Text="查询" onclick="Button3_Click"/></span>
                        </td>
                        </tr>
                        </table>
                        </div>
          <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="0" cellpadding="0" border="0">
               <thead><tr>
                     <th>
                       <input type="checkbox" id="cbAll" onclick="checkThis()" />全选/反选
                    </th>
                    <th>
                        姓名
                    </th>
                     <th>
                        手机号码
                    </th>
                    
                </tr></thead>
                <asp:Repeater ID="Repeater" runat="server">
                    <ItemTemplate>
                      <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td  class="Operation">
                                <input runat="server" name="cbItems" id='cbItem' type="checkbox" value='<%# Eval("Tel").ToString()%>'
                                            onclick="checkItem(this);" />
                            </td>
                            <td class="pnr" >
                                <%#Eval("Name")%>
                            </td>
                            <td>
                                <%#Eval("Tel")%>
                            </td>
                            
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
              
            </table>
            <div align="center">
              <span class="btn btn-ok-s">
            <asp:Button ID="Button1" runat="server" Text="确定" onclick="Button1_Click" /></span>&nbsp;
              <span class="btn btn-ok-s">
            &nbsp;<asp:Button ID="Button2" runat="server" Text="返回" onclick="Button2_Click"/></span>
            </div>
    </div>
   </div>
    </form>
</body>
</html>

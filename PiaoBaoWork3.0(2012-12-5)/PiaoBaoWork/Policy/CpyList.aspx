<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CpyList.aspx.cs" Inherits="Policy_CpyList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
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
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
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
        <div class="title">
            <span>商家列表</span>
        </div>
        <div id="tabs-1">
         <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                width="100%">
                <thead>
                    <tr>
                       <th>
                            <input type="checkbox" id="cbAll" onclick="checkThis()" />全选/反选
                        </th>
                        <th>
                            公司名称
                        </th>
                        <th>
                            状态
                        </th>
                        <th>
                            联系人
                        </th>
                        <th>
                            联系电话
                        </th>
                        <th>
                            开户时间
                        </th>
                        
                    </tr>
                </thead>
                <asp:Repeater ID="Repeater" runat="server">
                    <ItemTemplate>
                        <tr onmouseout="this.bgColor='#ffffff';" onmouseover="this.bgColor='#F5F5F5';">
                          <td>
                          <input runat="server" name="cbItems" id='cbItem' type="checkbox" value='<%# Eval("UninAllName").ToString()+"|"+Eval("UninCode").ToString()+"|"+Eval("RoleType").ToString()%>'
                                            onclick="checkItem(this);" />
                          </td>
                            <td>
                                <%#Eval("UninAllName")%>
                            </td>
                            <td>
                                <%#Eval("AccountState").ToString() == "1" ? "正常" : "冻结"%>
                            </td>
                            <td>
                                <%#Eval("ContactUser").ToString()%>
                            </td>
                            <td>
                                <%#Eval("ContactTel")%>
                            </td>
                            <td>
                                <%#Eval("CreateTime")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
          <br />
             <div align="center">
              <span class="btn btn-ok-s">
            <asp:Button ID="btsure" runat="server" Text="确定" onclick="btsure_Click" /></span>&nbsp;
              <span class="btn btn-ok-s">
            &nbsp;<asp:Button ID="btback" runat="server" Text="返回" 
                     onclick="btback_Click"/></span>
            </div>
        </div>
        </div>
    </form>
</body>
</html>

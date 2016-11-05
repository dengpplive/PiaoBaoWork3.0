<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CacheList.aspx.cs" Inherits="Base_CacheList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>缓存管理</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .wtd
        {
            width: 200px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function ClearCache(obj, type, msg) {
            obj.disabled = true;
            $.post("CacheList.aspx",
                { ctype: type,currentuserid:$("#currentuserid").val() },
                 function (data) {
                     obj.disabled = false;
                     var arrList = data.split('@@');
                     if (arrList.length == 2) {
                         var state = arrList[0];
                         var msg1 = arrList[1];
                         if (state == "1") {
                             alert(msg + "成功");
                         } else {
                             alert(msg + "失败");
                         }
                     } else {
                         alert(msg + "失败");
                     }
                 },
            "text");
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <div id="tabs">
        <div class="title">
            <span>缓存管理</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                        border="1" style="border-collapse: collapse;">
                        <tr>
                            <th class="wtd">
                                <h4>
                                    缓存名称</h4>
                            </th>
                            <th>
                                <h4>
                                    操作</h4>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <h5 style="color: Red;">
                                    票宝缓存服务
                                </h5>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                清空所有缓存
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,8,'清空所有缓存清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                承运人管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,2,'承运人管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                舱位管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,1,'舱位管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                机场管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,5,'机场管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                票价管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,3,'票价管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                燃油管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,4,'燃油管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                机建管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,0,'机建管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                退改签管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,6,'退改签管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                政策管理
                            </td>
                            <td>
                                <a href="#" onclick="return ClearCache(this,7,'政策管理清空')">清空</a>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <h5 style="color: Red;">
                                    IIS 缓存</h5>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                清空所有缓存
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtnBd_Base_Dictionary" runat="server" OnClick="lbtnBd_Base_Dictionary_Click">清空</asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>

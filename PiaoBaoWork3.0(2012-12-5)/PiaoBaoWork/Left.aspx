<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Left.aspx.cs" Inherits="Left" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <title></title>
    <link href="css/Left.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript">
        $(function () {
            var icons = {
                header: "ui-icon-circle-arrow-e",
                headerSelected: "ui-icon-circle-arrow-s"
            };
            $("#accordion").accordion({
                icons: icons,
                autoHeight: false
            }).click(function () {
                //获取 
                var collapsible = $('#accordion').accordion('option', 'collapsible');
                collapsible = !collapsible;
                //设置 
                $('#accordion').accordion('option', 'collapsible', collapsible);
            });
        });
        function LeftToRight(fid) {
            var defaultFrame = window.parent.document.getElementById('mainFrame');
            var cols = defaultFrame.cols;
            if (fid == "btnRight") {
                defaultFrame.cols = "7,*";
                document.getElementById('td1').style.display = "block";
                document.getElementById('td2').style.display = "none";
                window.parent.frames["topFrame"].document.all['StoryState'].value = "1";
            } else if (fid == "btnLeft") {
                var defaultFrame = window.parent.document.getElementById('mainFrame');
                defaultFrame.cols = "187,*";
                window.parent.frames["topFrame"].document.all['StoryState'].value = "0";
                document.getElementById('td1').style.display = "none";
                document.getElementById('td2').style.display = "block";
            }
        }
        function MouseStyle(td, f) {
            if (f == "over") {
                td.style.backgroundColor = "#ffffff";
            } else if (f == "out") {
                td.style.backgroundColor = "#ffffff";
            }
        }
        function SetState(fg) {
            if (fg == "1") {
                LeftToRight("btnRight");
            } else if (fg == "0") {
                LeftToRight("btnLeft");
            }
        }
    </script>
</head>
<body  style="width: 180px; background:url(img/left/Leftbg2.gif) repeat-y;">
    <form id="form1" runat="server">
    <asp:HiddenField ID="currentuserid" runat="server" />
    <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td valign="middle" id="td1" onmouseover="MouseStyle(this,'over')" onmouseout="MouseStyle(this,'out')"
                style="display: none;">
                <span id="btnLeft" onclick="LeftToRight('btnLeft')" title="展开菜单">
                <img alt="" src="img/left/open.gif" style=" cursor:pointer;" />
                    </span>
            </td>
            <td>
                <table border="0" cellpadding="0" cellspacing="0" width="180px" align="center" height="100%">
                    <tr>
                        <td style="background: url(img/left/Leftbg4.gif) repeat-y scroll 0 0 transparent;
                            height: 6px; width: 180px;">
                        </td>
                        <td style="background-color: #c3dcf2;">
                        </td>
                    </tr>
                    <tr>
                        <td style="background: url(img/left/Leftbg2.gif) repeat-y scroll 0 0 transparent;
                            height: 100%; width: 176px; padding: 0 1px 0 3px;">
                            <div class="demo">
                                <!--权限管理，管理员权限-->
                                <div id="accordion" runat="server">
                                </div>
                                <asp:Literal ID="ltl_QQ" runat="server"></asp:Literal>
                            </div>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </td>
            <td  valign="middle" id="td2" onmouseover="MouseStyle(this,'over')" onmouseout="MouseStyle(this,'out')">
                <span id="btnRight" onclick="LeftToRight('btnRight')" title="收缩菜单">
                <img alt="" src="img/left/close.gif" style=" cursor:pointer;" /></span>
            </td>
        </tr>
    </table>
    
    </form>
</body>
</html>

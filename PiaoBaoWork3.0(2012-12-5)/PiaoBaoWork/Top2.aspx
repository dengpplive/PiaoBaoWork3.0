<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Top2.aspx.cs" Inherits="Top2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body
        {
            color: #333333;
            font: 12px/1.5 Arial,Tahoma,sans-serif;
            margin: 0;
            padding: 0;
        }
        .main_top_bg
        {
            height: 30px;
            line-height: 30px;
            width: 100%;
            overflow: hidden;
            background: url(img/top/top2bg.gif) repeat-x;
        }
        .gg_red
        {
            float: left;
            margin-left: 5px;
        }
        .gg
        {
            float: left;
            margin-right: 5px;
            background: url(img/top/gongg.gif) no-repeat scroll 0 10px;
            padding-left: 20px;
        }
        span
        {
            height: 30px;
            line-height: 30px;
            margin: 0;
            padding: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main_top_bg">
        <div class="gg_red">
            亲爱的&nbsp;<span style="color: #f60;"><%=userName %></span>&nbsp;您好!
            <%--    &nbsp; <span id="lblShowLast">您上次登录时间是：</span><span style="color: #f60"><%=lastTime%></span>&nbsp;IP：<span style="color: #f60"><%=lastIP%></span>&nbsp;--%>
            最新公告</div>
        <div class="gg" style="width: 65%">
            <marquee scrollamount="2" onmouseout="start()" onmouseover="stop()"><div runat="server" id="divGG" style=" font-size:12px"></div></marquee>
        </div>
    </div>
    </form>
</body>
</html>

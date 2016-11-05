<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <meta name="CopyRight" content="版权所有：采购通科技" />
    <meta name="Web Design" content="采购通科技" />
    <link rel="shortcut icon" href="favicon.ico" />
    <title>
        <%=Title%>&nbsp;&nbsp;（<%=System.Configuration.ConfigurationManager.ConnectionStrings["Version"].ConnectionString %>）
    </title>   
</head>
<frameset rows="105,*,40" cols="*" framespacing="0" frameborder="no" border="0">
  <frame src="<%=TopUrl %>" name="topFrame" scrolling="No" noresize="noresize" id="topFrame"  />
  <frameset id="mainFrame" rows="*" cols="187,*"  frameborder="0" framespacing="0" name="mainFrame" border="0">
  <frame scrolling="no" noresize="" frameborder="no" src="Left.aspx?currentuserid=<%=sessionuserid %>" marginheight="0" marginwidth="0" name="LeftFrame" id="LeftFrame" height="100%" ></frame>
   <frameset  rows="30,*" cols="*" framespacing="0" frameborder="no" border="0">
  <frame src="Top2.aspx?currentuserid=<%=sessionuserid %>" name="topFrame2" scrolling="No" noresize="noresize" id="topFrame2"  />
   <frame scrolling="yes" src="<%=DefaultUrl %>" name="ALLFrame" id="ALLFrame" rows="*" cols="*" /> 
  </frameset>

  </frameset>  
  <frame src="Bottom.aspx?currentuserid=<%=sessionuserid %>" name="bottomF" id="bottomF" scrolling="No" noresize="noresize" />
</frameset>
<noframes>
    <body>
        对不起，您的浏览器不支持框架，请升级或更换浏览器！
    </body>
</noframes>
</html>

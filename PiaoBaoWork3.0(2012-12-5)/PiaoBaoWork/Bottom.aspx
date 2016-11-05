<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Bottom.aspx.cs" Inherits="Bottom" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link type="text/css" href="css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="js/js_Browser.js"></script>
    <style type="text/css">
        .Popup
        {
            border: 2px outset #ffffff;
        }
        .promptBody
        {
            width: 300px;
            height: 300px;
            border: 1px solid green;
            background-color: Red;
        }
    </style>
    <script type="text/javascript">
        function ShowPrompt() {
            var CpyNo=$.trim($("#Hid_CpyNo").val());
            //登录用户角色
            var RoleType=$.trim($("#Hid_RoleType").val());
            var url="AJAX/GetHandler.ashx";
            var param=
            {
                CpyNo: escape(CpyNo),
                RoleType: escape(RoleType),
                OpName: escape("Prompt"),
                currentuserid: "<%=this.mUser.id.ToString() %>",
                num: Math.random()
            };
            $.post(url,param,function (data) {
                if(data!="") {
                    data=unescape(data);
                    var dataArr=data.split('#######');
                    if(dataArr.length>1&&dataArr[1]!="") {
                        //显示
                        ShowPopup(dataArr[1]);
                    }
                }
                //调用提醒
                CallPrompt(1);
            },"text");
        }

        //显示提醒框
        function ShowPopup(data) {
            //提示声音
            data+="<embed src=\"wav/msg.wav\" autostart=\"true\" loop=\"false\" hidden=\"true\" volume=\"100\" />";
            var win=window.parent;
            //浏览器名称
            var browserName=getBrowser();
            var d,dwin;
            if(browserName.Moz) {
                dwin=win.document.getElementById("ALLFrame");
                d=win.document.getElementById("ALLFrame").contentDocument;
            } else {
                dwin=win.document.frames["ALLFrame"]||win.document.getElementById("ALLFrame");
                d=win.document.frames["ALLFrame"].document||win.document.getElementById("ALLFrame").contentDocument;
            }
            //页面加载完毕显示提醒框
            $(function () {
                try {
                    var div=d.getElementById("Promptdiv");
                    if(div==null) {
                        div=d.createElement("div");
                        div.id="Promptdiv";
                        div.style.width="100%";
                        div.style.height="100%";
                        div.style.position="fixed";
                        div.style.right="15px";
                        div.style.bottom="5px";
                        $(div).html(data);
                        if(!window.opera||browserName.Moz) {
                            d.body.appendChild(div);
                        }
                        else {
                            d.insertBefore(div,d.lastChild);
                        }
                    } else {
                        $(div).html(data);
                    }
                    $(div).show();
                    //关闭
                    var span_Close=div.getElementsByTagName("span")[1];
                    span_Close.onclick=function () {
                        $(div).hide();
                    }


                    //窗口发生变化时
                    dwin.onscroll=dwin.onload=dwin.onresize=function () {
                        div.style.width="100%";
                        div.style.height="100%";
                        div.style.position="fixed";
                        div.style.right="15px";
                        div.style.bottom="5px";
                    }
                } catch(e) {
                    //alert(e.Message);
                }
            },dwin);
        }
        var winTime=null;
        var IsOpen=null;
        var PromptTime=null;
        function CallPrompt(fg) {
            //登录用户角色
            var RoleType=$.trim($("#Hid_RoleType").val());
            if(RoleType=="2") {
                //是否开启订单提醒
                IsOpen=$.trim($("#IsPrompt").val());
                //弹出时间间隔单位秒    
                PromptTime=$.trim($("#PromptTime").val());
                PromptTime=PromptTime==""?"15":PromptTime;
                if(fg==0) {
                    PromptTime=1;
                }
                if(IsOpen=="1") {
                    //调用这个                  
                    winTime=setTimeout('ShowPrompt()',PromptTime*1000);
                } else {
                    if(winTime!=null) {
                        clearTimeout(winTime);
                    }
                }
            }
        }
        //加载
        $(function () {
            CallPrompt(0);
        });  
    </script>
</head>
<body style="overflow: hidden; margin: 0px; height: 47px;">
    <form id="Form1" runat="server">
    <table border="0" cellpadding="0" cellspacing="0" style="background: #e1ebf7; width: 100%;
        height: 25px;">
        <tr>
            <td colspan="3" height="25" align="center">
                <span style="color: #1D8FCD; font-size: 12px;">COPYRIGHT &copy; 2013 版权所有 <a href="http://www.miibeian.gov.cn/icp/publish/query/icpMemoInfo_login.action?id=23005842">
                </a>
                    <br />
                    本站服务器托管由北京数据家提供<br />
                    www.inidc.com.cn <a href="http://www.miibeian.gov.cn/icp/publish/query/icpMemoInfo_login.action?id=23005842">
                    </a></span>
            </td>
        </tr>
    </table>
    <%-- <input id="btnTest" type="button" value="测试" onclick="ShowPrompt()" />--%>
    <div id="Promptdiv" style="display: none;">
    </div>
    <%--是否开启订单提醒--%>
    <input type="hidden" id="IsPrompt" runat="server" />
    <%--登录用户角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" />
    <%--公司编号--%>
    <input type="hidden" id="Hid_CpyNo" runat="server" />
    <%--订单提醒时间间隔--%>
    <input type="hidden" id="PromptTime" runat="server" value="15" />
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Top.aspx.cs" Inherits="Top" %>

<html>
<head>
    <title id="title"></title>
    <link href="css/Top.css" rel="stylesheet" type="text/css" />
    <link type="text/css" href="css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="js/jquery-ui-1.8.13.custom.min.js"></script>
    <script src="js/js_WinOpen.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //点击事件
            $("div").click(function () {
                if($(this)[0].id.indexOf('menu')>=0) {
                    var l=$("div");
                    for(var i=0;i<l.length;i++) {
                        if(l[i].id.indexOf('menu')>=0) {
                            l[i].className="menuno";
                        }
                    }
                    $(this)[0].className="menuselect";
                }
            });
            var url='<%=DefaultUrl %>';
            if(url!="1") {
                GoToUrl(url,'0');
            }
        });


        function showmenu(str,str2) {
            var strlist=str.split(',');
            var str2list=str2.split(',');
            for(var i=0;i<strlist.length;i++) {
                if(i==0) {
                    document.getElementById("menu"+strlist[i]).className="menuselect";
                }
                document.getElementById("menu"+strlist[i]).style.display="";
            }
        }
        function GoToUrl(url,type) {
            try {

                if(type=='chakankegui') {
                    window.open(url,'chakankegui','height=600, width=800,toolbar=yes, menubar=yes, scrollbars=yes,resizable=yes,location=yes, status=yes');
                } else if(type=='gongao') {
                    window.open(url,'gongao','height=500, width=700, top=100, left=150, toolbar=no, menubar=no, scrollbars=no,resizable=no,location=no, status=no');
                }
                else if(type=='PlatformDocument') {
                    window.open(url,'PlatformDocument','height=600, width=800,toolbar=yes, menubar=yes, scrollbars=yes,resizable=yes,location=yes, status=yes');
                }
                else if(type=='0') {
                    window.parent.window.ALLFrame.location=url;
                }
                else {
                    var v=document.getElementById("StoryState").value;
                    window.parent.window.LeftFrame.location="Left.aspx?type="+type+"&v="+v+"&currentuserid=<%=this.mUser.id.ToString() %>";
                    window.parent.window.ALLFrame.location=url;
                }
            }
            catch(e) {

            }
        }

        function ShowModuleIndex() {

            try {
                var moduleIndex=document.getElementById("hid_ShowModuleIndex").value;

                if(moduleIndex!=null&&moduleIndex!=undefined) {

                    var strs=moduleIndex.split(',');

                    for(var i=0;i<strs.length;i++) {
                        if(strs[i]=="1") {
                            document.getElementById("menu1").style.display="block";
                        }
                        else if(strs[i]=="8") {
                            document.getElementById("menu8").style.display="block";
                        }
                        else if(strs[i]=="13") {
                            document.getElementById("menu13").style.display="block";
                        }
                        else if(strs[i]=="14") {
                            document.getElementById("menu14").style.display="block";
                        }
                        else if(strs[i]=="15") {
                            document.getElementById("menu15").style.display="block";
                        }
                        else if(strs[i]=="16") {
                            document.getElementById("menu16").style.display="block";
                        }
                    }
                }

                //根据角色类型显示隐藏
                var hidRoleType=document.getElementById("hid_RoleType").value;
                //                "menu1"  机 票
                //                "menu8"  短 信
                //                "menu13" 系 统
                //                "menu14" 公 司
                //                "menu15" 客 户
                //                "menu16" 财 务

                if(hidRoleType=="1") { }
                else if(hidRoleType=="2") { }
                else if(hidRoleType=="3") { document.getElementById("menu15").style.display="none"; }
                else if(hidRoleType=="4") { }
                else if(hidRoleType=="5") { document.getElementById("menu15").style.display="none"; }

            } catch(e) {

            }
        }
        $(function () {
            $("#AccountMoneyRef").click(function () {
                $.get('AJAX/AccountHandler.ashx',{ i: Math.random(),currentuserid: '<%=this.mUser.id.ToString() %>' },function (data) {
                    $("#AccountMoney").text(data);
                })
            })
            $("#AccountMoneyRef").click();
        })
        //此方法用于与服务器保持通讯
        //用于实现一个用户账号不能多个人登录
        //勿做过多的操行,尤其不能有数据交互,因为调用频繁,避免过多的消耗系统资源
        var isOne=false;
        function keepStatus() {
            if(!isOne) {
                $.get('AJAX/keepStatus.ashx',
             { i: Math.random(),currentuserid: '<%=this.mUser.id.ToString() %>' },
             function (data) {
                 if(data=="0") {
                     isOne=true;
                     alert("您的账号在另一地点登录,您被迫下线,如有疑问,请联系管理员");
                     location.href="Login.aspx";
                 }
                 if(data=="2") {
                     isOne=true;
                     alert("系统校验码过期,您被迫下线,如有疑问,请联系管理员");
                     location.href="Login.aspx";
                 }
             }
            )
            }
        }
        //setInterval
        // setInterval(keepStatus, 10000);
    </script>
</head>
<body onload="ShowModuleIndex()">
    <form id="Form1" runat="server">
    <div class="topcss">
        <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; overflow: hidden;
            height: 105px; border: none;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="height: 76px;">
                        <tr>
                            <td style="width: 136px; height: 76px; vertical-align: middle; line-height: 76px;"
                                runat="server" id="logimg">
                                <%-- <img src="images/logto.png" />--%>
                            </td>
                            <td class="wordtopmenu">
                                <asp:Label ID="lblCompayName" runat="server" Style="color: #424242; font-size: 18px;
                                    font-family: 微软雅黑,tahoma,arial,sans-serif;" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="admin_txt" style="text-align: right; height: 76px; padding-right: 10px;">
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; overflow: hidden;
                        text-align: right;">
                        <tr style="height: 25px; line-height: 25px;">
                            <td>
                                <ul style="width: 700px; position: absolute; right: 30px; top: 10px; height: 92px;">
                                    <li>
                                        <asp:LinkButton runat="server" ID="lbtnOut" Text="退出" OnClientClick="return confirm('确定要退出吗？');"
                                            OnClick="lbtnOut_Click" CssClass="outa"></asp:LinkButton></li>
                                    <li><a href="#" onclick="GoToUrl('UpdatePwd.aspx?currentuserid=<%=this.mUser.id.ToString() %>','0');return false;"
                                        class="outa">密码修改</a> </li>
                                    <li><a href="#" onclick="GoToUrl('User/EmployeesEdit.aspx?currentuserid=<%=this.mUser.id.ToString() %>','0');return false;"
                                        class="outa">个人信息</a></li>
                                    <li runat="server" id="lilbntIsShow" visible="true">
                                        <asp:LinkButton runat="server" ID="lbntIsShow" Text="隐" CssClass="outa" OnClick="lbntIsShow_Click"></asp:LinkButton></li>
                                    <li><a href="#" onclick="GoToUrl('Manager/Sys/NewBulletinList.aspx?currentuserid=<%=this.mUser.id.ToString() %>','gongao');return false;"
                                        class="outa">查看公告</a></li>
                                    <li runat="server" id="lbtnKeGuiS"><a href="#" onclick="GoToUrl('http://kegui.jptonghang.com/dm/Prescribe/guiding.html?Airways=','chakankegui');return false;"
                                        class="outa">查看客规</a></li>
                                    <li runat="server" id="lblIBEsel" visible="false"><a href="#" onclick="GoToUrl('Manager/Base/IBEsel.aspx?currentuserid=<%=this.mUser.id.ToString() %>','0');return false;"
                                        class="outa">IBE查询</a></li>
                                    <li runat="server" id="lblPTword" visible="false"><a href="#" onclick="GoToUrl('PlatformDocument.htm','PlatformDocument');return false;"
                                        class="outa">平台文档</a></li>
                                    <li runat="server" id="lblClearHF" visible="false"><a href="#" onclick="GoToUrl('Manager/Base/CacheList.aspx?currentuserid=<%=this.mUser.id.ToString() %>','0');return false;"
                                        class="outa">清理缓存</a></li>
                                    <%--  <li runat="server" id="lblClearHF1"><a href="#" onclick="GoToUrl('Policy/PolicyQuery.aspx?PageType=1&currentuserid=<%=this.mUser.id.ToString() %>','0');return false;"
                                        class="outa">1111111111111</a></li>
                                    --%>
                                    <li><a href="#" onclick="GoToUrl('index.aspx?currentuserid=<%=this.mUser.id.ToString() %>','0');return false;"
                                        class="outa">首页</a></li>
                                    <li runat="server" id="lblOldVersion" visible="true">
                                        <asp:LinkButton runat="server" ID="lbntOld" Text="旧版本" CssClass="outa" OnClick="lbntOld_Click"></asp:LinkButton></li>
                                </ul>
                            </td>
                        </tr>
                        <tr style="height: 25px; line-height: 25px;">
                            <td>
                                <div style="font-size: 12px; width: 400px; text-align: right; color: #FBFC72; position: absolute;
                                    right: 10px; top: 40px">
                                    <asp:Label runat="server" ID="lblShow"></asp:Label><br />
                                    <asp:Label ID="lbl_contact" runat="server"></asp:Label>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="width: 100%;">
                <td colspan="2" style="width: 100%;">
                    <div class="menu">
                        <div id="menu1" class="menuselect" style="display: none;">
                            <a href="#" onclick="GoToUrl('Buy/List.aspx?currentuserid=<%=mUser.id.ToString() %>','1');return false;">
                                机 票</a>
                        </div>
                        <div id="menu16" class="menuno" style="display: none;">
                            <a href="#" onclick="GoToUrl('Bill/BillOfCount.aspx?currentuserid=<%=mUser.id.ToString() %>','16');return false;">
                                财 务</a></div>
                        <div id="menu15" class="menuno" style="display: none;">
                            <a href="#" onclick="GoToUrl('User/ComPanyList.aspx?currentuserid=<%=mUser.id.ToString() %>','15');return false;">
                                客 户</a></div>
                        <div id="menu14" class="menuno" style="display: none;">
                            <a href="#" onclick="GoToUrl('User/EmployeesEdit.aspx?currentuserid=<%=mUser.id.ToString() %>','14');return false;">
                                公 司</a></div>
                        <div id="menu13" class="menuno" style="display: none;">
                            <a href="#" onclick="GoToUrl('Manager/Base/BaseDictionaryList.aspx?currentuserid=<%=mUser.id.ToString() %>','13');return false;">
                                系 统</a></div>
                        <div id="menu8" class="menuno" style="display: none;">
                            <a href="#" onclick="GoToUrl('SMS/SmsSend.aspx?currentuserid=<%=mUser.id.ToString() %>','8');return false;">
                                短 信</a></div>
                    </div>
                </td>
            </tr>
        </table>
        <input type="hidden" id="StoryState" value="0" />
        <input type="hidden" id="hid_RoleType" value="0" runat="server" />
        <input type="hidden" id="hid_ShowModuleIndex" value="0" runat="server" />
    </div>
    </form>
</body>
</html>

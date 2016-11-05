<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>航空商旅综合业务交易系统</title>
    <link href="css/login.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" href="favicon.ico" />
    <style type="text/css">
        .clearfix:after
        {
            content: ".";
            display: block;
            height: 0;
            clear: both;
            visibility: hidden;
        }
        .clearfix
        {
            zoom: 1;
        }
        ul, li
        {
            list-style: none;
        }
        img
        {
            border: 0;
        }
        
        
        h1
        {
            height: 50px;
            line-height: 50px;
            font-size: 22px;
            font-weight: normal;
            font-family: "Microsoft YaHei" ,SimHei;
        }
        
        .shuoming
        {
            margin-top: 20px;
            border: 1px solid #ccc;
            padding-bottom: 10px;
        }
        .shuoming dt
        {
            height: 30px;
            line-height: 30px;
            font-weight: bold;
            text-indent: 10px;
        }
        .shuoming dd
        {
            line-height: 20px;
            padding: 5px 20px;
        }
        
        /* qqshop focus */
        #focus
        {
            width: 620px;
            height: 244px;
            overflow: hidden;
            position: relative;
        }
        #focus ul
        {
            height: 380px;
            position: absolute;
        }
        #focus ul li
        {
            float: left;
            width: 620px;
            height: 244px;
            overflow: hidden;
            position: relative;
            background: #000;
        }
        #focus ul li div
        {
            position: absolute;
            overflow: hidden;
        }
        #focus .btnBg
        {
            position: absolute;
            width: 620px;
            height: 20px;
            left: 0;
            bottom: 0;
            background: #000;
        }
        #focus .btn
        {
            position: absolute;
            width: 600px;
            height: 10px;
            padding: 5px 10px;
            right: 0;
            bottom: 0;
            text-align: right;
        }
        #focus .btn span
        {
            display: inline-block;
            _display: inline;
            _zoom: 1;
            width: 25px;
            height: 10px;
            _font-size: 0;
            margin-left: 5px;
            cursor: pointer;
            background: #fff;
        }
        #focus .btn span.on
        {
            background: #fff;
        }
        #focus .preNext
        {
            width: 45px;
            height: 100px;
            position: absolute;
            top: 90px;
            background: url(img/sprite.png) no-repeat 0 0;
            cursor: pointer;
        }
        #focus .pre
        {
            left: 0;
        }
        #focus .next
        {
            right: 0;
            background-position: right top;
        }
    </style>
    <script src="js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="js/dialog1/JqueryMask.js" type="text/javascript"></script>
    <link href="css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        function setImage() {
            $("#img1").attr("src", "CheckCode.aspx?num=" + Math.random() * 1000);
            if (window.self != window.top) {
                top.href = 'Login.aspx';
            }
        }
        //对话框包含处理
        function showdialog(t, f) {
            if (t.indexOf('验证码错误') != -1) {
                setImage();
            }
            /*
            var div=document.getElementById("divDG");
            if(div==null) {
            div=document.createElement("div");
            div.id="divDG";
            if(document.all) {
            document.body.appendChild(div);
            }
            else {
            document.insertBefore(div,document.body);
            }
            }*/
            $("select").hide();
            $("#divDG").html(t);
            $("#divDG").dialog("open");
        }
        function IsOpen() {
            if (window.parent.frames.length > 0) {
                top.location = 'Login.aspx';
            }
        }
        //-----------------------------------------------------------------
        $(function () {
            $("#divDG").dialog({
                title: '提示',
                bgiframe: true,
                height: 180,
                autoOpen: false,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    $("select").show();
                },
                buttons: {
                    '确定': function () {
                        $(this).dialog('close');
                    }
                }
            });
            setImage();
            $("#txtUserName").focus();
            $(document).keydown(function (event) {
                if (event.keyCode == 13) {
                    if ($("#divDG").dialog("isOpen") == true) {
                        $("#divDG").dialog("close");
                    } else {
                        do_login();
                    }
                }
            });
        });
        function do_login() {
            var LoginUserName = $.trim($("#txtUserName").val());
            var Pwd = $.trim($("#txtPwd").val());
            var CheckCode = $.trim($("#txtCheckCode").val());
            if (LoginUserName == "") {
                showdialog('登录账号不能为空！');
                $("#txtUserName").focus();
            } else if (Pwd == "") {
                showdialog('登录密码不能为空！');
                $("#txtPwd").focus();
            } else if (CheckCode == "") {
                showdialog('输入验证码不能为空！');
                $("#txtCheckCode").focus();
            } else {
                $("#btnLogin").click();
            }
        }
        //-----------------------------------------------------------------
    </script>
    <script type="text/javascript">
        $(function () {
            var sWidth = $("#focus").width(); //获取焦点图的宽度（显示面积）
            var len = $("#focus ul li").length; //获取焦点图个数
            var index = 0;
            var picTimer;

            //以下代码添加数字按钮和按钮后的半透明条，还有上一页、下一页两个按钮
            var btn = "<div class='btnBg'></div><div class='btn'>";
            for (var i = 0; i < len; i++) {
                btn += "<span></span>";
            }
            btn += "</div><div class='preNext pre'></div><div class='preNext next'></div>";
            $("#focus").append(btn);
            $("#focus .btnBg").css("opacity", 0.5);

            //为小按钮添加鼠标滑入事件，以显示相应的内容
            $("#focus .btn span").css("opacity", 0.4).mouseenter(function () {
                index = $("#focus .btn span").index(this);
                showPics(index);
            }).eq(0).trigger("mouseenter");

            //上一页、下一页按钮透明度处理
            $("#focus .preNext").css("opacity", 0.2).hover(function () {
                $(this).stop(true, false).animate({ "opacity": "0.5" }, 300);
            }, function () {
                $(this).stop(true, false).animate({ "opacity": "0.2" }, 300);
            });

            //上一页按钮
            $("#focus .pre").click(function () {
                index -= 1;
                if (index == -1) { index = len - 1; }
                showPics(index);
            });

            //下一页按钮
            $("#focus .next").click(function () {
                index += 1;
                if (index == len) { index = 0; }
                showPics(index);
            });

            //本例为左右滚动，即所有li元素都是在同一排向左浮动，所以这里需要计算出外围ul元素的宽度
            $("#focus ul").css("width", sWidth * (len));

            //鼠标滑上焦点图时停止自动播放，滑出时开始自动播放
            $("#focus").hover(function () {
                clearInterval(picTimer);
            }, function () {
                picTimer = setInterval(function () {
                    showPics(index);
                    index++;
                    if (index == len) { index = 0; }
                }, 3000); //此4000代表自动播放的间隔，单位：毫秒
            }).trigger("mouseleave");

            //显示图片函数，根据接收的index值显示相应的内容
            function showPics(index) { //普通切换
                var nowLeft = -index * sWidth; //根据index值计算ul元素的left值
                $("#focus ul").stop(true, false).animate({ "left": nowLeft }, 300); //通过animate()调整ul元素滚动到计算出的position
                //$("#focus .btn span").removeClass("on").eq(index).addClass("on"); //为当前的按钮切换到选中的效果
                $("#focus .btn span").stop(true, false).animate({ "opacity": "0.4" }, 300).eq(index).stop(true, false).animate({ "opacity": "1" }, 300); //为当前的按钮切换到选中的效果
            }
        });

    </script>
</head>
<body>
    <div id="divDG">
    </div>
    <form id="form1" runat="server">
    <div class="page">
        <div runat="server" id="divLog" class="top">
            <div class="loginDisplay">
            </div>
            <div class="clear hideSkiplink">
            </div>
            <div class="en_bg">
            </div>
        </div>
        <div id="cont">
            <div class="left">
               <div class="fla">
                    <div id="flash">
                        <div runat="server" id="divswf" style="width: 620px; height: 244px;">
                           
                        </div>
                    </div>
                </div>
                <div class="indexContent">
                    <div class="indexContentl">
                    </div>
                    <div class="indexContentc">
                        <ul class="indexContentctxt">
                            <li class="top1">价格透明</li>
                            <li>所有机票价格均来自航空公司公布的运价，保证准确透明</li>
                            <li>票据管理规范透明，无需人工干预</li>
                        </ul>
                        <ul class="indexContentctxt">
                            <li class="top2">功能强大</li>
                            <li>机票酒店实时预订</li>
                            <li>数十项报表数据</li>
                            <li>自动生成分析报告</li>
                            <li>汇集除机票、酒店等常有的差旅服务外，还有租车、汽车票、火车票等多项增值服务</li>
                        </ul>
                        <ul class="indexContentctxt">
                            <li class="top3">优质服务商</li>
                            <li>提供优秀的服务公司</li>
                            <li>通过快乐行为客户提供标准化服务</li>
                            <li>有机会享受超低集中采购价格</li>
                        </ul>
                        <ul class="indexContentctxt last">
                            <li class="top4">省钱高效</li>
                            <li>自动化管理流程，省去人工和时间成本</li>
                            <li>最低票价提醒功能，合理节约采购成本</li>
                            <li>预订、审批、对账、结算一站式完成</li>
                        </ul>
                    </div>
                    <div class="indexContentr">
                    </div>
                </div>
            </div>
            <div class="right">
                <div class="login-box" runat="server" id="divleftbox">
                    <div class="login1">
                        <div class="inputStyle1">
                            <input type="text" tabindex="1" maxlength="32" value="" clientidmode="Static" id="txtUserName"
                                name="txtUserName" runat="server" />
                        </div>
                        <div class="inputStyle1">
                            <asp:TextBox TextMode="Password" runat="server" ID="txtPwd" TabIndex="2" MaxLength="20"></asp:TextBox>
                        </div>
                        <div class="yzm">
                            <div class="inputStyle1 inputStyle2">
                                <asp:TextBox ID="txtCheckCode" TabIndex="3" SkinID="txtCheckCode" ClientIDMode="Static"
                                    MaxLength="5" runat="server" CssClass="input" />
                            </div>
                            <img src="CheckCode.aspx" id="img1" runat="server" onclick="this.src='CheckCode.aspx?abc='+Math.random()"
                                alt="图片看不清？点击重新得到验证码" style="cursor: hand" />
                        </div>
                        <a href="#" onclick="document.getElementById('img1').src='CheckCode.aspx?abc='+Math.random()"
                            title="图片看不清？点击重新得到验证码">图片看不清？点击重新得到验证码</a>
                        <br />
                        <div class="safe" style="display: none">
                            <span id="J_LongLogin_l1" class="long-login" style="visibility: visible;">
                                <input type="checkbox" class="J_LognLoginInput" id="J_LongLogin_1" onclick="" runat="server" />
                                <label for="J_LongLogin_1">
                                    两周内免登录</label>
                            </span>
                            <p class="pin">
                                (公用电脑，请不要勾选此项)</p>
                        </div>
                        <div class="submit">
                            <button tabindex="3" class="J_Submit" type="button" onclick="do_login()">
                            </button>
                            &nbsp;&nbsp;<asp:CheckBox runat="server" ID="chkCook" Text="记住密码" ForeColor="White" />
                        </div>
                        <div class="dn">
                            <asp:Button ID="btnLogin" runat="server" Text="Button" ClientIDMode="Static" OnClick="btnLogin_Click" /></div>
                    </div>
                </div>
            </div>
        </div>
        <div id="foot">
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
        </div>
        <div class="clear">
        </div>
    </div>
    </form>
</body>
</html>

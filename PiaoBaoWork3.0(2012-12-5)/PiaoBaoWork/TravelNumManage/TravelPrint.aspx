<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TravelPrint.aspx.cs" Inherits="TravelNumManage_TravelPrint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <style type="text/css">
        .spanweb
        {
            border: 0px white solid;
        }
        span
        {
            border: 1px #DDE3EC solid;
            height: 21px;
        }
        .fontSize
        {
            font-size: 12px;
        }
    </style>
    <style type="text/css">
        @media Print
        {
            .notprint
            {
                display: none;
            }
            input
            {
                border-color: White;
            }
            span
            {
                border: 0px white solid;
            }
            #tainer
            {
                /*  background-image: url(../Images/ReceipePrint.bmp);*/
            }
        }
        @media Screen
        {
            .notprint
            {
                display: inline;
                cursor: move;
            }
            .tableTest
            {
                position: absolute;
            }
            .border
            {
                border: 1px #DDE3EC solid;
            }
            #tainer
            {
                /*background-image: url(../Images/ReceipePrint.bmp);*/
            }
        }
        .style1
        {
            color: #FF0000;
        }
    </style>
    <script type="text/javascript">
        var HKEY_Root,HKEY_Path,HKEY_Key;
        HKEY_Root="HKEY_CURRENT_USER";
        HKEY_Path="\\Software\\Microsoft\\Internet Explorer\\PageSetup\\";
        //设置网页打印的页眉页脚为空
        function PageSetup_Null() {
            try {
                var Wsh=new ActiveXObject("WScript.Shell");
                //--这个ActiveXObject火狐不支持，直接报错。
                //FireFox本身不支持ActiveX控件，当然无法使用ActiveXObject了。
                HKEY_Key="header";
                Wsh.RegWrite(HKEY_Root+HKEY_Path+HKEY_Key,"");
                HKEY_Key="footer";
                Wsh.RegWrite(HKEY_Root+HKEY_Path+HKEY_Key,"");
            }
            catch(e)
            { }
        }
        //设置网页打印的页眉页脚为默认值
        function PageSetup_Default() {
            try {
                var Wsh=new ActiveXObject("WScript.Shell");
                HKEY_Key="header";
                Wsh.RegWrite(HKEY_Root+HKEY_Path+HKEY_Key,"&w&b页码,&p/&P");//&w&b页码,&p/&P
                HKEY_Key="footer";
                Wsh.RegWrite(HKEY_Root+HKEY_Path+HKEY_Key,"&u&b&d");//&u&b&d
            }
            catch(e)
            { }
        }
        PageSetup_Null();
        //PageSetup_Default();
    </script>
    <script language="VBScript" type="text/vbscript">
    dim hkey_root,hkey_path,hkey_key
    hkey_root="HKEY_CURRENT_USER"
    hkey_path="\Software\Microsoft\Internet Explorer\PageSetup"

    '//设置网页打印的页眉页脚为空
    function pagesetup_null()
        on error resume next
        Set RegWsh = CreateObject("WScript.Shell")
        '--这个CreateObject火狐、IE都支持，但是火狐下还是清除不掉。
        hkey_key="\header"
        RegWsh.RegWrite hkey_root+hkey_path+hkey_key,""
        hkey_key="\footer"
        RegWsh.RegWrite hkey_root+hkey_path+hkey_key,""
    end function

    '//设置网页打印的页眉页脚为默认值
    function pagesetup_default()
    on error resume next
        Set RegWsh = CreateObject("WScript.Shell")
        hkey_key="\header"
        RegWsh.RegWrite hkey_root+hkey_path+hkey_key,"&w&b页码，&p/&P"
        hkey_key="\footer"
        RegWsh.RegWrite hkey_root+hkey_path+hkey_key,"&u&b&d"
    end function

   call pagesetup_null()
   'call pagesetup_default()
    </script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../JS/Cookie.js" charset="gb2312"></script>
    <script type="text/javascript" src="../js/json2.js"></script>
    <script type="text/javascript" src="../js/js_Drag.js"></script>
    <script type="text/javascript" language="javascript">
        document.execCommand("2D-position",false,true);
        function PrintWeb() {
            var spans=document.getElementsByTagName("span");
            for(var i=0;i<spans.length;i++) {
                if(spans[i]!=undefined) {
                    spans[i].ClassName="spanweb";
                }
            }
            document.getElementById("tainer").ClassName="spanweb";
            //行程单id            
            var TripId=document.getElementById("TripId").value;
            //乘客id
            var PasId=document.getElementById("Hid_Pid").value;
            if(TripId!=""||PasId!="") {
                var url="TravelPrint.aspx?PrintTime=ok&tid="+TripId+"&pid="+PasId+"&currentuserid=<%=this.mUser.id.ToString() %>";
                $.post(url,{},function (data) { },"text");
            }
            //打印
            window.print();
        }

        var minLeft=30;
        var leftIndex=minLeft,TopIndex=0;
        function LeftMove() {
            LrMove("Left");
        }
        function RightMove() {
            LrMove("Right");
        }
        function TopMove() {
            tbMove("Top");
        }
        function BottomMove() {
            tbMove("Bottom");
        }
        function LrMove(lr) {
            var Useless=' ';
            var val=document.getElementById("letText").value;
            var regex=eval("/^"+Useless+"*|"+Useless+"*$/g");
            val=val.replace(regex,"");
            if(val==""||isNaN(val)) {
                alert('请输入数字');
                return;
            }
            var letNum=parseInt(val);

            var tableTest=document.getElementById("tainer");
            if(lr=="Left") {
                leftIndex-=letNum;
            } else if(lr=="Right") {
                leftIndex+=letNum;
            }
            var time=24*60*60*1000*365;//一年
            HL.Cookie.Set("leftPos",leftIndex,time);
            tableTest.style.left=leftIndex+"px";
        }
        function tbMove(tb) {
            var Useless=' ';
            var val=document.getElementById("topText").value;
            var regex=eval("/^"+Useless+"*|"+Useless+"*$/g");
            val=val.replace(regex,"");
            if(val==""||isNaN(val)) {
                alert('请输入数字');
                return;
            }
            var tbNum=parseInt(val);
            var tableTest=document.getElementById("tainer");
            if(tb=="Top") {
                TopIndex-=tbNum;
            } else if(tb=="Bottom") {
                TopIndex+=tbNum;
            }
            var time=24*60*60*1000*365;//一年
            HL.Cookie.Set("toptPos",TopIndex,time);
            tableTest.style.top=TopIndex+"px";
        }

        function Restore() {
            HL.Cookie.Set("toptPos","0");
            HL.Cookie.Set("leftPos","0");
            leftIndex=minLeft;
            TopIndex=0;
            var objDiv=document.getElementById("tainer");
            objDiv.style.left=leftIndex+"px";
            objDiv.style.top=TopIndex+"px";
            HL.Cookie.Del("Piont");
            //恢复初始状态标志
            location.href=location.href+"&IsOld=1";
        }
        function selFontSize(val) {
            var fSpan=document.getElementsByTagName("span");
            for(var i=0;i<fSpan.length;i++) {
                fSpan[i].style.fontSize=val;
            }
            var time=24*60*60*1000*365;//一年
            HL.Cookie.Set("fSize",val,time);
        }
        function selFont(Val) {
            var fSpan=document.getElementsByTagName("span");
            for(var i=0;i<fSpan.length;i++) {
                fSpan[i].style.fontFamily=Val;
            }
            var time=24*60*60*1000*365;//一年
            HL.Cookie.Set("fFont",Val,time);
        }
        function setCheckNum(val) {
            document.getElementById("lblCheckNum").innerHTML=val;
        }



        //-----------------调整后的位置保存----------------------------------------------
        function getOffset(obj) {
            var top=obj.offsetTop;
            var left=obj.offsetLeft;
            var width=obj.offsetWidth;
            var height=obj.offsetHeight;
            var val=obj.offsetParent;

            while(val!=null) {
                if(obj.id!="tainer") {
                    if(val.id=="tainer") {
                        break;
                    }
                }
                top+=val.offsetTop;
                left+=val.offsetLeft;
                val=val.offsetParent;
            }
            return {
                Id: obj.id,
                Left: left,
                Top: top,
                Width: width,
                Height: height
            };
        }

        function FindSpan(id,colsSpan) {
            for(var i=0;i<colsSpan.length;i++) {
                if(colsSpan[i].id==id) {
                    return colsSpan[i];
                }
            }
        }
        function LoadPoint() {
            try {
                var str=window.location.href;
                var right="0";
                var es=/IsOld=/;
                if(es.test(str)) {
                    es.exec(str);
                    right=RegExp.rightContext;
                    right.substring(0,right.indexOf('&'));
                }
                var strPiont=HL.Cookie.Get("Piont");
                var url="TravelPrint.aspx";
                var param={
                    optype: "load",
                    num: Math.random(),
                    currentuserid: "<%=this.mUser.id.ToString() %>"
                };
                if(right=="0") {
                    $.post(url,param,function (data) {
                        //请求状态为4表示成功 //http状态200表示OK

                        var strArr=unescape(data).split('$##$');
                        if(strArr!=null&&strArr.length==3) {
                            if(strArr[0]=="1"&&strArr[1]=="load") {
                                strPiont=strArr[2];
                            }
                        }
                        if(strPiont!=""&&strPiont!=undefined) {
                            //容器
                            var Container=document.getElementById("tainer");
                            var Piont=eval("("+strPiont+")");
                            if(Piont!=null&&Piont._Point!=null&&Piont._Font!=null&&Piont._FontSize!=null) {
                                //span
                                var spans=document.getElementsByTagName("span");
                                var span_Font=Piont._Font;//字体                               
                                var selFontObj=document.getElementById("selFont0");
                                for(var i=0;i<selFontObj.options.length;i++) {
                                    if(selFontObj.options[i].value==span_Font) {
                                        selFontObj.options[i].selected=true;
                                        break;
                                    }
                                }
                                var span_FontSize=Piont._FontSize;//字体大小
                                var selFontSizeObj=document.getElementById("SelfontSize");
                                for(var i=0;i<selFontSizeObj.options.length;i++) {
                                    if(selFontSizeObj.options[i].value==span_FontSize) {
                                        selFontSizeObj.options[i].selected=true;
                                        break;
                                    }
                                }

                                var ArrModel=Piont._Point;//位置
                                for(var i=0;i<ArrModel.length;i++) {
                                    if(ArrModel[i].Id!='lblTravelNumber') {
                                        var span=FindSpan(ArrModel[i].Id,spans);
                                        if(span!=null) {
                                            if(span.id=="lblPassengerName") {
                                                span.style.left=ArrModel[i].Left+"px";
                                                span.style.top=ArrModel[i].Top+"px";
                                                span.style.height=ArrModel[i].Height+"px";
                                                span.style.width=ArrModel[i].Width+"px";
                                                span.style.fontSize=span_FontSize;//字体大小
                                                span.style.fontFamily=span_Font;//字体
                                            } else {
                                                span.style.left=ArrModel[i].Left+"px";
                                                span.style.top=ArrModel[i].Top+"px";
                                                span.style.height=ArrModel[i].Height+"px";
                                                span.style.width=ArrModel[i].Width+"px";
                                                span.style.fontSize=span_FontSize;//字体大小
                                                span.style.fontFamily=span_Font;//字体
                                            }
                                        }
                                        //整体容器
                                        if(ArrModel[i].Id=="tainer") {
                                            Container.style.left=ArrModel[i].Left+"px";
                                            Container.style.top=ArrModel[i].Top+"px";
                                            Container.style.height=ArrModel[i].Height+"px";
                                            Container.style.width=ArrModel[i].Width+"px";
                                        }
                                    }
                                }
                            }
                        }

                    },"text");
                }
            } catch(e) {
            }
        }
        function SavePoint() {
            var spans=document.getElementsByTagName("span");
            var pson=[];
            var Container=document.getElementById("tainer");
            pson.push(getOffset(Container));
            for(var i=0;i<spans.length;i++) {
                if(spans[i].id!='lblTravelNumber') {
                    if(spans[i].id=="lblPassengerName") {
                        pson.push(getOffset(spans[i]));
                    } else {
                        pson.push(getOffset(spans[i]));
                    }
                }
            }
            if(pson.length>0) {
                //字体
                var selFont=document.getElementById("selFont0");
                var val_Font=selFont.options[selFont.selectedIndex].value;
                //字体大小
                var selFontSize=document.getElementById("SelfontSize");
                var val_FontSize=selFontSize.options[selFontSize.selectedIndex].value;
                var param={ _Point: pson,_Font: val_Font,_FontSize: val_FontSize };
                //保存Cookie
                var strCookie=JSON.stringify(param);


                var time=24*60*60*1000*365;//一年
                HL.Cookie.Set("Piont",strCookie,time);

                //保存数据库                
                var url="TravelPrint.aspx";
                var SendParam={
                    Point: escape(strCookie),
                    optype: "save",
                    num: Math.random(),
                    currentuserid: "<%=this.mUser.id.ToString() %>"
                };
                $.post(url,SendParam,function (data) {
                    var strArr=unescape(data).split('$##$');
                    if(strArr.length==3) {
                        alert(strArr[2]);
                    }
                },"text");
            }
        }


        function initOne() {
            //加载位置
            LoadPoint();
            //设置所有span均可拖动
            var spans=document.getElementsByTagName("span")
            Drag(spans);
        }
        //---------------------------------------------------------------
    </script>
</head>
<body oncontextmenu="event.returnValue=false" onselectstart="return false" oncopy="return false"
    leftmargin="0" topmargin="10" style="position: absolute;" onload="initOne()">
    <form name="frmReceipt" method="post" id="frmReceipt">
    <div id="tainer" style="position: absolute;" class="border" contenteditable="false">
        <table cellspacing="0" border="0" cellpadding="0" style="width: 750px">
            <%--750px--%>
            <tr>
                <td>
                    <table height="340" cellspacing="0" border="0" cellpadding="0" style="width: 750px;">
                        <tr>
                            <td style="height: 290px">
                                <font style="font-family: 宋体" size="3"><span id="lblIssuedDate" runat="server" style="display: inline-block;
                                    width: 185px; left: 563px; position: absolute; top: 301px; text-align: right;
                                    cursor: move;" contenteditable="false">
                                    <%--2008-9-4 --%></span><span name="lblIssuedBy" runat="server" type="text" id="lblIssuedBy"
                                        style="font-family: 宋体; font-size: Medium; width: 322px; left: 237px; position: absolute;
                                        top: 309px; right: 193px; text-align: center; cursor: move;" contenteditable="true">
                                        <%--ISSUEDBY --%></span><span id="lblAgentCode" style="display: inline-block; width: 223px;
                                            left: 7px; position: absolute; top: 295px; text-align: center; right: 522px;
                                            cursor: move;" runat="server" contenteditable="false">
                                            <%--CKG177 --%></span>
                                    <br />
                                    <span id="Agent_Code" style="display: inline-block; width: 223px; left: 7px; position: absolute;
                                        top: 308px; right: 522px; text-align: center; cursor: move;" runat="server" contenteditable="false">
                                        <%--AGENT CODE--%></span><span id="lblInsurance" runat="server" style="display: inline-block;
                                            width: 115px; left: 632px; position: absolute; top: 270px; text-align: right;
                                            cursor: move;" contenteditable="false">
                                            <%--保险费 --%></span>
                                <span id="lblCheckNum" contenteditable="true" style="display: inline-block;
                                                width: 124px; left: 246px; position: absolute; top: 270px; text-align: center;
                                                right: 378px; cursor: move;" runat="server"></span><span id="lblTicNo" runat="server"
                                                    style="display: inline-block; width: 234px; left: 7px; position: absolute; top: 270px;
                                                    right: 511px; text-align: center; cursor: move;" contenteditable="false">
                                                    <%--784-1234567890 --%></span><span id="lblTotal" runat="server" style="display: inline-block;
                                                        width: 179px; left: 567px; position: absolute; top: 244px; text-align: center;
                                                        cursor: move;" contenteditable="false">
                                                        <%--CNY 1450.00 --%></span><span id="lblOtherTaxes" style="display: inline-block;
                                                            width: 80px; left: 483px; position: absolute; top: 244px; text-align: center;
                                                            cursor: move;" runat="server" contenteditable="false">
                                                            <%--其他税费 --%></span><span id="msgInfo" style="display: inline-block; width: 249px;
                                                                left: 377px; position: absolute; top: 270px; text-align: center; right: 122px;
                                                                cursor: move;" runat="server" contenteditable="true">
                                                                <%--提示信息--%>
                                                            </span><span id="lblYQ" runat="server" style="display: inline-block; width: 105px;
                                                                left: 377px; position: absolute; top: 244px; text-align: center; cursor: move;"
                                                                contenteditable="false">
                                                                <%--YQ 150.00--%>
                                                            </span><span runat="server" id="lblTax" style="display: inline-block; width: 111px;
                                                                left: 261px; position: absolute; top: 244px; right: 380px; text-align: center;
                                                                cursor: move;" contenteditable="false">
                                                                <%--CN 50.00 --%></span><span id="lblFare" runat="server" style="display: inline-block;
                                                                    width: 102px; left: 146px; position: absolute; top: 244px; right: 504px; text-align: center;
                                                                    cursor: move;" contenteditable="false">
                                                                    <%--CNY 1250.00 --%></span><span id="lblTo4" style="display: inline-block; width: 103px;
                                                                        left: 7px; position: absolute; top: 244px; text-align: center; cursor: move;"
                                                                        runat="server" contenteditable="false">
                                                                        <%--VOID--%>
                                                                    </span><span id="lblAllow3" style="display: inline-block; width: 50px; left: 696px;
                                                                        position: absolute; top: 200px; text-align: center; cursor: move;" runat="server"
                                                                        contenteditable="false">
                                                                        <%--10K --%></span><span id="lblInvalidDate3" style="display: inline-block; height: 20px;
                                                                            width: 95px; left: 626px; position: absolute; top: 201px; cursor: move;" runat="server"
                                                                            contenteditable="false">
                                                                            <%--21SEP--%></span><span id="lblValidDate3" style="display: inline-block; height: 20px;
                                                                                width: 95px; position: absolute; top: 200px; left: 543px; right: 141px; cursor: move;"
                                                                                runat="server" contenteditable="false">
                                                                                <%--10SEP--%></span><span id="lblFareBasis3" style="display: inline-block; width: 97px;
                                                                                    left: 465px; position: absolute; top: 200px; right: 222px; text-align: center;
                                                                                    cursor: move;" runat="server" contenteditable="false">
                                                                                    <%--Y70 --%></span><span id="lblTime3" runat="server" style="display: inline-block;
                                                                                        width: 50px; left: 412px; position: absolute; top: 200px; right: 343px; cursor: move;"
                                                                                        contenteditable="false">
                                                                                        <%--1920 --%></span><span id="lblDate3" style="display: inline-block; height: 20px;
                                                                                            width: 95px; left: 315px; position: absolute; top: 200px; cursor: move; right: 338px;"
                                                                                            runat="server" contenteditable="false">
                                                                                            <%--12SEP --%></span><span id="lblClass3" style="display: inline-block; height: 20px;
                                                                                                width: 51px; text-align: center; left: 262px; position: absolute; top: 200px;
                                                                                                right: 460px; cursor: move;" runat="server" contenteditable="false">
                                                                                                <%--P --%></span><span id="lblFlight3" style="display: inline-block; height: 20px;
                                                                                                    width: 53px; left: 212px; position: absolute; top: 200px; right: 501px; cursor: move;"
                                                                                                    runat="server" contenteditable="false">
                                                                                                    <%--8088 --%></span>
                                    <span id="lblCarrier3" style="display: inline-block; height: 20px; width: 59px; left: 164px;
                                        position: absolute; top: 200px; right: 529px; white-space: nowrap; cursor: move;"
                                        runat="server" contenteditable="false">
                                        <%--MU--%>
                                    </span><span id="lblTo3" runat="server" style="display: inline-block; width: 145px;
                                        left: 7px; position: absolute; top: 200px; right: 641px; text-align: center;
                                        cursor: move;" contenteditable="false">
                                        <%--杭州HGH--%>
                                    </span><span id="lblAllow2" runat="server" style="display: inline-block; width: 50px;
                                        left: 696px; position: absolute; top: 176px; text-align: center; cursor: move;"
                                        contenteditable="false">
                                        <%--40K--%>
                                    </span><span id="lblInvalidDate2" style="display: inline-block; width: 95px; left: 626px;
                                        position: absolute; top: 176px; right: 51px; cursor: move;" runat="server" contenteditable="false">
                                        <%--15AUG--%>
                                    </span><span id="lblValidDate2" style="display: inline-block; width: 95px; left: 543px;
                                        position: absolute; top: 176px; right: 141px; cursor: move;" runat="server" contenteditable="false">
                                        <%--21JUN--%></span><span id="lblFareBasis2" runat="server" style="display: inline-block;
                                            width: 97px; left: 465px; position: absolute; top: 176px; text-align: center;
                                            cursor: move;" contenteditable="false">
                                            <%--Y60 --%></span><span id="lblTime2" runat="server" style="display: inline-block;
                                                width: 50px; left: 412px; position: absolute; top: 176px; cursor: move;" contenteditable="false">
                                                <%--2005--%>
                                            </span><span id="lblDate2" runat="server" style="display: inline-block; width: 95px;
                                                left: 315px; position: absolute; top: 176px; cursor: move;" contenteditable="false">
                                                <%--20MAR --%></span><span id="lblClass2" runat="server" style="display: inline-block;
                                                    text-align: center; width: 51px; left: 262px; position: absolute; top: 176px;
                                                    cursor: move;" contenteditable="false">
                                                    <%--C--%></span><span id="lblFlight2" runat="server" style="display: inline-block;
                                                        width: 53px; left: 212px; position: absolute; top: 176px; right: 503px; cursor: move;"
                                                        contenteditable="false">
                                                        <%--8017 --%></span><span id="lblCarrier2" runat="server" style="display: inline-block;
                                                            width: 59px; left: 164px; position: absolute; top: 176px; right: 529px; cursor: move;
                                                            white-space: nowrap;" contenteditable="false">
                                                            <%--CA--%></span><span id="lblTo2" runat="server" style="display: inline-block; width: 145px;
                                                                left: 7px; position: absolute; top: 175px; text-align: center; cursor: move;"
                                                                contenteditable="false">
                                                                <%--广州CAN --%></span><span id="lblAllow1" style="display: inline-block; width: 50px;
                                                                    left: 696px; position: absolute; top: 144px; text-align: center; cursor: move;"
                                                                    runat="server" contenteditable="false">
                                                                    <%--30K--%>
                                                                </span><span id="lblInvalidDate1" style="display: inline-block; width: 95px; left: 626px;
                                                                    position: absolute; top: 144px; cursor: move;" runat="server" contenteditable="false">
                                                                    <%--26FEB--%>
                                                                </span><span id="lblValidDate1" style="display: inline-block; width: 95px; left: 543px;
                                                                    position: absolute; top: 144px; right: 141px; cursor: move;" runat="server" contenteditable="false">
                                                                    <%--20FEB--%></span><span id="lblFareBasis1" style="display: inline-block; width: 97px;
                                                                        left: 465px; position: absolute; top: 144px; text-align: center; cursor: move;"
                                                                        runat="server" contenteditable="false">
                                                                        <%--Y50 --%></span><span id="lblTime1" runat="server" style="display: inline-block;
                                                                            width: 52px; position: absolute; top: 144px; left: 412px; cursor: move; right: 284px;"
                                                                            contenteditable="false">
                                                                            <%--16O0--%>
                                                                        </span><span id="lblDate1" style="display: inline-block; width: 95px; left: 315px;
                                                                            position: absolute; top: 144px; cursor: move;" runat="server" contenteditable="false">
                                                                            <%--19SEP--%>
                                                                        </span><span id="lblClass1" runat="server" style="display: inline-block; width: 51px;
                                                                            text-align: center; left: 262px; position: absolute; top: 144px; cursor: move;"
                                                                            contenteditable="false">
                                                                            <%--F --%></span><span id="lblFlight1" runat="server" style="display: inline-block;
                                                                                width: 53px; left: 212px; position: absolute; top: 144px; right: 503px; cursor: move;"
                                                                                contenteditable="false">
                                                                                <%--8016 --%></span><span id="lblCarrier1" runat="server" style="display: inline-block;
                                                                                    height: 20px; width: 59px; left: 164px; position: absolute; top: 144px; bottom: 178px;
                                                                                    white-space: nowrap; cursor: move;" contenteditable="false">
                                                                                    <%--VOID--%></span><span id="lblTo1" runat="server" style="display: inline-block;
                                                                                        width: 145px; height: 20px; left: 7px; position: absolute; top: 113px; bottom: 209px;
                                                                                        text-align: center; cursor: move;" contenteditable="false">
                                                                                        <%--北京PEK --%></span><span runat="server" id="lblAllow" style="display: inline-block;
                                                                                            width: 50px; left: 696px; position: absolute; top: 112px; bottom: 209px; text-align: center;
                                                                                            cursor: move;" contenteditable="false">
                                                                                            <%--20K --%></span><span id="lblInvalidDate" style="display: inline-block; height: 21px;
                                                                                                width: 95px; left: 626px; position: absolute; top: 112px; right: 51px; cursor: move;"
                                                                                                runat="server" contenteditable="false">
                                                                                                <%--27FEB --%></span>
                                    <span id="lblValidDate" style="display: inline-block; width: 95px; left: 543px; position: absolute;
                                        top: 112px; right: 144px; cursor: move;" runat="server" contenteditable="false">
                                        <%--29FEB --%></span><span id="lblFareBasis" runat="server" style="display: inline-block;
                                            width: 97px; left: 465px; position: absolute; top: 112px; right: 240px; text-align: center;
                                            cursor: move;" contenteditable="false">
                                            <%--Y80 --%></span><span id="lblTime" runat="server" style="display: inline-block;
                                                height: 22px; width: 52px; left: 412px; position: absolute; top: 111px; cursor: move;"
                                                contenteditable="false">
                                                <%--1940 --%></span><span id="lblDate" style="display: inline-block; width: 95px;
                                                    left: 315px; position: absolute; top: 111px; right: 342px; cursor: move;" runat="server"
                                                    contenteditable="false">
                                                    <%--18SEP--%></span><span id="lblClass" runat="server" style="display: inline-block;
                                                        width: 51px; left: 262px; position: absolute; top: 111px; text-align: center;
                                                        right: 439px; cursor: move;" contenteditable="false">
                                                        <%--H--%></span><span id="lblFlight" runat="server" style="display: inline-block;
                                                            width: 53px; left: 212px; position: absolute; top: 111px; right: 490px; cursor: move;"
                                                            contenteditable="false">
                                                            <%--8105 --%></span><span id="lblCarrier" runat="server" style="display: inline-block;
                                                                width: 59px; white-space: nowrap; left: 164px; position: absolute; top: 112px;
                                                                cursor: move;" contenteditable="false">
                                                                <%--CZ--%></span><span id="lblFrom" runat="server" style="display: inline-block;
                                                                    width: 145px; left: 7px; position: absolute; top: 143px; text-align: center;
                                                                    cursor: move;" contenteditable="false">
                                                                    <%--重庆CKG--%>
                                                                </span><span id="lblTravelNumber" runat="server" class="notprint" style="width: 183px;
                                                                    left: 562px; position: absolute; top: 12px; text-align: center; cursor: move;"
                                                                    contenteditable="true">
                                                                    <%--4394146201--%>
                                                                </span><span name="lblPersonCardID" type="text" id="lblPersonCardID" style="font-family: 宋体;
                                                                    font-size: Medium; width: 222px; left: 207px; position: absolute; top: 58px;
                                                                    cursor: move;" runat="server" contenteditable="true">
                                                                    <%--证件号 --%></span><span id="qianzhu" runat="server" contenteditable="true" style="display: inline-block;
                                                                        position: absolute; width: 314px; left: 430px; position: absolute; top: 58px;
                                                                        text-align: center; cursor: move;">
                                                                        <%--不得签转 --%></span><span id="lblPassengerName" style="display: inline-block; width: 125px;
                                                                            left: 6px; position: absolute; top: 58px; right: 617px; text-align: center; cursor: move;"
                                                                            runat="server" contenteditable="false">
                                                                            <%--乘机人 --%></span><span id="lblPnr" style="display: inline-block; width: 122px;
                                                                                left: 8px; position: absolute; top: 85px; text-align: center; cursor: move;"
                                                                                runat="server">
                                                                                <%--编码 --%></span><span id="lblConjunctionTKT" runat="server" style="display: inline-block;
                                                                                    width: 323px; left: 237px; position: absolute; top: 297px; text-align: center;
                                                                                    cursor: move;" contenteditable="true">
                                                                                    <%--填开单位 --%></span></font>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <p class="notprint" style="position: absolute; top: 350px; left: 50px; width: 800px;
        font-size: 12px; color: Red;">
        *每个打印位置均可以用鼠标移动调整
    </p>
    <table align="center" class="notprint" border="1" style="z-index: 105; position: absolute;
        top: 430px; left: 50px; width: 730px; border-collapse: collapse; border: solid 1px blabk;">
        <tr>
            <td colspan="3" align="center">
                <p style="font-size: 15px; color: Red;">
                    整体范围调整设置
                </p>
            </td>
        </tr>
        <tr align="center" style="width: 730px;">
            <td valign="middle">
                <input type="text" style="width: 30px" id="letText" value="5" />
                <input onclick="LeftMove()" type="button" value="左调" />
            </td>
            <td valign="middle">
                <input onclick="RightMove()" type="button" value="右调" />
            </td>
            <td align="left" valign="middle">
                <p>
                    字体：<select id="selFont0" runat="server" style="width: 120px;" onchange="selFont(this.options[this.selectedIndex].value)">
                    </select>
                </p>
            </td>
        </tr>
        <tr align="center" valign="middle" style="width: 730px;">
            <td>
                <input type="text" style="width: 30px" id="topText" value="5" />
                <input onclick="TopMove()" type="button" value="上调" />
            </td>
            <td valign="middle">
                <input onclick="BottomMove()" type="button" value="下调" />
            </td>
            <td align="left" valign="middle">
                <p style="font-size: 12px;">
                    字体大小：
                    <select id="SelfontSize" style="width: 80px;" onchange="selFontSize(this.options[this.selectedIndex].value)">
                        <option value="9">9</option>
                        <option value="10">10</option>
                        <option value="11">11</option>
                        <option value="12">12</option>
                        <option value="13">13</option>
                        <option value="14" selected="selected">14</option>
                        <option value="15">15</option>
                        <option value="16">16</option>
                        <option value="17">17</option>
                        <option value="18">18</option>
                        <option value="19">19</option>
                        <option value="20">20</option>
                        <option value="21">21</option>
                        <option value="22">22</option>
                        <option value="23">23</option>
                        <option value="24">24</option>
                        <option value="25">25</option>
                    </select>
                </p>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <span>行程单号:</span><input type="text" runat="server" id="inputCheckNum" style="width: 100px"
                    readonly="readonly" />
            </td>
        </tr>
        <tr style="width: 730px;">
            <td colspan="3" align="center">
                <input onclick="Restore()" type="button" value="恢复初始位置" />
                <input onclick="SavePoint()" type="button" value="保存位置" />
                <input type="hidden" id="TripId" runat="server" />
                <input type="hidden" id="Hid_Pid" runat="server" />
                <input onclick="PrintWeb();" type="button" value="行程单打印&nbsp;&nbsp;&nbsp;" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>

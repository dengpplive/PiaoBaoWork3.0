<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" ValidateRequest="false" Debug="true" CodeBehind="testArch.aspx.cs"
    Inherits="PiaoBao.API.Test.testArch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="testUserCase">
    </div>
    <div>
        Uri:<asp:TextBox ID="txtUri" runat="server" Width="432px"></asp:TextBox>
        <asp:TextBox ID="txtMethod" runat="server"></asp:TextBox>
        <br />
        Data:<asp:TextBox ID="txtData" runat="server" Width="421px" TextMode="MultiLine"
            Height="66px"></asp:TextBox>
        <br />
        <div id="divRemark">
        </div>
        <asp:Button ID="btnSubmit" runat="server" Text="开始测试" OnClick="btnSubmit_Click" />
        <input type="button" value="清除数据" onclick="document.getElementById('txtResult').value=''" />
        <br />
        <asp:TextBox ID="txtResult" runat="server" TextMode="MultiLine" Height="464px" Width="1030px"></asp:TextBox>
    </div>
    </form>
    <script type="text/javascript">
        //var baseurl = "http://localhost:37127/api/";
        var baseurl = "http://210.14.138.26:206/api/";
        function CreateDataList() {
            var testDataList = {}

            //查询单程航班
            var q1 = {};
            q1.Code = "a";
            q1.Name = "查询单程航班";
            q1.Method = "GET";
            q1.Url = baseurl + "SkyWay/OneWay";
            q1.Data = "startCity=CTU&endCity=PEK&startDate=20130720&cairry=CA&isQueryShareFlight=1&isQuerySpecialFlight=1";
            q1.Remark = "参数及调用说明";
            testDataList[q1.Code] = q1;

            //查询往返航班
            var q2 = {};
            q2.Code = "b";
            q2.Name = "查询往返航班";
            q2.Method = "Get";
            q2.Url = baseurl + "SkyWay/RoundWay";
            q2.Data = "startCity=CTU&endCity=PEK&startDate=20130720&backDate=20130724&cairry=CA&isQueryShareFlight=1&isQuerySpecialFlight=1";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //查询联程航班
            var q2 = {};
            q2.Code = "c";
            q2.Name = "查询联程航班";
            q2.Method = "Get";
            q2.Url = baseurl + "SkyWay/ConnWay";
            q2.Data = "startCity=CTU&midCity=XIY&endCity=PEK&startDate=20130720&midDate=20130723&cairry=CA&isQueryShareFlight=1&isQuerySpecialFlight=1";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //查询航班特价
            var q2 = {};
            q2.Code = "d";
            q2.Name = "查询航班特价";
            q2.Method = "Get";
            q2.Url = baseurl + "SkyWay/SpecialPrice";
            q2.Data = "cacheNameGuid=&fullFlightNo=&cairrGuid=&policyGuid=";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //查询订单
            var q2 = {};
            q2.Code = "e";
            q2.Name = "查询订单";
            q2.Method = "Get";
            q2.Url = baseurl + "orderList";
            q2.Data = "pager=10;1&GYList=&orderid=&pnr=&passengerName=&corporationName=&flightCode=&airCode=&status=&airFTimeDate=&airETimeDate=&createFTimeDate=&createETimeDate=&fcity=&tcity=";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //获取订单
            var q2 = {};
            q2.Code = "f";
            q2.Name = "获取订单";
            q2.Method = "Get";
            q2.Url = baseurl + "order/0130513163701396704";
            q2.Data = "pnr=JVXS6K";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //取票
            var q2 = {};
            q2.Code = "g";
            q2.Name = "取票";
            q2.Method = "Get";
            q2.Url = baseurl + "Ticket";
            q2.Data = "orderID=0130513113839835855";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //查询实时政策
            var q2 = {};
            q2.Code = "h";
            q2.Name = "查询实时政策";
            q2.Method = "Get";
            q2.Url = baseurl + "Policy";
            q2.Data = "orderID=0130513113839835855";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //创建PNR
            var q2 = {};
            q2.Code = "i";
            q2.Name = "创建PNR";
            q2.Method = "Post";
            q2.Url = baseurl + "PNR";
            q2.Data = "pList=陈丽,1,510183198908170011&skywaylist=CZ,3904,2013-8-6 19:45,2013-8-6 22:25,CTU,PEK,成都,北京,W,5&linkMan=陈丽&linkManPhone=13548106127&travelType=1";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //确认订单
            var q2 = {};
            q2.Code = "j";
            q2.Name = "确认订单";
            q2.Method = "Post";
            q2.Url = baseurl + "confirm";
            q2.Data = "adultOrderId=&childOrderId=&adultPolicyGuid=&childPolicyGuid=&remark=";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //退废改签
            var q2 = {};
            q2.Code = "k";
            q2.Name = "退废改签";
            q2.Method = "Put";
            q2.Url = baseurl + "Ticket";
            q2.Data = "pasList=&skywayList=&orderID=0130513113839835855&ApplayType=&TGQRemark=&quitReasonType=&reason=";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //支付订单
            var q2 = {};
            q2.Code = "l";
            q2.Name = "支付订单";
            q2.Method = "Put";
            q2.Url = baseurl + "order/0130513113839835855";
            q2.Data = "pnr=JVXS6K";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;

            //取消订单
            var q2 = {};
            q2.Code = "m";
            q2.Name = "取消订单";
            q2.Method = "Delete";
            q2.Url = baseurl + "order/0130513113839835855";
            q2.Data = "pnr=JVXS6K";
            q2.Remark = "参数及调用说明22";
            testDataList[q2.Code] = q2;




            return testDataList;
        }
        var data = CreateDataList();
        var usercase = "";
        for (var i in data) {
            usercase += "<a href='#' onclick='setData(\"" + data[i].Code + "\")'>" + data[i].Name + "</a> | ";
        }
        document.getElementById("testUserCase").innerHTML = usercase;



        function setData(code) {
            var d = data[code];
            document.getElementById("txtUri").value = d.Url;
            document.getElementById("txtMethod").value = d.Method;
            document.getElementById("txtData").value = d.Data;
            document.getElementById("divRemark").innerHTML = d.Remark;
        }

    </script>
</body>
</html>

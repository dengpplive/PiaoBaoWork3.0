<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpCabinPriceList.aspx.cs"
    Inherits="Manager_Base_SpCabinPriceList" %>

<%@ Register Src="~/UserContrl/SelectAirCode.ascx" TagName="SelectAirCode" TagPrefix="uc1" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>特价舱位价格管理</title>
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script id="script_k0" src="../../JS/CitySelect/tuna_100324_jsLoader.js" type="text/javascript"
        charset="utf-8"></script>
    <style type="text/css">
        .Search tr
        {
            height: 30px;
            line-height: 30px;
        }
        .tb-all-trade td
        {
            text-align: center;
        }
        .show
        {
            display: block;
        }
        .hide
        {
            display: none;
        }
        .inputW100
        {
            width: 100px;
        }
    </style>
    <script type="text/javascript">
        var $J=jQuery.noConflict(false);
        //显示对话框
        function showdialog(t,param) {
            var div=document.getElementById("dg");
            if(div==null) {
                div=document.createElement("div");
                div.id="dg";
                if(document.all) {
                    document.body.appendChild(div);
                }
                else {
                    document.insertBefore(div,document.body);
                }
            }
            $J(dg).html(t);
            $J(dg).dialog({
                title: '标题',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $J(this).dialog('close');
                        if(param!=null) {
                            if(param.OK=="1") {
                                $J("#btnQuery").click();
                            }
                        }
                    },
                    "取消": function () {
                        $J(this).dialog('close');
                    }
                }
            });
        }
        //清空数据
        function ClearData() {
            $J('input[type="text"]').val("");
            $J('select option').eq(0).attr("selected",true)
            return false;
        }
        function SelAll(obj) {
            $J("input[type='checkbox'][name='ckChildSel']").attr("checked",obj.checked);
        }
        function patchDel() {
            var SelBox=$J("input[type='checkbox'][name='ckChildSel']:checked");
            var idArr=[];
            SelBox.each(function (index,ckBox) {
                var id=$J(ckBox).val();
                if(id!="") {
                    idArr.push("'"+id+"'");
                }
            });
            if(idArr.length==0) {
                showdialog('请选择需要删除的数据！');
            } else {
                var ids=idArr.join(",");
                var url="SpCabinPriceList.aspx";
                var param=
                {
                    CommandName: "Del",
                    IdList: ids,
                    num: Math.random(),
                    currentuserid: $J("#currentuserid").val()
                };
                //处理函数
                function fn_Hand(data) {
                    if(data=="1") {
                        showdialog('删除成功！',{ OK: 1 });
                    } else {
                        showdialog('删除失败！');
                    }
                }
                //发送请求
                $J.post(url,param,fn_Hand,"text");
            }
            return false;
        }

        function edit(id) {
            $J("#del_"+id).hide();
            $J("#show_edit"+id).hide();
            $J("#hide_edit"+id).show();

            $J("#show_SpPrice"+id).hide();
            $J("#hide_SpPrice"+id).show();

            $J("#show_SpABFare"+id).hide();
            $J("#hide_SpABFare"+id).show();

            $J("#show_SpRQFare"+id).hide();
            $J("#hide_SpRQFare"+id).show();

            return false;
        }
        function Cancel(id) {
            $J("#del_"+id).show();
            $J("#show_edit"+id).show();
            $J("#hide_edit"+id).hide();

            $J("#show_SpPrice"+id).show();
            $J("#hide_SpPrice"+id).hide();

            $J("#show_SpABFare"+id).show();
            $J("#hide_SpABFare"+id).hide();

            $J("#show_SpRQFare"+id).show();
            $J("#hide_SpRQFare"+id).hide();


            return false;
        }

        function Update(id) {
            var SpPrice=$J("#txtSpPrice_"+id).val();
            var SpABFare=$J("#txtSpABFare_"+id).val();
            var SpRQFare=$J("#txtSpRQFare_"+id).val();
            if($J.isNaN(SpPrice)) {
                showdialog('舱位价格式错误！');
                return false;
            }

            if($J.isNaN(SpABFare)) {
                showdialog('机建费格式错误！');
                return false;
            }

            if($J.isNaN(SpRQFare)) {
                showdialog('燃油费格式错误！');
                return false;
            }

            var url="SpCabinPriceList.aspx";
            var param=
                {
                    CommandName: escape("Update"),
                    IdList: encodeURIComponent(id),
                    SpPrice: SpPrice,
                    SpABFare: SpABFare,
                    SpRQFare: SpRQFare,

                    num: Math.random(),
                    currentuserid: jQueryOne("#currentuserid").val()
                };
            //处理函数
            function fn_Hand(data) {
                if(data=="1") {
                    showdialog('修改成功！',{ OK: 1 });
                } else {
                    showdialog('修改失败！');
                }
            }
            //发送请求
            $J.post(url,param,fn_Hand,"text");
            return false;
        }
    </script>
    <script type="text/javascript" src="../../js/My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <%--城市控件使用容器开始--%>
    <div id="jsContainer">
        <div id="jsHistoryDiv" class="hide">
            <iframe id="jsHistoryFrame" name="jsHistoryFrame" src="about:blank"></iframe>
        </div>
        <textarea id="jsSaveStatus" class="hide"></textarea>
        <div id="tuna_jmpinfo" style="visibility: hidden; position: absolute; z-index: 120;
            overflow: hidden;">
        </div>
        <div id="tuna_alert" style="display: none; position: absolute; z-index: 999; overflow: hidden;">
        </div>
        <%--日期容器--%>
        <div style="position: absolute; top: 0; z-index: 120; display: none" id="tuna_calendar"
            class="tuna_calendar">
        </div>
    </div>
    <%--城市控件使用容器结束--%>
    <form id="form2" runat="server">
    <div id="tabs-1">
        <div class="title">
            <span>特价舱位价格管理</span>
        </div>
        <div class="c-list-filter">
            <table class="Search" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;">
                <tr>
                    <th>
                        出发城市：
                    </th>
                    <td>
                        <%-- 出发城市--%>
                        <input name="txtFromCityName" class="inputtxtdat" runat="server" type="text" id="txtFromCityName"
                            mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                            mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                            style="width: 150px;" mod_address_reference="FromCityCode" />
                        <input id="FromCityCode" name="FromCityCode" type="hidden" runat="server" />
                    </td>
                    <th>
                        承运人：
                    </th>
                    <td>
                        <uc1:SelectAirCode ID="txtCarryCode" IsShowAll="true" IsDShowName="false" DefaultOptionValue=""
                            runat="server" />
                    </td>
                    <th>
                        起飞时间：
                    </th>
                    <td>
                        <input type="text" readonly="readonly" id="txtStartFlightTime" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                            style="width: 110px;" class="Wdate inputtxtdat" />-<input type="text" readonly="readonly"
                                id="txtEndFlightTime" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                                style="width: 110px;" class="Wdate inputtxtdat" />
                    </td>
                </tr>
                <tr>
                    <th>
                        到达城市：
                    </th>
                    <td>
                        <%-- 到达城市--%>
                        <input name="txtToCityName" class="inputtxtdat" runat="server" type="text" id="txtToCityName"
                            mod_address_suggest="@Beijing|北京|PEK@Shanghai|上海|SHA@Shenzhen|深圳|SZX@Guangzhou|广州|CAN@Qingdao|青岛|TAO@Chengdu|成都|CTU@Hangzhou|杭州|HGH@Wuhan|武汉|WUH@Tianjin|天津|TSN@Dalian|大连|DLC@Xiamen|厦门|XMN@Chongqing|重庆|CKG@"
                            mod_address_source="fltdomestic" mod_notice_tip="中文/英文" mod="address|notice"
                            style="width: 150px;" mod_address_reference="ToCityCode" />
                        <input id="ToCityCode" name="ToCityCode" type="hidden" runat="server" />
                    </td>
                    <th>
                        舱位：
                    </th>
                    <td>
                        <asp:TextBox ID="txtSpCabin" runat="server" MaxLength="2"></asp:TextBox>
                    </td>
                    <th>
                        缓存时间：
                    </th>
                    <td>
                        <input type="text" readonly="readonly" id="txtStartUpdateTime" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                            style="width: 110px;" class="Wdate inputtxtdat" />-<input type="text" readonly="readonly"
                                id="txtEndUpdateTime" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                                style="width: 110px;" class="Wdate inputtxtdat" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="4">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnDel" runat="server" Text="批量删除 " OnClientClick="return patchDel();" /></span>
                        <span class="btn btn-ok-s">
                            <asp:Button ID="Reset" runat="server" Text="重置数据" OnClientClick="return ClearData();" /></span>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <table id="tb-all-trade" class="tb-all-trade" width="100%" cellspacing="" cellpadding="0"
                border="1" style="border-collapse: collapse;">
                <thead>
                    <tr>
                        <th>
                            <input type="checkbox" id="ckSel" name="ckSel" onclick="SelAll(this)" />
                        </th>
                        <th>
                            航空公司二字码
                        </th>
                        <th>
                            航班号
                        </th>
                        <th>
                            出发城市三字码
                        </th>
                        <th>
                            到达城市三字码
                        </th>
                        <th>
                            舱位
                        </th>
                        <th>
                            舱位价
                        </th>
                        <th>
                            基建费
                        </th>
                        <th>
                            燃油费
                        </th>
                        <th>
                            起飞时间
                        </th>
                        <th>
                            缓存时间
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater ID="repSpList" runat="server" OnItemCommand="repSpList_ItemCommand">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <td class="Operation">
                                <input type="checkbox" id="ckChildSel" name="ckChildSel" value='<%# Eval("id") %>' />
                            </td>
                            <td>
                                <%#Eval("SpAirCode")%>
                            </td>
                            <td>
                                <%#Eval("SpFlightCode")%>
                            </td>
                            <td>
                                <%#Eval("FromCityCode")%>
                            </td>
                            <td>
                                <%#Eval("ToCityCode")%>
                            </td>
                            <td>
                                <%#Eval("SpCabin")%>
                            </td>
                            <td>
                                <span id="show_SpPrice<%# Eval("id") %>" class="show">
                                    <%#Eval("SpPrice")%></span> <span id="hide_SpPrice<%# Eval("id") %>" class="hide">
                                        <input type="text" id='txtSpPrice_<%# Eval("id") %>' value='<%#Eval("SpPrice")%>'
                                            size="5" class="inputW100" />
                                    </span>
                            </td>
                            <td>
                                <span id="show_SpABFare<%# Eval("id") %>" class="show">
                                    <%#Eval("SpABFare")%></span> <span id="hide_SpABFare<%# Eval("id") %>" class="hide">
                                        <input type="text" id='txtSpABFare_<%# Eval("id") %>' value='<%#Eval("SpABFare")%>'
                                            size="5" class="inputW100" />
                                    </span>
                            </td>
                            <td>
                                <span id="show_SpRQFare<%# Eval("id") %>" class="show">
                                    <%#Eval("SpRQFare")%></span> <span id="hide_SpRQFare<%# Eval("id") %>" class="hide">
                                        <input type="text" id='txtSpRQFare_<%# Eval("id") %>' value='<%#Eval("SpRQFare")%>'
                                            size="5" class="inputW100" />
                                    </span>
                            </td>
                            <td>
                                <%#Eval("FlightTime")%>
                            </td>
                            <td>
                                <%#Eval("UpdateTime")%>
                            </td>
                            <td>
                                <span id="del_<%# Eval("id") %>">
                                    <asp:LinkButton runat="server" ID="btn_Del" CommandName="Del" OnClientClick="return confirm('确认要删除该航线数据吗？');"
                                        CommandArgument='<%# Eval("id") %>' Text="删除"></asp:LinkButton>
                                </span><span id='show_edit<%# Eval("id") %>' class="show"><a href="#" onclick="return edit('<%# Eval("id") %>');">
                                    编辑</a> </span><span id='hide_edit<%# Eval("id") %>' class="hide"><a href="#" onclick="return Update('<%# Eval("id") %>');">
                                        更新</a> <a href="#" onclick="return Cancel('<%# Eval("id") %>');">取消</a>
                                </span>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="20" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
            </webdiyer:AspNetPager>
        </div>
    </div>
    <asp:HiddenField ID="currentuserid" runat="server" ClientIDMode="Static" />
    <script language="javascript" type="text/javascript">
        function ReLoad() {
            var rd=""; //  "?r=" + Math.random();
            var SE=new CtripJsLoader();
            var files=[["../../JS/CitySelect/tuna_100324.js"+rd,"GB2312",true,null],["../../AJAX/GetCity.aspx"+rd,"GB2312",true,null]];
            SE.scriptAll(files);
        }
        $J(function () {
            ReLoad();
        });
    </script>
    </form>
</body>
</html>

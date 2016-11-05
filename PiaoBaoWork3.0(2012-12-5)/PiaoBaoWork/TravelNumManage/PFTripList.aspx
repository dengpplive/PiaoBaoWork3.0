<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PFTripList.aspx.cs" Inherits="TravelNumManage_PFTripList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>行程单管理</title>
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
    <link type="text/css" href="../css/table.css" rel="stylesheet" />
    <style type="text/css">
        .title
        {
            font-size: 14px;
            font-weight: bolder;
            height: 100%;
            line-height: 20px;
            padding: 3px 0;
            color: #0092FF;
            text-align: left;
            background-color: #EFF4F8;
            border: 1px solid #D4D4D4;
        }
        .title span
        {
            margin-left: 15px;
        }
        .colorGreen
        {
            color: Green;
        }
        .colorRed
        {
            color: red;
        }
        .colorYellow
        {
            color: #ED911B;
        }
        .hide
        {
            display: none;
        }
        .show
        {
            display: block;
        }
    </style>
    <script type="text/javascript" language="javascript">
        var $J=jQuery.noConflict(false);
        function CreateDiv(id) {
            var div=document.getElementById(id);
            if(div==null) {
                div=document.createElement("div");
                div.id=id;
                if(document.all) {
                    document.body.appendChild(div);
                }
                else {
                    document.insertBefore(div,document.body);
                }
            }
            return div;
        }
        //显示对话框
        function showdg(id,BtnObj,html,t) {
            $J("select").hide();
            var dg=CreateDiv(id);
            $J(dg).html(html);
            var param=
            {
                title: t==null?'提示':t,
                bgiframe: true,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {
                    $J("select").show();
                }
            }
            if(BtnObj=="0") {
                param.buttons={
                    "确定": function () {
                        $J(dg).dialog('close');
                        if(html.indexOf('成功')!= -1) {
                            $J("#btnQuery").click();
                        }
                    }
                };
            }
            else if(BtnObj=="1") {
                param.buttons={
                    "修改": function () {
                        $J(dg).dialog('close');
                        UpdateStatus();
                    },
                    "取消": function () {
                        $J(dg).dialog('close');
                    }
                };
            }
            $J(dg).dialog(param).css({ "width": "auto","height": "auto" });
        }
        //全选
        function setSelAll(obj) {
            var ckAllBox=$J("input[name='ck_ids'][type='checkbox']:visible");
            for(var i=0;i<ckAllBox.length;i++) {
                ckAllBox[i].checked=obj.checked;
            }
        }
        //显示编辑
        function ShowEdit(type,id) {
            var RoleType=$J("#Hid_RoleType").val();
            if(RoleType=="1") {
                var Ids=[];
                if(type==1) {
                    id="";
                    $J("input[name='ck_ids'][type='checkbox']:visible").each(function (index,ck) {
                        if($J(ck).attr("checked")) {
                            Ids.push("'"+$J(ck).val()+"'");
                        }
                    });
                } else {
                    Ids.push("'"+id+"'");
                }
                if(Ids.length==0) {
                    showdg("aaa",0,"请选择需要修改的行程单号！","行程单状态修改");
                } else {
                    var html='<table><tr><td>行程单状态:</td><td><select id="selTripStatus" style="width:200px;">';
                    html+='<option value="1">已入库,未分配</option>';//1    
                    html+='<option value="2">已分配,未使用 </option>';//  2   已分配,未使用   
                    html+='<option value="6">作废行程单,等待审核</option>';//6   作废行程单,等待审核
                    html+='<option value="7">拒绝作废行程单,审核失败 </option>';//7   拒绝作废行程单,审核失败  
                    html+='<option value="8">空白回收,未分配</option>';//8   空白回收,未分配    
                    html+='<option value="9">已创建行程单</option>';//9   已创建行程单     
                    html+='<option value="10">已作废行程单,审核成功 </option>';//10   已作废行程单,审核成功 
                    html+='<option value="11">空白回收,已分配</option>';//11   空白回收,已分配                
                    html+='</select></td></tr>';
                    html+='</table>';
                    html+='</table><input type="hidden" id="optype" value="'+type+'"><input type="hidden" id="ids" value="'+id+'">';
                    showdg("aaa",1,html,"行程单状态修改");
                }
            } else {
                showdg("aaa",0,"您无权限操作","行程单状态修改");
            }
        }

        //修改行程单状态
        function UpdateStatus() {
            var Ids=[];
            //行程单状态
            var TpStatus=$J("#selTripStatus").val();
            var RoleType=$J("#Hid_RoleType").val();
            var type=$J("#optype").val();
            var id=$J("#ids").val();
            if(type=="1") {
                $J("input[name='ck_ids'][type='checkbox']:visible").each(function (index,ck) {
                    if($J(ck).attr("checked")) {
                        Ids.push("'"+$J(ck).val()+"'");
                    }
                });
            } else {
                if(id!="") {
                    Ids.push("'"+id+"'");
                }
            }
            if(Ids.length>0&&RoleType=="1") {
                var url="../AJAX/GetHandler.ashx";
                var param={
                    Ids: escape(Ids.join(',')),
                    TpStatus: escape(TpStatus),
                    RoleType: escape(RoleType),
                    OpName: escape("UpdateTripStatus"),
                    currentuserid: "<%=this.mUser.id.ToString() %>",
                    num: Math.random()
                }
                $J.post(url,param,function (data) {
                    if(data!="") {
                        data=unescape(data);
                        var strArr=data.split('#######');
                        if(strArr.length>=2) {
                            showdg("aaa",0,strArr[1],"提示");
                        }
                    }
                },"text");
            }
        }

        function Edit(Id) {
            //$J("#status_"+Id).hide();

            $J("#show_Con"+Id).hide();
            $J("#hide_Con"+Id).show();

            $J("#show_TicketNum"+Id).hide();
            $J("#hide_TicketNum"+Id).show();

            $J("#show_TripStatus"+Id).hide();
            $J("#hide_TripStatus"+Id).show();



            return false;
        }
        function Cancel(Id) {

           // $J("#status_"+Id).show();

            $J("#show_Con"+Id).show();
            $J("#hide_Con"+Id).hide();

            $J("#show_TicketNum"+Id).show();
            $J("#hide_TicketNum"+Id).hide();

            $J("#show_TripStatus"+Id).show();
            $J("#hide_TripStatus"+Id).hide();

            return false;
        }
        function UpdateData(Id) {

            var RoleType=$J("#Hid_RoleType").val();//角色
            var TpStatus=$J("#selTripStatus_"+Id).val();//状态
            var TicketNum=$J("#txtTicketNum_"+Id).val();//票号

            var Ids=[];
            Ids.push(Id);
            if(Ids.length>0&&RoleType=="1") {
                var url="../AJAX/GetHandler.ashx";
                var param={
                    Ids: escape(Ids.join(',')),
                    TpStatus: escape(TpStatus),
                    RoleType: escape(RoleType),
                    TicketNum: escape(TicketNum),
                    OpName: escape("UpdateTripStatus"),
                    currentuserid: "<%=this.mUser.id.ToString() %>",
                    num: Math.random()
                }
                $J.post(url,param,function (data) {
                    if(data!="") {
                        data=unescape(data);
                        var strArr=data.split('#######');
                        if(strArr.length>=2) {
                            showdg("bbb",0,strArr[1],"提示");
                        }
                    }
                },"text");
            }
            return false;
        }
                 
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="title">
        <ul style="list-style-type: none; margin: 0 0; padding: 0 0; position: relative;">
            <li>行程单管理</li>
        </ul>
    </div>
    <div>
        <div class="contentbox">
            <table class="Search" cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <th>
                        领用公司名称：
                    </th>
                    <td>
                        <asp:TextBox ID="txtUseCpyName" runat="server" Width="120"></asp:TextBox>
                    </td>
                    <th>
                        领用公司编号：
                    </th>
                    <td>
                        <asp:TextBox ID="txtUseCpyNo" runat="server" Width="120"></asp:TextBox>
                    </td>
                    <th>
                        票号：
                    </th>
                    <td>
                        <asp:TextBox ID="txtTicketNumber" runat="server" Width="120"></asp:TextBox>
                    </td>
                    <th>
                        创建Office号：
                    </th>
                    <td>
                        <asp:TextBox ID="txtOffice" runat="server" Width="120"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th>
                        行程单号段：
                    </th>
                    <td colspan="3" align="left">
                        <%--  前6位：<asp:TextBox ID="txtStartCode" runat="server" MaxLength="6" Width="100px" CssClass="inputBorder"></asp:TextBox>--%>
                        <asp:TextBox ID="txtStartNum" runat="server" Width="120px" MaxLength="10" CssClass="inputBorder"></asp:TextBox>
                        <asp:TextBox ID="txtEndNum" runat="server" Width="120px" MaxLength="10" CssClass="inputBorder"></asp:TextBox>
                    </td>
                    <th>
                        入库时间：
                    </th>
                    <td colspan="3">
                        <input type="text" readonly="readonly" id="txtSDate" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                            style="width: 110px;" class="Wdate inputtxtdat" />-<input type="text" readonly="readonly"
                                id="txtEDate" runat="server" onfocus="WdatePicker({isShowWeek:false,dateFmt:'yyyy-MM-dd'})"
                                style="width: 110px;" class="Wdate inputtxtdat" />
                    </td>
                </tr>
                <tr>
                    <th>
                        落地运营商
                    </th>
                    <td>
                        <asp:DropDownList ID="ddlGY" runat="server" Width="300">
                        </asp:DropDownList>
                        <%--<asp:TextBox ID="txtOrderId" runat="server" Width="120"></asp:TextBox>--%>
                    </td>
                    <th>
                        状态:
                    </th>
                    <td>
                        <asp:DropDownList ID="ddlTripStatus" runat="server" Width="120">
                            <asp:ListItem Value="-1">所有</asp:ListItem>
                            <asp:ListItem Value="1">已入库,未分配</asp:ListItem>
                            <asp:ListItem Value="2">已分配,未使用</asp:ListItem>
                            <asp:ListItem Value="6">作废行程单,等待审核   </asp:ListItem>
                            <asp:ListItem Value="7">拒绝作废行程单,审核失败</asp:ListItem>
                            <asp:ListItem Value="8">空白回收,未分配 </asp:ListItem>
                            <asp:ListItem Value="9">已创建行程单</asp:ListItem>
                            <asp:ListItem Value="10">已作废行程单,审核成功</asp:ListItem>
                            <asp:ListItem Value="11">空白回收,已分配</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <th>
                    </th>
                    <td>
                    </td>
                    <th>
                    </th>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td colspan="8" style="text-align: center;">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnQuery" runat="server" Text=" 查 询 " OnClick="btnQuery_Click" /></span>
                        <span class="btn btn-ok-s">
                            <asp:Button ID="Button1" runat="server" Text=" 批量修改行程单状态 " OnClientClick="ShowEdit(1,1);return false;" /></span>
                    </td>
                </tr>
            </table>
            <table width="100%" class="tb-all-trade">
                <thead>
                    <tr>
                        <th style="width: 3%;">
                            <input type="checkbox" id="ckAll" name="ckAll" onclick="setSelAll(this)" />
                        </th>
                        <th style="width: 8%;">
                            运营商公司名称
                        </th>
                        <th style="width: 5%;">
                            行程单领用公司
                        </th>
                        <th style="width: 8%;">
                            行程单号
                        </th>
                        <th style="width: 8%;">
                            票号
                        </th>
                        <th style="width: 3%;">
                            创建Office
                        </th>
                        <th style="width: 8%;">
                            状态
                        </th>
                        <th style="width: 8%;">
                            创建时间
                        </th>
                        <th style="width: 8%;">
                            作废时间
                        </th>
                        <th style="width: 8%;">
                            入库时间
                        </th>
                        <th style="width: 8%;">
                            领用时间
                        </th>
                        <th style="width: 8%;">
                            操作
                        </th>
                    </tr>
                </thead>
                <asp:Repeater runat="server" ID="repList">
                    <ItemTemplate>
                        <tr onmouseover="this.bgColor='#F5F5F5';" onmouseout="this.bgColor='#ffffff';">
                            <th style="width: 8%;">
                                <input type="checkbox" id="ck_sel" name="ck_ids" value='<%# Eval("Id")%>' />
                            </th>
                            <td>
                                <%# Eval("OwnerCpyName")%>
                            </td>
                            <td>
                                <%# Eval("UseCpyName")%>
                            </td>
                            <td>
                                <%# Eval("TripNum")%>
                            </td>
                            <td>
                                <span id='show_TicketNum<%# Eval("Id")%>'>
                                    <%# Eval("TicketNum")%></span> <span id='hide_TicketNum<%# Eval("Id")%>' class="hide">
                                        <input type="text" id='txtTicketNum_<%# Eval("Id")%>' value='<%# Eval("TicketNum")%>' />
                                    </span>
                            </td>
                            <td>
                                <%# Eval("CreateOffice")%>
                            </td>
                            <td>
                                <span id='show_TripStatus<%# Eval("Id")%>'>
                                    <%# ShowText(0, Eval("TripStatus"))%></span>
                                    
                                     <span id='hide_TripStatus<%# Eval("Id")%>'
                                        class="hide">
                                        <%# ShowText(2, Eval("Id"),Eval("TripStatus"))%>
                                    </span>
                            </td>
                            <td>
                                <%# ShowText(1, Eval("PrintTime"))%>
                            </td>
                            <td>
                                <%# ShowText(1, Eval("InvalidTime"))%>
                            </td>
                            <td>
                                <%# ShowText(1, Eval("AddTime"))%>
                            </td>
                            <td>
                                <%# ShowText(1, Eval("UseTime"))%>
                            </td>
                            <td>
                                <span id="show_Con<%# Eval("Id")%>"><a href="#" onclick="return Edit('<%# Eval("Id")%>');">
                                    编辑</a></span> <span id="hide_Con<%# Eval("Id")%>" class="hide"><a href="#" onclick="return UpdateData('<%# Eval("Id")%>')">
                                        修改</a> <a href="#" onclick="return Cancel('<%# Eval("Id")%>');">取消</a> </span>
                                <span id='status_<%# Eval("Id")%>' class="hide"><a href="#" onclick="ShowEdit(0,'<%# Eval("Id")%>')">
                                    修改行程单状态</a></span>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <input type="hidden" id="Hid_PageSize" runat="server" value="50" />
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" HorizontalAlign="Left" CssClass="paginator"
                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="5" PagingButtonSpacing="3px"
                PrevPageText="上一页" ShowInputBox="Always" OnPageChanging="AspNetPager1_PageChanging"
                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第" Width="100%">
            </webdiyer:AspNetPager>
        </div>
    </div>
    <%--登录用户角色--%>
    <input type="hidden" id="Hid_RoleType" runat="server" />
    </form>
</body>
</html>

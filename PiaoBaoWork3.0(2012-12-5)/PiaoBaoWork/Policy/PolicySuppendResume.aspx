<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PolicySuppendResume.aspx.cs"
    Inherits="Policy_PolicySuppendResume" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>政策挂起解挂</title>
    <link type="text/css" href="../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <link href="../css/table.css" rel="stylesheet" type="text/css" />
    <link type="text/css" href="../js/Tooltip/Tooltip.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
        .wd250
        {
            width: 250px;
        }
        .wd100
        {
            width: 100px;
        }
         .wd150
        {
            width: 150px;
        }
        .green
        {
            color: Green;
        }
        .red
        {
            color: Red;
        }
    </style>
    <script type="text/javascript">

        //对话框包含处理
        function showdialog(t,param) {
            $("#divshow").html(t);
            $("#divshow").dialog({
                title: '提示',
                bgiframe: true,
                height: 180,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                close: function () {

                },
                buttons: {
                    '确定': function () {
                        if(param!=null&&param.op!=null) {
                            if(param.op==0) {
                                history.go(-1);
                            } else if(param.op==1) {
                                $(this).dialog('close');
                                $("#btnQuery").click();
                            }
                        } else {
                            $(this).dialog('close');
                        }
                    }
                }
            });
        }
        function SetHid(obj) {
            var val=$(obj).val().split('@@');
            if(val.length==3) {
                $("#Hid_CpyNo").val(val[0]);
                $("#Hid_LoginName").val(val[1]);
                $("#Hid_UserName").val(val[2]);
            }
        }
        function B_BAll(obj) {
            $("input[type='checkbox'][name='b2b_air']").attr("checked",obj.checked);
        }
        function B_SAll(obj) {
            $("input[type='checkbox'][name='bsp_air']").attr("checked",obj.checked);
        }
        function SelAll(obj) {
            $("input[type='checkbox'][name='b2b_air']").attr("checked",obj.checked);
            $("input[type='checkbox'][name='bsp_air']").attr("checked",obj.checked);
        }
        function UpdatePolicy(param) {
            if(param!=null) {
                var val_CpyNo=$.trim($("#Hid_CpyNo").val());
                //1挂起 0解挂
                var val_opType=param.opType;
                var val_LoginName=$("#Hid_LoginName").val();
                var val_UserName=$("#Hid_UserName").val();
                var val_UserRoleType=$("#Hid_UserRoleType").val();
                var b2bArr=[];
                var bsbArr=[];
                $("input[type='checkbox'][name='b2b_air']:checked").each(function (index,ck) {
                    b2bArr.push("'/"+$(ck).val()+"/'");
                });
                $("input[type='checkbox'][name='bsp_air']:checked").each(function (index,ck) {
                    bsbArr.push("'/"+$(ck).val()+"/'");
                });
                if(parseInt(val_UserRoleType,10)>3) {
                    showdialog("您没有进入该页面的权限！",{ op: 0 });
                }
                else if($.trim(val_CpyNo)=="") {
                    showdialog("页面已失效,请重新登录！");
                }
                else if(b2bArr.length==0&&bsbArr.length==0) {
                    showdialog("请选择需要挂起解挂的航空公司！");
                } else {
                    var url="../AJAX/CommonAjAx.ashx";
                    //b2bArr.push("'/ALL/'");
                    //bsbArr.push("'/ALL/'");
                    var param=
                    {
                        SupType: escape(val_opType),//1挂起 0解挂
                        CpyNo: escape(val_CpyNo),
                        b2b: escape(b2bArr.join(",")),
                        bsp: escape(bsbArr.join(",")),
                        LoginName: escape(val_LoginName),
                        UserName: escape(val_UserName),
                        RoleType: escape(val_UserRoleType),
                        IsWhere: escape("0"),//是否根据条件挂起解挂 否不是  而是航空公司
                        //-------------------------
                        OpType: escape("2"),
                        OpPage: escape("PolicySuppendResume.aspx"),
                        OpFunction: escape("Suppend"),
                        currentuserid:"<%=this.mUser.id.ToString() %>",
                        num: Math.random()
                    };
                    $.post(url,param,function (data) {
                        var strReArr=data.split('##');
                        if(strReArr.length==3) {
                            //错误代码
                            var errCode=strReArr[0];
                            //错误描述
                            var errDes=strReArr[1];
                            //错误结果
                            var result=$.trim(unescape(strReArr[2]));
                            if(errCode=="1") {
                                showdialog(errDes,{ op: 1 });
                            } else {
                                showdialog(errDes);
                            }
                        } else {
                            showdialog("页面出错了，请重新登录！");
                        }
                    },"text");

                }
            }
            return false;
        }
        $(function () {
            var CpyNo=$("#Hid_CpyNo").val();
            $("select[id='SelGY'] option[value*='"+CpyNo+"']").attr("selected",true);
        });

        function GoBack() {
            history.go(-1);
        }
    </script>
    <script type="text/javascript" src="../js/My97DatePicker/WdatePicker.js"></script>
</head>
<body>
    <div id="divshow">
    </div>
    <form id="form1" runat="server">
    <div>
        <div class="title">
            <ul style="list-style-type: none; margin: 0 0; padding: 0 0; position: relative;">
                <li>
                    <asp:Label ID="lblShow" Text="政策挂起解挂" runat="server" />
                </li>
            </ul>
        </div>
        <div>
            <table style="width: 100%; border-collapse: collapse;" border="1">
                <tr>
                    <th align="rigth" class="wd150">
                        类型
                    </th>
                    <th align="center">
                        航空公司
                    </th>
                </tr>
                <tr id="TRGY" runat="server" visible="false">
                    <th align="rigth" class="wd150">
                        供应或者落地运营商:
                    </th>
                    <td>
                        <asp:Literal ID="literGY" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <th align="rigth" class="wd150">
                        b2b政策
                    </th>
                    <td>
                        <asp:Literal ID="B2BSuppendBox" runat="server"></asp:Literal>
                        <br />
                        <label for="ck_b2b">
                            <input id="ck_b2b" type="checkbox" onclick="B_BAll(this)" />全选b2b</label>
                    </td>
                </tr>
                <tr>
                    <th align="rigth" class="wd150">
                        bsp政策
                    </th>
                    <td>
                        <asp:Literal ID="BSPSuppendBox" runat="server"></asp:Literal>
                        <br />
                        <label for="ck_bsp">
                            <input id="ck_bsp" type="checkbox" onclick="B_SAll(this)" />全选bsp</label>
                    </td>
                </tr>
                <tr>
                    <td class="leftTd">
                        &nbsp;
                    </td>
                    <td align="center">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnSuppend" runat="server" Text="挂起政策" OnClientClick="return UpdatePolicy({opType:1});" />
                        </span><span class="btn btn-ok-s">
                            <asp:Button ID="btnResume" runat="server" Text="解挂政策" OnClientClick="return UpdatePolicy({opType:0});" />
                        </span>
                        <%--<span class="btn btn-ok-s">
                            <asp:Button ID="btn" runat="server" OnClientClick="return GoBack();" Text="返回" />
                        </span>--%>
                        <label for="ck_all">
                            <input id="ck_all" type="checkbox" onclick="SelAll(this)" />全选</label>
                    </td>
                </tr>
            </table>
            <p>
                &nbsp;</p>
            <div class="title">
                <ul style="list-style-type: none; margin: 0 0; padding: 0 0; position: relative;">
                    <li>
                        <asp:Label ID="Label1" Text="政策挂起解挂操作日志" runat="server" />
                    </li>
                </ul>
            </div>
            <table id="tb-all-trade1" class="tb-all-trade" border="1" cellspacing="0" cellpadding="0"
                width="100%">
                <tr>
                    <th>
                        操作账号
                    </th>
                    <td>
                        <asp:TextBox ID="txtLoginName" runat="server" MaxLength="50" style="width:150px;"></asp:TextBox>
                    </td>
                    <th>
                        操作人姓名
                    </th>
                    <td>
                        <asp:TextBox ID="txtUserName" runat="server" MaxLength="50" style="width:150px;"></asp:TextBox>
                    </td>
                    <th>
                        操作类型
                    </th>
                    <td>
                        <asp:DropDownList ID="ddlopType" runat="server">
                            <asp:ListItem Value="">--全部--</asp:ListItem>
                            <asp:ListItem Value="政策解挂">解挂</asp:ListItem>
                            <asp:ListItem Value="政策挂起">挂起</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <th>
                        操作日期
                    </th>
                    <td>
                        <input type="text" id="txtStart" style="width: 130px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />至
                        <input type="text" id="txtEnd" style="width: 130px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:true,isShowWeek:false,dateFmt:'yyyy-MM-dd'})" />
                    </td>
                </tr>
                <tr>
                    <td colspan="8" align="center">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btnQuery" runat="server" Text="政策挂起解挂操作日志" OnClick="btnQuery_Click" />
                        </span><span class="btn btn-ok-s">
                            <asp:Button ID="btnReset" runat="server" Text="清空" OnClick="btnReset_Click" />
                        </span><span id="span_msg" runat="server" visible="false"><font class="red">* 请选择供应或者落地运营商后查询</font></span>
                    </td>
                </tr>
            </table>
            <table id="tb-all-trade" class="tb-all-trade" border="1" cellspacing="0" cellpadding="0"
                width="100%">
                <tr>
                    <th>
                        操作账号
                    </th>
                    <th>
                        操作人姓名
                    </th>
                    <th>
                        操作类型
                    </th>
                    <th>
                        内容
                    </th>
                    <th>
                        操作日期
                    </th>
                </tr>
                <asp:Repeater ID="Repeater" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td class="wd100">
                                <%# Eval("LoginName")%>
                            </td>
                            <td class="wd100">
                                <%# Eval("UserName")%>
                            </td>
                            <td class="wd100">
                                <%# ShowText(0,Eval("OperateType"))%>
                            </td>
                            <td title='<%# Eval("OptContent") %>'>
                                <%# SubChar(Eval("OptContent"),100,"...")%>
                            </td>
                            <td class="wd100">
                                <%# DataBinder.Eval(Container.DataItem,"CreateTime","{0:yyyy-MM-dd<br />HH:mm:ss}")%>
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
    <%--公司编号--%>
    <input id="Hid_CpyNo" type="hidden" runat="server" />
    <input id="Hid_LoginName" type="hidden" runat="server" />
    <input id="Hid_UserName" type="hidden" runat="server" />
    <input id="Hid_UserRoleType" type="hidden" runat="server" />
    <script type="text/javascript" src="../js/Tooltip/ToolTip.js"> </script>
    <script type="text/javascript">
        initToolTip("td");
    </script>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QueryBatch.aspx.cs" Inherits="Pay_BatchPay_QueryBatch" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../../css/table.css" rel="stylesheet" />
    <link type="text/css" href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" />
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../js/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-ui-1.8.13.custom.min.js"></script>
    <style type="text/css">
       .ui-state-default{height:30px; color:Black;}
       #dialog{ display:none;}
       #dialog table{ border-collapse:collapse; width:100%;}
       #dialog table tr td{ vertical-align:middle; height:26px; line-height:26px;}
       #dialog table tr td.left{ text-align:right;}
       .input
        {
            border: 1px solid #e2e2e2;
            background: #fff;
            color: #333;
            padding: 3px;
            width: 150px;
        }
       #dialog input[type='text']:focus 
        {
            border: 1px solid #7ac0da;
        }
       #dialog div.content
        {
            border-top:#aaaaaa 1px solid;
            margin:5px 0px;
            padding:5px;
            text-align:right;
        }
       #dialog span
        {
           cursor:pointer;
         }
       #dialog  tr.bottomline
         {
             border-bottom:#aaaaaa 1px dashed;
         }
       #dialog  .validateTips { border: 1px solid transparent; padding: 0.1em; margin:5px 0px; }
    </style>
    <script type="text/javascript">
        $(function () {
            $("#btn_clear,#btn_pay,#btn_query").button();
            $("#confirmDialog").dialog({
                autoOpen: false,
                width: 250,
                height: 120,
                modal: true,
                resizable: false,
                buttons: {
                    '支付完成': function () {
                        $("#dialog").dialog("close");
                        $("#confirmDialog").dialog("close");
                        $("#btn_query").click();
                    },
                    '重新支付': function () {
                        $("#confirmDialog").dialog("close");
                    }
                }
            });
            $("#dialog").dialog({
                autoOpen: false,
                width: 900,
                height: 350,
                modal: true,
                draggable: true,
                resizable: true,
                buttons: {
                    '确定付款': function () {
                        var bValid = true;
                        $("#dialog :text").each(function () {
                            $(this).removeClass("ui-state-error");
                            var text = $(this);
                            bValid = bValid && checkRequired(text);
                            if (text.hasClass("money")) {
                                bValid = bValid && checkRegexp(text, /^[0-9]+(\.[0-9]{2})?$/);
                            }

                        })
                        if (bValid) {
                            $("#confirmDialog").dialog("open");
                            $("#dialogForm").submit();
                        }
                    },
                    '取消': function () {
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    $("#dialog :text").val("").removeClass("ui-state-error");
                }
            });
            function updateTips(t) {
                $(".validateTips").text(t).addClass("ui-state-highlight");
                setTimeout(function () {
                    $(".validateTips").removeClass("ui-state-highlight", 1500)
                }, 500);
            }
            function checkRegexp(o, regexp) {
                if (!(regexp.test(o.val()))) {
                    o.addClass("ui-state-error");
                    updateTips('此项输入格式有错误');
                    return false;
                } else {
                    return true;
                }
            }
            function checkRequired(o) {
                if (o.val().length == 0) {
                    o.addClass("ui-state-error");
                    updateTips('此项必须输入!');
                    return false;
                } else {
                    return true;
                }
            }
            //添加项
            $("#btn_add").click(function () {
                $("#table").append($("#cloneTemplate > table > tbody > tr").clone());
            }).click();
            //删除项
            $("span.btn_remove").live("click", function (e) {
                var count = $("#table tr").length;
                if (count > 4) {
                    $(e.target).closest("tr").remove();
                }
            });
            $("#btn_pay").click(function (evt) {
                $("#dialog").dialog("open");
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="title">
        <span>转款查询</span>
        </div>
        <div id="tabs-1">
            <div class="c-list-filter">
                <div class="container" style="padding-bottom: 10px;">
                    <table  id="moreSearch" cellspacing="0" cellpadding="0" border="0" width="100%">
                <tr>
                    <td>付款帐号:</td>
                    <td>
                        <asp:TextBox ID="txt_payNum" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                    </td>
                    <td>收款帐号:</td>
                    <td>
                        <asp:TextBox ID="txt_payeeNum" CssClass="inputtxtdat" runat="server"></asp:TextBox>
                    </td>
                    <td>支付状态:</td>
                    <td>
                        <asp:DropDownList ID="drop_list" runat="server">
                            <asp:ListItem Value="" Text="--请选择--"></asp:ListItem>
                            <asp:ListItem Value="0" Text="等待付款"></asp:ListItem>
                            <asp:ListItem Value="1" Text="付款成功"></asp:ListItem>
                            <asp:ListItem Value="2" Text="付款失败"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>打款时间:</td>
                    <td>
                        <asp:TextBox ID="txt_StartTime" runat="server" CssClass="inputtxtdat"  onfocus="WdatePicker({isShowClear:true})"></asp:TextBox>--
                        <asp:TextBox ID="txt_EndTime" runat="server" CssClass="inputtxtdat" onfocus="WdatePicker({isShowClear:true})"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center;" colspan="6">
                        <asp:Button ID="btn_query" ClientIDMode="Static" runat="server" Text="查 询" 
                            onclick="btn_query_Click" />
                        <input type="button" id="btn_pay" value="我要付款" />
                        <input type="reset" value="清空条件" id="btn_clear" />
                    </td>
                </tr>
            </table>
                </div>
            </div>
        </div>
        <div>
            <table id="tb-all-trade" class="tb-all-trade" border="0" cellspacing="0" cellpadding="0"
                width="100%">
                <colgroup>
                    <col width="6%" />
                    <col width="8%" />
                    <col width="8%" />
                    <col width="10%" />
                    <col width="10%" />
                    <col width="10%" />
                    <col width="10%" />
                    <col width="8%" />
                    <col width="30%" />
                </colgroup>
                <thead>
                    <tr>
                        <th>序号</th>
                        <th>操作员</th>
                        <th>支付金额</th>
                        <th>支付时间</th>
                        <th>批次号</th>
                        <th>付款帐号</th>
                        <th>收款帐号</th>
                        <th>支付状态</th>
                        <th>备注</th>
                    </tr>
                </thead>
            <tbody>
            <asp:Repeater ID="rp_list" runat="server">
                <ItemTemplate>
                        <tr>
                            <td><%# Eval("RowNum") %></td>
                            <td><%# Eval("UserName") %></td>
                            <td><%# Eval("PayMoney")%></td>
                            <td><%# Eval("PayTime") %></td>
                            <td><%# Eval("OutOrderId") %></td>
                            <td><%# Eval("PayAccount") %></td>
                            <td><%# Eval("GetAccount") %></td>
                            <td><%# Eval("Result") %></td>
                            <td title="<%# Eval("Remark") %>" style="overflow:hidden"><%# Eval("Remark") %></td>
                        </tr>
                </ItemTemplate>
            </asp:Repeater>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="9" align="right">
                       
                    </td>
                </tr>
            </tfoot>
            </table>
            <webdiyer:AspNetPager ID="Pager" runat="server" HorizontalAlign="Left" CssClass="paginator"
                                CurrentPageButtonClass="cpb" NextPageText="下一页" PageSize="15" PagingButtonSpacing="3px"
                                PrevPageText="上一页" ShowInputBox="Always" onpagechanged="Pager_PageChanged"
                                AlwaysShow="True" CustomInfoHTML="" CustomInfoSectionWidth="6%" EnableTheming="True"
                                EnableUrlRewriting="True" FirstPageText="首页" LastPageText="尾页" NavigationToolTipTextFormatString="到第{0}页"
                                ShowCustomInfoSection="Left" ShowNavigationToolTip="True" SubmitButtonClass="cpb"
                                SubmitButtonText="GO" TextAfterInputBox="页" TextBeforeInputBox="跳转到第">
          </webdiyer:AspNetPager>
            <div style="width:100%; color:Red;">
                <div style="font-size:14px;"><b>注意事项:</b></div>
                <div style="margin:10px; font-size:12px; padding:10px 30px;">
                    <ol>
                        <li>在“自由付款”过程中，"*"为必填项目；</li>
                        <li>“自由付款”中，要求填写的信息以在“支付宝”上的帐号为依据填写，请一定填写正确的支付宝信息，否则可能支付失败；</li>
                    </ol>
                </div>
          </div>
        </div>
    </form>
    <div id="dialog" title="支付宝批量付款">
        <form action="BatchPay.aspx" id="dialogForm" method="post" target="_blank">
        <input type="hidden" value='<%=this.mUser.id.ToString() %>' name="currentuserid" />
        <p class="validateTips">所有字段都必须填写</p>
        <table id="table">
                <colgroup>
                    <col width="105" />
                    <col width="165" />
                    <col width="140" />
                    <col width="165" />
                    <col width="70" />
                    <col width="165" />
                    <col width="24" />
                </colgroup>
                <tbody>
                <tr class="bottomline">
                    <td colspan="7">
                        付款信息
                    </td>
                </tr>
                <tr>
                    <td class="left"><font color="red">*</font>付款支付宝帐号：</td>
                    <td>
                        <input name="payNum" class="ui-widget-content input" type="text" />
                    </td>
                    <td class="left"><font color="red">*</font>付款方支付宝真实姓名：</td>
                    <td>
                        <input name="payName" class="ui-widget-content input" type="text" />
                    </td>
                    <td class="left"><font color="red">*</font>付款理由：</td>
                    <td>
                        <input name="payReason" class="ui-widget-content input" type="text" />
                    </td>
                    <td></td>
                </tr>
                <tr class="bottomline">
                    <td colspan="6">
                        收款信息
                    </td>
                    <td>
                        <span id="btn_add" class="ui-icon ui-icon-circle-plus" title="添加收款项目"></span>
                    </td>
                </tr>
                </tbody>
            </table>  
    </form>
    </div>
    <!--模版项-->
    <div id="cloneTemplate" style="display:none;">
        <table>
            <tr>
                <td class="left"><font color="red">*</font>收款支付宝账户：</td>
                <td>
                    <input name="payeeNum" class="ui-widget-content input" type="text" />
                </td>
                <td class="left"><font color="red">*</font>收款方账户真实姓名：</td>
                <td>
                    <input name="payeeName" class="ui-widget-content input" type="text" />
                </td>
                <td class="left"><font color="red">*</font>收款金额：</td>
                <td>
                    <input name="payeeAmount" class="ui-widget-content input money" type="text" />
                </td>
                <td>
                    <span class="btn_remove ui-icon ui-icon-circle-close" title="删除该项"></span>
                </td>
               </tr>
        </table>
    </div>
    <div id="confirmDialog" title="确认支付">
        是否支付完成？
    </div>
</body>
</html>

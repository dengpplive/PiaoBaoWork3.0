<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TimeCtrl.ascx.cs" Inherits="UserContrl_TimeCtrl" %>
<style type="text/css">
    .ddlWD
    {
        width: 45px;
    }
</style>
<asp:Literal ID="literScript" runat="server"></asp:Literal>
<div>
    <asp:DropDownList ID="ddlHour0" runat="server" class="ddlWD">
    </asp:DropDownList>
    <span id="span_hour0" runat="server">：</span><asp:DropDownList ID="ddlMinute0" runat="server"
        class="ddlWD">
    </asp:DropDownList>
    <span id="span_minute0" runat="server">：</span><asp:DropDownList ID="ddlSec0" runat="server"
        class="ddlWD">
    </asp:DropDownList>
    <span id="span_Char" runat="server">-</span><asp:DropDownList ID="ddlHour1" runat="server"
        class="ddlWD">
    </asp:DropDownList>
    <span id="span_hour1" runat="server">：</span><asp:DropDownList ID="ddlMinute1" runat="server"
        class="ddlWD">
    </asp:DropDownList>
    <span id="span_minute1" runat="server">：</span><asp:DropDownList ID="ddlSec1" runat="server"
        class="ddlWD">
    </asp:DropDownList>
</div>

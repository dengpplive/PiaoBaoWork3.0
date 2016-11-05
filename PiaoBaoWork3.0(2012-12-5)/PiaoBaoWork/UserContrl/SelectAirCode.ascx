<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectAirCode.ascx.cs"
    Inherits="UserControl_SelectAirCode" %>
<asp:Literal ID="scriptLiter" runat="server" EnableViewState="false"></asp:Literal>
<span><span id="airSpan" style="font-size: 12px;" runat="server">承运人: </span>
    <%--<asp:TextBox ID="txt_AirCo" runat="server" Width="30px" MaxLength="2" autocomplete="off"
        onpropertychange="setAirCode(this.value)" Style="margin-bottom: 2px; margin-top: -2px;"></asp:TextBox>--%>
    <asp:TextBox ID="txt_AirCo" runat="server" Width="30px" MaxLength="2" autocomplete="off"
        onkeyup="setAirCode(this.value)" Style="margin-bottom: 2px; margin-top: -2px;"></asp:TextBox>
    <asp:DropDownList ID="ddl_AirCode" runat="server" onchange="ddlSetText(this)">
    </asp:DropDownList>
    <asp:HiddenField ID="hf_AirCodeText" runat="server" />
    <asp:HiddenField ID="Hid_SelectIndex" Value="-1" runat="server" />
</span>
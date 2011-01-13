<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Render.aspx.cs" Inherits="WebNoSql.Render" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<iframe src="RenderGadget.ashx?gadget=<%=Request.QueryString["gadget"] %>" style="width:80%;height:500px;"></iframe>

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SampleWeb.Models.App>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <h3>App <%: Model.Name %></h3>

    <iframe src="/Gadget/Render/<%: Model.LatestGadgetID %>" style="background:#fff;width:90%; height:500px;" ></iframe>

</asp:Content>

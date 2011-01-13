<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<SampleWeb.Models.App>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Negroni Sample Apps</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: ViewData["Message"] %></h2>

    <hr />
    <table style="border:1px solid #c0c0c0;" >
    <thead>
    <tr><th>Name</th><th>URL</th></tr>
    </thead>
    <tbody>
    
    <%foreach(var a in ViewData.Model) { %>
    <tr>
    <td>
    <%: a.Name %>
    </td>
    <td><%: Html.ActionLink("Edit", "Edit") %>
    </td>
    </tr>
    <% } %>
    
    </tbody>
    </table>
</asp:Content>

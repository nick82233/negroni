<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<SampleWeb.Models.App>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Bite Me
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: ViewData["Message"] %></h2>
    <p>
        To learn more about ASP.NET MVC visit <a href="http://asp.net/mvc" title="ASP.NET MVC Website">http://asp.net/mvc</a>.
    </p>

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


    <h1>Start time: <%:ViewData["StartTime"]%></h1>
    <h1>Bind End: <%:ViewData["BindEnd"]%></h1>
    <h1>Finish time: <%:DateTime.Now.Ticks %></h1>
    <h1>Diff: <%:DateTime.Now.Ticks - (long)ViewData["StartTime"]%></h1>
</asp:Content>

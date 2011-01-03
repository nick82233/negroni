<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SampleWeb.Models.App>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">App List</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <table class="listTable">
        <tr>
            <th></th>
            <th>
                AppId
            </th>
            <th>
                Name
            </th>
            <th>
                SourceUrl
            </th>
            <th>
                ManifestUrl
            </th>
            <th>
                LatestGadgetID
            </th>
            <th>
                CreateDate
            </th>
            <th>
                UpdateDate
            </th>
        </tr>
        
    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%: Html.ActionLink("Edit", "Edit", new { id=item.AppId }) %> |
                <%: Html.ActionLink("Details", "Details", new { id=item.AppId })%> |
                <%: Html.ActionLink("Delete", "Delete", new { id=item.AppId })%>
            </td>
            <td>
                <%: item.AppId %>
            </td>
            <td>
                <%: item.Name %>
            </td>
            <td>
                <%: item.SourceUrl %>
            </td>
            <td>
                <%: item.ManifestUrl %>
            </td>
            <td>
                <%: item.LatestGadgetID %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.CreateDate) %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.UpdateDate) %>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%: Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>


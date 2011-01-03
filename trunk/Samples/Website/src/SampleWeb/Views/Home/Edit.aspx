<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SampleWeb.Models.App>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.AppId) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.AppId) %>
                <%: Html.ValidationMessageFor(model => model.AppId) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name) %>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.SourceUrl) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.SourceUrl) %>
                <%: Html.ValidationMessageFor(model => model.SourceUrl) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.ManifestUrl) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.ManifestUrl) %>
                <%: Html.ValidationMessageFor(model => model.ManifestUrl) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.LatestGadgetID) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.LatestGadgetID) %>
                <%: Html.ValidationMessageFor(model => model.LatestGadgetID) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CreateDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CreateDate, String.Format("{0:g}", Model.CreateDate)) %>
                <%: Html.ValidationMessageFor(model => model.CreateDate) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.UpdateDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.UpdateDate, String.Format("{0:g}", Model.UpdateDate)) %>
                <%: Html.ValidationMessageFor(model => model.UpdateDate) %>
            </div>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>


<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SampleWeb.Models.App>" %>

    <div class="editor-label">
        App ID: <%: Model.AppId %>
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
        <%: Html.TextBoxFor(model => model.SourceUrl, new {@style = "Width:400px"}) %>
        <a href="<%: Model.SourceUrl %>" target="_blank" >View URL</a>
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
        Created: <%: String.Format("{0:g}", Model.CreateDate)%><br />
        Last Update: <%: String.Format("{0:g}", Model.UpdateDate)%>
    </div>
            


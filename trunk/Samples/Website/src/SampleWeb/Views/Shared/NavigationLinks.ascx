<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
        <ul id="menu">              
            <li><%: Html.ActionLink("Home", "Index")%></li>
            <li><%: Html.ActionLink("About", "About", "Home")%></li>
        </ul>

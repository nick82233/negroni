<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebNoSql._Default" %>
<%@ Register TagPrefix="osml" Assembly="WebNoSql" 
        Namespace="WebNoSql"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    

    <osml:OsmlInlineControl runat="server" 
     ControlParserKey="sampleWeb"
    id="gadgetGrid" Width="250" Height="500" >
    <os:Var key="foo">
    {
    "color": "red",
    "number" : 2
    }
    </os:Var>
    <sample:GadgetList key="gadgets" />
    <h1 style="color:${foo.color};">Hello World ${foo.number}</h1>

    <ul>
    <li repeat="${gadgets}"><a href="Render.aspx?gadget=${Cur}">Render ${Cur}</a></li>
    </ul>
    </osml:OsmlInlineControl>
    

</asp:Content>

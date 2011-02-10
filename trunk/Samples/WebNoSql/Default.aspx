<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebNoSql._Default" %>
<%@ Register TagPrefix="neg" Assembly="Negroni" Namespace="Negroni"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <div style="float:right;width:350px;">
    <h2>Fetch an External Gadget</h2>

    <form method="post" action="GadgetFetcher.ashx">
    URL: <input type="text" style="width:300px;" name="source" value="http://negroni.googlecode.com/svn/trunk/ComplianceTests/OpenSocial_1.0/ExpressionLangSample.xml" />
    <br />
    <input type="submit" value="Go" />
    
    </form>
    </div>
    
    <!-- This control demonstrates inline Negroni gadget with private factory parser and data tags -->
    <neg:RenderInlineControl runat="server" 
     ControlParserKey="sampleWeb" BorderStyle="Groove" BorderWidth="2px" BorderColor="Aqua"
    id="gadgetGrid" Width="250" Height="300" >
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
    </neg:RenderInlineControl>
    
    <!-- This control demonstrates Negroni gadget rendered from an external server -->
    <h3>The below gadget is external</h3>
    <div style="padding:10px;background:#ffa;">
    <neg:RenderInlineControl ID="RenderInlineControl1" runat="server" ControlParserKey="gadget_v1.0"
    Src="http://negroni.googlecode.com/svn/trunk/ComplianceTests/OpenSocial_1.0/helloWorld.xml" />
    </div>

    <hr />
    <h3>Clock below</h3>
    <neg:RenderInlineControl ID="renderClock" runat="server" ControlParserKey="gadget_v1.0"
    Src="http://gadgets.presse-papiers.fr/clock/clock.xml" />

    

</asp:Content>

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Editor.aspx.cs" Inherits="WebNoSql.Editor" %>
<%@ Register TagPrefix="neg" Assembly="Negroni" Namespace="Negroni"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <h2>Create a Gadget</h2>

    <table style="width:100%;">
    <tr valign="top">
    <td>
    <div style="width:300px;">
    <form method="post" action="RenderGadget.ashx" target="renderFrame">
    <p>
    Parser: 
    <select name="parser">
    <asp:Literal ID="literalParsers" runat="server" />
    </select>
    </p>

    <p>
    Source: <br />
    <textarea name="source" cols="50" rows="30" style="background:#eee;width:300px;height:250px;"></textarea>
    </p>


    <input type="submit" value="Go" />
    
    </form>
    </div>
    </td>
    <td>
    <div style="width:400px;margin-left:15px;border:3px outset black;">
    <iframe id="renderFrame" name="renderFrame" src="RenderGadget.ashx" style="width:100%;height:450px;" ></iframe>

    </div>
    </td>
    </tr>
    </table>
    



    

</asp:Content>

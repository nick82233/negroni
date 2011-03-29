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

    <input style="float:right;" type="submit" value="Go" />

    <a href="javascript:loadSimpleGadget()" >Load Simple Gadget Format</a>
    <br />
    <a href="javascript:loadFullGadget()" >Load Old Gadget Format</a>
    <br />
    <textarea name="source" id="txtSource" cols="60" rows="80" style="background:#eee;width:350px;height:450px;"></textarea>
    </p>


    
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
    


<script type="text/javascript">
// <![CDATA[

    var simpleGadget = "<Module title='Simple Gadget Sample'> \n  <Data>\n  <os:Var key='someName'>Joe</os:Var>\n  </Data>\n  <Templates>\n    <Template tag='my:FancyName'>\n    <h2 style='font-family:comic sans ms;color:${My.color};'>Hello, ${My.name}</h2>\n    </Template>\n  </Templates>\n  \n  <Content>\n  Test simple tag <br />\n  <my:FancyName color='red' name='${someName}' />\n  <br />\n  <my:FancyName >\n   <color>blue</color>\n   <name>Chris</name>\n  </my:FancyName>\n  </Content>\n</Module>";

    var fullGadget = "&lt;Module&gt;\n  &lt;ModulePrefs title='Original Gadget Format' \n  description='Same gadget as in SimpleGadget, \n  but in the current 1.0 OpenSocial gadget format'  &gt; \n  \n  &lt;Require feature='opensocial-1.0' /&gt;\n  &lt;Require feature='opensocial-templates' /&gt;\n  \n  &lt;/ModulePrefs&gt;\n  \n  &lt;Content type='html'&gt;\n  &lt;script type='text/os-data'&gt;\n  &lt;os:Var key='someName'&gt;Joe&lt;/os:Var&gt;\n  &lt;/script&gt;\n  \n  &lt;script type='text/os-template' tag='my:FancyName'&gt;\n    &lt;h2 style='font-family:comic sans ms;color:${My.color};'&gt;Hello, ${My.name}&lt;/h2&gt;\n  &lt;/script&gt;\n  \n  &lt;script type='text/os-template' &gt;\n  Test simple tag &lt;br /&gt;\n  &lt;my:FancyName color='red' name='${someName}' /&gt;\n  &lt;br /&gt;\n  &lt;my:FancyName &gt;\n   &lt;color&gt;blue&lt;/color&gt;\n   &lt;name&gt;Chris&lt;/name&gt;\n  &lt;/my:FancyName&gt;\n  &lt;/script&gt;\n  &lt;/Content&gt;\n&lt;/Module&gt;";

    function loadSimpleGadget() {
        var el = document.getElementById("txtSource");
        el.value = simpleGadget;
    }
    function loadFullGadget() {
        var el = document.getElementById("txtSource");
        var re1 = new RegExp("&lt;", "g")
        var re2 = new RegExp("&gt;", "g")
        el.value = fullGadget.replace(re1, "<").replace(re2, ">");
    }
// ]]>    
</script>



    

</asp:Content>

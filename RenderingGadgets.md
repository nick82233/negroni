


# Introduction #

One of the major uses of Negroni is to dynamically execute and render content in the form of gadgets.  These may be OpenSocial gadgets conforming to the GadgetXML specification, or they may be gadgets built to utilize your custom parser.

A capability Negroni adds that is not available by working with user controls is to dynamically render gadgets supplied from external sources.  This allows your site to aggregate and render content from multiple vendors on the server-side.

# Rendering on a Page #

The simplest way to render gadgets is by dropping the **`RenderInlineControl`** onto the page surface and letting the standard ASPX page engine trigger rendering.  `RenderInlineControl` is supplied in the baseline Negroni package.  The markup to render can be directly authored inside the control, specified as a link to an external URI with the **`Src`** attribute, or set in code by assigning to the **`MarkupContent`** property.

## Adding the `RenderInlineControl` to a Page ##

`RenderInlineControl` is implemented as a standard `WebControl`.  To make it available for use you must register the Negroni assembly with the page.  Then you can add it to the markup just like any server control.

In the Sample project `WebNoSql` the Default.aspx page has registered and is using the `RenderInlineControl`.  The sample below registers the Negroni assembly with the page with the directive **`<%@Register TagPrefix="neg" Assembly="Negroni" Namespace="Negroni"%>`**.  Then later in the page an instance of the control is added with **`<neg:RenderInlineControl runat="server" ... `**.  The particular example is specifying to render from a source gadget on a different server.

```
<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebNoSql._Default" %>
<%@ Register TagPrefix="neg" Assembly="Negroni" Namespace="Negroni"%>

...

<neg:RenderInlineControl runat="server" ID="renderClock"
  ControlParserKey="gadget_v1.0" 
  Src="http://gadgets.presse-papiers.fr/clock/clock.xml" />

```

## Rendering Inline Content ##

Static content you with Negroni to render may be directly authored within the control.  In this way you can access data with declarative data controls and render with that data by just using markup!  There is no need to access the code editor.  Changing the data being used is simply a matter of updating the markup - no recompile is required.

The below example is from the Default page of the `WebNoSql` sample application.  In it we see two data tags being used - a static variable with os:Var and our custom list of gadgets constructed by reading the filesystem.  The **`<os:Var>`** tag is used to create a static data object with JSON syntax and registered under the key "foo".  Later it is accessed with a simple EL statement.  The **`<sample:GadgetList>`** tag is our custom data tag.  It returns an array of filenames.  A repeater later iterates over this list to display the results.


```
    <neg:RenderInlineControl runat="server" 
     ControlParserKey="sampleWeb" 
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
    <li repeat="${gadgets}">
<a href="Render.aspx?gadget=${Cur}">Render ${Cur}</a></li>
    </ul>
    </neg:RenderInlineControl>
```


## Render from External Source ##

Aggregating external dynamic content on the server-side is a powerful feature.  Using Negroni you can expose your internal data to an external party and allow them to build, host, and maintain features that will deeply integrated and rendered on your site.  The simplest way to accomplish this is to specify the **`Src`** attribute on a `RenderInlineControl` and point it to the location of the external gadget source.

The below example references one of the gadgets supplied as part of the compliance tests directly from the main Subversion repository.

```
  <neg:RenderInlineControl ID="renderHello" runat="server"
   ControlParserKey="gadget_v1.0" 
Src="http://negroni.googlecode.com/svn/trunk/ComplianceTests/OpenSocial_1.0/helloWorld.xml" />
```

**Notes on external sourced content:** Referencing externally sourced gadget content is a powerful feature, but you must take care to make your system robust.  The rendering of your page blocks on each externally sourced gadget.  That means if an external party's servers go down, it could cause execution of your page to hang.  In practice you should pre-resolve externally sourced gadgets to a local cache system under your control.  During execution, if the `RenderInlineControl` will not attempt to re-fetch from the `Src` URL if the `MarkupContent` property is already set.


# Rendering to Stream #

If all you have is a handle to the response stream, gadgets can also render directly to the steam and same a few string allocations.  Once you have had the `ControlFactory` initialize the appropriate `RootElementMaster` object for the gadget, simply pass a `TextWriter` to the `Render` method and you're off.  If you are using an OpenSocial gadget and the `GadgetMaster` object, it is recommended to use `RenderContent` instead, since this allows you to specify a view in a gadget that supports multiple views.


# Optimizing Rendering #

There are two primary pieces of data required to render a gadget:

  * `ControlFactory` parser key
  * Raw XML markup of gadget

Rendering with this information is fairly fast.  But you can make it faster for gadgets that don't have constantly changing markup code.  This is done by saving and re-using a third piece of data:

  * Parsed offset string

Whenever a gadget is initialized the control factory must construct the entire control tree.  The markup must be parsed to identify all the controls and properly initialize them.  This is done with the help of an `XMLReader`.  For very large markup blocks with lots and lots of elements, this can get slow.  Negroni speeds this process up by allowing the positional offsets of all controls to be stored and loaded from an offset string.  With the offsets pre-calculated, the `XMLReader` is bypassed and the control tree is constructed by string chunking the markup.

In practice, this optimization means very little for small/trivial gadgets, but can make a considerable difference for very large gadgets with lots of elements.  If my computer science doesn't fail me, it is a growth of (O)N versus (O)logN for pre-parsed gadgets, where N is the number of elements in the markup (someone should check my work).  In practical terms, using pre-calculated offsets can translate to a 30%-40% performance improvement on gadgets of roughly 1200 lines of code.


# Introduction #

Negroni can be used to build simple control parsers for reading XML configuration files, XML renderers, or highly complicated and constrained control structures.  It is also tuned to consuming data from multiple sources, both internal and across the web, and accessing and utilizing that information for dynamic rendering and data processing.

This page will deal primarily with building a control parser.

# Parser Basics #

Parsing of an XML file is managed by a **Control Factory**.  Control Factories maintain a catalog of controls, which have one or more markup tags associated with them.  Control factories are defined in a configuration file.  This configuration file also identifies which assemblies contain the controls that will be loaded into the control factory.  In this way you can mix and match control assemblies to create specialized parsers or extend existing parsers to add new tags and functionality.

Controls are organized into **Context Groups**. Context Groups restrict the parsing context for which a particular control is valid and recognized.  There is always one default context group controls are placed inside when a specific context group is not specified.

It is good practice to specify one **Root Element** to register with a control factory.  The root element is the element node expected to be the first node in a parsed control tree.  You may specify multiple Root Elements, but they will share controls and context groups between them.

# Creating a Parser #

For this exercise we will reference the _colorParser_ defined in the **`WebNoSql`** sample project.  This is included under the _**`Samples/WebNoSql/`**_ directory in the source tree or redistributable package.

The parser will accept markup like below and output appropriate markup to display blocks of color.

```
<colors>
 <rainbow></rainbow> 
 <green/>
</colors>
```

## Defining Controls ##

Controls are defined using an attribute tagging syntax.  Negroni parsing attributes are defined in the `Negroni.TemplateFramework` namespace.

### Root Element ###

The first control we will define will be the `<colors>` root element.  An element is defined by adding the **`MarkupTag`** attribute on the class and specifying the literal tag(s) to recognize.  The colors element is identified as a root element by additionally adding the **`RootElement`** attribute and inheriting from the `RootElementMaster` base class.

```
using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
  [MarkupTag("colors")]
  [RootElement]
  public class Colors : RootElementMaster
  {
  }
}
```

### Rendered Element Controls ###

Control tags are also identified by using the `MarkupTag` attribute and must inherit from one of the base control classes.  In the case of the _`<rainbow>`_ element, we will inherit from the simplest control - `BaseGadgetControl`.  Because this is a rendered control, we must override the **Render** method.

```
using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
  [MarkupTag("rainbow")]
  public class RainbowControl : BaseGadgetControl
  {
    public override void Render(System.IO.TextWriter writer)
    {
      string divTemplate = "<div style='width:50px;float:left;height:30px;background:{0};'>&nbsp;</div>";
      writer.Write(String.Format(divTemplate, "red"));
      writer.Write(String.Format(divTemplate, "orange"));
      writer.Write(String.Format(divTemplate, "yellow"));
      writer.Write(String.Format(divTemplate, "green"));
      writer.Write(String.Format(divTemplate, "blue"));
      writer.Write(String.Format(divTemplate, "violet"));

    }
  }
}
```

### Element Container Controls ###

Sometimes you may wish for control elements to contain other parsed control elements.  This is accomplished by defining a markup control that inherits from **`BaseContainerControl`**.  This control automatically renders all child controls in the **`Controls`** element collection, so you don't even have to implement your own Render method.  In this case, we're defining a `<box>` element that displays a border around it's contents, so we will have an implementation of Render.

```
using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
  [MarkupTag("box")]
  public class BoxControl : BaseContainerControl
  {
    public override void Render(System.IO.TextWriter writer)
    {
      writer.Write("<div style='border:3px solid red;margin:2px;padding:4px;'>\n");
      base.Render(writer);
      writer.Write("</div>\n");
    }
  }
}
```

## Registering the Control Factory ##

Once your parsing controls are defined in an assembly you must register them with the Negroni framework under a control factory key.  This key is used to identify the control set to use when parsing a block of XML.  Control factories are registered by adding a **`<NegroniControlFactory>`** element to the **`NegroniFramework.config`** file.  Under this element you then add an **`<assembly>`** element for each assembly that contains your desired controls.

In the **`WebNoSql`** sample project, you can open the `NegroniFramework.config` file from the web root and see the definition of the **`colorParser`** control factory.  You may also notice the **exclude** attribute.  The configuration format supports both an **`include`** and **`exclude`** attribute to more granularly add or remove individual or groups of controls from an assembly.


```
<?xml version="1.0" encoding="utf-8"?>
<NegroniControlFactories>
...
  <NegroniControlFactory key="colorParser">
    <assembly name="WebNoSql" exclude="GadgetFileList" />
  </NegroniControlFactory>
</NegroniControlFactories>
```


# Rendering #

Rendering may be handled in two ways: in code with a response stream or in page markup with an ASP.Net web control. **Note:** Not all parsed control trees must be rendered, but rendered control trees is a primary usage of Negroni. Negroni uses its self to parse its configuration file, for instance.

As we saw before, every control has a Render method that accepts a **`System.IO.TextWriter`**.  Once a control tree is constructed, you simply invoke the **`Render`** method on the root element and it and all child controls will render.

Sometimes it's not convenient to get a handle on the output stream or you simply want to drop literal markup for rendering on a page.  Negroni includes a bundled `WebControl` implementation with the **`RenderInlineControl`**.

Register the **`RenderInlineControl`** control with the page object and you can use it to drop parsed content directly on the page.  An example of this can be seen with the rainbow banner shown with the **`WebNoSql`** sample project.  Look in the **`Site.Master`** page to see it in action.  The control only needs the parsing control factory key and the markup.  It takes care of the rest.

```
<%@ Register TagPrefix="neg" Assembly="Negroni" Namespace="Negroni"%>
...

<neg:RenderInlineControl runat="server" 
     ControlParserKey="colorParser" 
    id="gadgetColor" >
<colors>
 <rainbow></rainbow>
</colors>
</neg:RenderInlineControl>
```


You might be saying to yourself "this is all well and good, but there's already a component model in ASP.Net"  That is true, but the difference is that those only allow compiled widget components.  Negroni executes components at runtime, so the content can change more dynamically.  It can even consume and execute components from external servers.
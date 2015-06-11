


# Introduction #

One of the most useful aspects of OpenSocial and the Negroni framework is the ability to define declarative data tags and use them in rendering and processing.  Data tags declare in markup the piece of data you wish to retrieve and the key to place it under in the data context.  This document looks at some basic mechanisms for building data controls


# Data Controls #

Data controls are created in much the same way as other controls.  They have a **`MarkupTag`** attribute to identify the tag to parse.  They also inherit from one of the base controls.  Data controls should all inherit from **`Negroni.TemplateFramework.BaseDataControl`**.  This tags them as a recognized data control to the Negroni framework.

## Data Resolution ##

Data resolution may be handled either inline during the Render phase, or prior to render with an **`IDataPipelineResolver`** implementation.  Using a data pipeline resolver adds more complexity to your data controls, but it allows for greater control of how and when the data is fetched.

When using the simpler inline render resolution, your widget authors must include the data tag prior to any usages of the data.  When using a data pipeline resolver, the data tags can appear anywhere since there is an entire data resolution pass prior to the render pass.

## Creating a Data Control ##

In the _WebNoSql_ sample project, the site its self makes use of data controls to display the list of available gadgets.  Our data control loads the file list into the DataContext.  Then the Negroni widget uses a simple repeater to generate links for all available gadgets.

The first step is to define the basic control class.  We define our data control to be recognized under the tag **sample:GadgetList**

```
using System;
using System.Collections.Generic;
using System.IO;

using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace WebNoSql.SiteOSML
{
  [Negroni.TemplateFramework.MarkupTag("sample:GadgetList")]
  public class GadgetFileList : BaseDataControl
  {
  }
}

```

Notice how we inherit from **`BaseDataControl`** instead of `BaseGadgetControl` or `BaseContainerControl`.  That allows Negroni to recognize this as a data control and register it with the widget's DataContext during parsing.

The next step is to implement the code to fetch the data.  We're not using a pipeline resolver, but its still good practice to tie data resolution to the `InvokeTarget()` method in case you want to switch from render-time resolution to pipeline resolution prior to render.  This code also calls `InvokeTarget` from within the render method, using a flag value to insure it's not called multiple times.

The data is a listing of all XML files found in a reserved folder of the app.



```
using System;
using System.Collections.Generic;
using System.IO;

using Negroni.TemplateFramework;
using Negroni.DataPipeline;


namespace WebNoSql.SiteOSML
{
  [Negroni.TemplateFramework.MarkupTag("sample:GadgetList")]
  public class GadgetFileList : BaseDataControl
  {
    public const string GADGET_DIRECTORY = "Gadgets";

    private List<string> _gadgetList = null;

    public List<string> GadgetFiles
    {
      get {
        if (_gadgetList == null)
        {
          string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GADGET_DIRECTORY);
          
          string[] foo = Directory.GetFiles(path, "*.xml");

          _gadgetList = new List<string>();
          int len = path.Length;
          for (int i = 0; i < foo.Length; i++)
          {
            if (!string.IsNullOrEmpty(foo[i])
              && foo[i].Length + 1 >= len)
            {
              _gadgetList.Add(foo[i].Substring(len + 1));
            }
          }

        }
        return _gadgetList; 
      }
    }

    bool wasInvoked = false;

    public override void InvokeTarget()
    {
      if (!wasInvoked)
      {
        this.MyDataContext.RegisterDataItem(this.Key, this.GadgetFiles);
        wasInvoked = true;
      }
    }

    public override void Render(System.IO.TextWriter writer)
    {
      InvokeTarget();
      //base.Render(writer);
    }
  }
}
```

## Using a Data Control ##

To see this control in action, open the **Default.aspx** page (markup, not code) and view the contents of the inline control `<neg:RenderInlineControl >`.  The simplified code listing below shows the data tag and how a repeater and EL statements are used to build the links.

```
  <sample:GadgetList key="gadgets" />
  <ul>
  <li repeat="${gadgets}">
    <a href="Render.aspx?gadget=${Cur}">Render ${Cur}</a></li>
  </ul>
```

The `GadgetList` registers its result under the key **gadgets** with the DataContext.  Next you see the li element using an attribute repeater and accessing the collection with the EL statement **`${gadgets}`**.  Repeaters are akin to a foreach statement.  Attribute-based repeaters cause the containing element to be dynamically evaluated and emitted for each item in a collection.  The current item is accessed via the special **`${Cur}`** variable.

# Data #

## Allowed Data and EL resolution ##

Data controls are very flexible in what they can consume.  Just about any type of object, collection, or JSON string can be consumed and exposed via EL statements (note: anonymous objects have not yet been tested).

When a new object type is registered, the DataContext automatically wraps it in a `GenericExpressionEvalWrapper`.  The system reflects on each type only once and maintains a store of property and field invokers that the EL interpreter uses to dynamically access the values.  In cases where you want to control what and how an object's properties are exposed via the EL, you must implement an `IExpressionEvaluator` to wrap your underlying object.
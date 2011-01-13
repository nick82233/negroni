using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Negroni.DataPipeline;
using Negroni.OpenSocial;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.TemplateFramework;


namespace WebNoSql
{
	[ToolboxData("<{0}:OsmlInlineControl runat=\"server\" ></{0}:OsmlInlineControl>")]
	[ParseChildren(true, "GadgetContent")]
	public class OsmlInlineControl : System.Web.UI.WebControls.WebControl
	{
		const string DEFAULT_CONTROL_PARSER = "gadget_v1.0";

		public OsmlInlineControl()
		{
			//min width/height
			Width = 250;
			Height = 300;
			Surface = "canvas";
			ShowBorder = true;
		}

		private int _applicationId = -1;
		/// <summary>
		/// ID of the application
		/// </summary>
		[Bindable(true)]
		public int ApplicationId
		{
			get
			{
				return _applicationId;
			}
			set
			{
				_myGadgetMaster = null;
				_applicationId = value;
			}
		}

		/// <summary>
		/// Surface name.  Canvas/home/profile
		/// </summary>
		[DefaultValue("Canvas")]
		[Bindable(true)]
		[Description("Surface (view) to render")]
		public string Surface { get; set; }

		/// <summary>
		/// Literal content for a gadget to be parsed
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public string GadgetContent { get; set; }

		/// <summary>
		/// Indicates the dev version of the gadget should be used.
		/// </summary>
		[Bindable(true)]
		public bool DevVersion { get; set; }


		private string _controlParserKey = null;

		/// <summary>
		/// Accessor for controlParserKey.
		/// Performs lazy load upon first request
		/// </summary>
		[DefaultValue(DEFAULT_CONTROL_PARSER)]
		[Bindable(true)]
		[Description("TemplateFramework control parsing factory key")]
		public string ControlParserKey
		{
			get
			{
				if (_controlParserKey == null)
				{
					_controlParserKey = DEFAULT_CONTROL_PARSER;
				}
				return _controlParserKey;
			}
			set
			{
				_controlParserKey = value;
			}
		}


		private GadgetMaster _myGadgetMaster = null;

		/// <summary>
		/// GadgetMaster object that will render
		/// </summary>
		public GadgetMaster MyGadgetMaster
		{
			get
			{
				if (_myGadgetMaster == null)
				{
					if (ApplicationId > 0)
					{
						_myGadgetMaster = null; // GadgetGateway.GadgetProvider.GetGadgetMasterSpecific(ApplicationId, ver);
					}
					else if (!string.IsNullOrEmpty(GadgetContent))
					{
						string content = GadgetContent.Trim();
						if (!content.StartsWith("<Module"))
						{
							content = String.Concat("<Module><Content><script type='text/os-template'>", content, "\n</script></Content></Module>");
						}
						try
						{
							_myGadgetMaster = GadgetMaster.CreateGadget(ControlParserKey, content);
						}
						catch
						{
							_myGadgetMaster = new GadgetMaster();
							_myGadgetMaster.AddContentBlock(new ContentBlock());
							_myGadgetMaster.ContentBlocks[0].AddControl(new GadgetLiteral("Error Loading Content"));
						}
					}
					//else parse contents

				}
				return _myGadgetMaster;
			}
			set
			{
				_myGadgetMaster = value;
			}
		}


		/// <summary>
		/// Shows a hairline border around the control
		/// </summary>
		[Bindable(true),
		Description("Show a hairline border around control"),
		DefaultValue(false),
		Category("Appearance")]
		public bool ShowBorder { get; set; }




		protected override void Render(HtmlTextWriter writer)
		{
			if (null == MyGadgetMaster) return;

			//let's reparse
			if (MyGadgetMaster.MyControlFactory.FactoryKey != this.ControlParserKey)
			{
				ControlFactory fact = ControlFactory.GetControlFactory(ControlParserKey);
				MyGadgetMaster.MyControlFactory = fact;
				MyGadgetMaster.ReParse();
			}

			SetupGadgetForInlineRender(MyGadgetMaster);
			////todo: create an interface and wrap this better so we're not hard coding
			//if (this.ControlParserKey == DEFAULT_CONTROL_PARSER)
			//{
			//    MyGadgetMaster.MyDataResolver = DataResolver.GetInstance();
			//}
			string bordStyle = "";
			if (ShowBorder)
			{
				bordStyle = "border:1px solid green;";
			}

			writer.WriteLine(String.Format("<div style='width:{0};height:{1};{2}'>", new object[] { Width, Height, bordStyle }));
			if (MyGadgetMaster.Errors.HasParseErrors())
			{
				writer.Write("<span style='font-weight:bold;color:red;'>PARSER ERROR WITH CONTENT: </span>");
				writer.Write(MyGadgetMaster.Errors.ParseErrors[0].Message);
			}
			else
			{
				MyGadgetMaster.RenderContent(writer, Surface);
			}
			writer.WriteLine("</div>");
		}

		private static void SetupGadgetForInlineRender(GadgetMaster gadget)
		{
			gadget.ClientRenderCustomTemplates = false;
			gadget.RenderingOptions.ClientRenderDataContext = false;
		}
	}
}

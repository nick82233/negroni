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
//using Negroni.OpenSocial.Gadget;
//using Negroni.OpenSocial.Gadget.Controls;
using Negroni.TemplateFramework;


namespace Negroni
{
	/// <summary>
	/// Web composite control (server control) that can render OSML inline on a web page
	/// </summary>
	[ToolboxData("<{0}:RenderInlineControl runat=\"server\" ></{0}:RenderInlineControl>")]
	[ParseChildren(true, "MarkupContent")]
	public class RenderInlineControl : System.Web.UI.WebControls.WebControl
	{

		public RenderInlineControl()
		{
			//min width/height
			Width = 250;
			Height = 300;
			ShowBorder = true;
		}

		/// <summary>
		/// Literal content for a gadget to be parsed
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public string MarkupContent { get; set; }



		private string _controlParserKey = null;

		/// <summary>
		/// Accessor for controlParserKey.
		/// Performs lazy load upon first request
		/// </summary>
		[Bindable(true)]
		[Description("TemplateFramework control parsing factory key")]
		public string ControlParserKey
		{
			get
			{
				if (_controlParserKey == null)
				{
					List<string> factoryOptions = ControlFactory.GetControlFactoryKeys();

					//if not set, use the first factory that isn't the config factory
					for (int i = 0; i < factoryOptions.Count; i++)
					{
						if (factoryOptions[i] != Negroni.TemplateFramework.Configuration.NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY)
						{
							_controlParserKey = factoryOptions[i];
							break;
						}

					}
				}
				return _controlParserKey;
			}
			set
			{
				_controlParserKey = value;
			}
		}


		private RootElementMaster _myGadget = null;

		/// <summary>
		/// Gadget control that will render
		/// </summary>
		public RootElementMaster MyGadget
		{
			get
			{
				if(_myGadget == null){
					if (!string.IsNullOrEmpty(this.MarkupContent))
					{
						string content = MarkupContent.Trim();
						ControlFactory cf = ControlFactory.GetControlFactory(this.ControlParserKey);
						_myGadget = cf.BuildControlTree(content);
					}
				}
				return _myGadget;
			}
			set
			{
				_myGadget = value;
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
			if (null == MyGadget) return;

			if(MyGadget is RootElementMaster){
				SetupGadgetForInlineRender((RootElementMaster)MyGadget);
			}

			string bordStyle = "";
			if (ShowBorder)
			{
				bordStyle = "border:1px solid green;";
			}

			writer.WriteLine(String.Format("<div style='width:{0};height:{1};{2}'>", new object[] { Width, Height, bordStyle }));
			//if (MyGadgetMaster is GadgetMaster && ((GadgetMaster)MyGadgetMaster).Errors.HasParseErrors())
			//{
			//    writer.Write("<span style='font-weight:bold;color:red;'>PARSER ERROR WITH CONTENT: </span>");
			//    writer.Write(((GadgetMaster)MyGadgetMaster).Errors.ParseErrors[0].Message);
			//}
			MyGadget.Render(writer);

			writer.WriteLine("</div>");
		}

		private static void SetupGadgetForInlineRender(RootElementMaster gadget)
		{
			gadget.ClientRenderCustomTemplates = false;
			gadget.RenderingOptions.ClientRenderDataContext = false;
		}
	}
}

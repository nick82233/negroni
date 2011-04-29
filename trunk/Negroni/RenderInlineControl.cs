using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

using Negroni.DataPipeline;
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
			
		}

		/// <summary>
		/// Literal content for a gadget to be parsed
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public string MarkupContent { get; set; }


		/// <summary>
		/// Set to true so the control normalizes markup tags often left unclosed,
		/// like img, br, and hr
		/// </summary>
		[Bindable(true)]
		[Description("Set to true so the control normalizes markup tags often left unclosed")]
		public bool NormalizeHtml
		{
			get;
			set;
		}



		[Bindable(true)]
		[Description("Source URI for markup content that may be fetched with a GET")]
		public string Src
		{
			get;
			set;
		}

		[Bindable(true)]
		public override Unit Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
			}
		}

		[Bindable(true)]
		public override Unit Height
		{
			get
			{
				return base.Height;
			}
			set
			{
				base.Height = value;
			}
		}


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
					ControlFactory cf = ControlFactory.GetControlFactory(this.ControlParserKey);

					// if Src is specified and MarkupContent not already specified, fetch the source
					if (!string.IsNullOrEmpty(this.Src) && string.IsNullOrEmpty(MarkupContent))
					{
						_myGadget = cf.FetchGadget(this.Src);
					}
					else if (!string.IsNullOrEmpty(this.MarkupContent))
					{
						string content = MarkupContent.Trim();
						if (NormalizeHtml) //&& !ControlFactory.IsTagBalancedElement(content))
						{
							content = NormalizeHtmlMarkupTags(content);
						}
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


		private static Regex regex_Img = new Regex("<img [^/]*>");
		private static Regex regex_Br = new Regex("<br[^/]*>");
		private static Regex regex_Hr = new Regex("<hr[^/]*>");

		/// <summary>
		/// Performs rudimentary tag balancing on some html markup
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		private static string NormalizeHtmlMarkupTags(string content)
		{
			if (String.IsNullOrEmpty(content))
			{
				return string.Empty;
			}

			MatchEvaluator evalor = delegate(Match match)
				{
					return match.Value.Replace(">", "/>");
				};

			Regex[] tests = {
								regex_Img, regex_Br, regex_Hr
							};

			for (int i = 0; i < tests.Length; i++)
			{
				if (tests[i].IsMatch(content))
				{
					content = regex_Img.Replace(content, evalor);
				}
			}
			return content;
		}
		


		protected override void Render(HtmlTextWriter writer)
		{
			if (null == MyGadget) return;

			if(MyGadget is RootElementMaster){
				SetupGadgetForInlineRender((RootElementMaster)MyGadget);
			}
			string bordStyle = "";
			if (BorderWidth.Value > 0)
			{
				string color = BorderColor.IsEmpty ? "" : BorderColor.ToKnownColor().ToString();
				bordStyle = string.Format("border:{0} {1} {2};", BorderWidth.ToString(), BorderStyle.ToString(), color);
			}

			writer.Write("<div");
			if (!string.IsNullOrEmpty(CssClass))
			{
				writer.Write(" class=\"" + CssClass + "\" ");
			}
			if (!string.IsNullOrEmpty(bordStyle)
				|| !Width.IsEmpty
				|| !Height.IsEmpty)
			{
				writer.Write(" style=\"");
				writer.Write(bordStyle);
				if (!Width.IsEmpty)
				{
					writer.Write(String.Format("width:{0};", Width));
				}
				if (!Height.IsEmpty)
				{
					writer.Write(String.Format("height:{0};", Height));
				}

				writer.Write("\"");
			}
			writer.WriteLine(">");

			if (MyGadget.HasErrors())
			{
				writer.WriteLine("<pre style='font-weight:bold;color:red;'>");
				writer.WriteLine(MyGadget.Errors.ParseErrors[0].Message);
				if (MyGadget.Errors.ParseErrors[0] is XmlException)
				{
					string err = MyGadget.Errors.GetParseExceptionContent((XmlException)MyGadget.Errors.ParseErrors[0]);
					if (!string.IsNullOrEmpty(err))
					{
						writer.WriteLine(err.Replace("<", "&lt;").Replace(">", "&gt;"));
					}
				}
				writer.WriteLine("</pre>");
			}
			else
			{
				MyGadget.Render(writer);
			}
			writer.WriteLine("</div>");
		}

		private static void SetupGadgetForInlineRender(RootElementMaster gadget)
		{
			gadget.ClientRenderCustomTemplates = false;
			gadget.RenderingOptions.ClientRenderDataContext = false;
		}
	}
}

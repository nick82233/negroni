/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Represents template and data for a content block.
	/// This must have one or more templates defined.
	/// </summary>
	/// <remarks>
	/// Content blocks are identified in the client code by special CSS class names
	/// </remarks>
	[MarkupTag("Content")]
	[ContextGroup(typeof(GadgetMaster))]
	[ContextGroupContainer(true)]
	[OffsetKey("ContentBlock")]
	public class ContentBlock : BaseContainerControl, IDisposable
	{
		public readonly string ContentType = "text/html";

		#region Constants
		/// <summary>
		/// Delimiter used for identifying subviews
		/// </summary>
		public const char SUBVIEW_DELIM = '.';

		public const string CLIENTID_TEMPLATE = "_osmlContentBlock_{0}";

		/// <summary>
		/// Template for the classname used for each view name
		/// </summary>
		public const string CONTENT_CLIENT_CLASSPREFIX = "xcontentblockview-{0}";

		internal const string CONTENT_TAG = "<Content";
		internal const string CONTENT_CLOSETAG = "</Content>";

		internal const string SCRIPT_TAG = "<script";
		internal const string SCRIPT_CLOSETAG = "</script>";


		public const string PROFILEVIEW_RESERVED_LEFT = "profile.left";
		public const string PROFILEVIEW_RESERVED_RIGHT = "profile.right";

		#endregion

		internal static string TagName
		{
			get
			{
				//consider interrogation and static initializer
				return "Content";
			}
		}



		/// <summary>
		/// Sets the GadgetMaster pointer
		/// </summary>
		/// <param name="master"></param>
		/// <returns></returns>
		internal GadgetMaster SetGadgetMaster(GadgetMaster master)
		{
			base.MyRootMaster = master;
			return (GadgetMaster)MyRootMaster;
		}


		/// <summary>
		/// Mime types
		/// </summary>
		internal static string[] recognizedScriptTypes = {
													"text/os-data",
													"text/os-template"
												};

		internal const int SCRIPTTYPEKEY_DATA = 0;
		internal const int SCRIPTTYPEKEY_TEMPLATE = 1;

//		private Dictionary<string, OsTemplate> _templateHash = new Dictionary<string, OsTemplate>(15);

		private List<BaseGadgetControl> _templates = new List<BaseGadgetControl>();

		/// <summary>
		/// Loaded templates in this content block.
		/// This also allows Literals to accomodate non-templated content
		/// </summary>
		public List<BaseGadgetControl> Templates
		{
			get
			{
				return _templates;
			}
			private set
			{
				_templates = value;
			}
		}

		private List<DataScript> _dataScripts = new List<DataScript>();

		/// <summary>
		/// Data scripts defined in this ContentBlock
		/// </summary>
		public List<DataScript> DataScripts
		{
			get
			{
				return _dataScripts;
			}
			private set
			{
				_dataScripts = value;
			}
		}


		private bool _includeWrappingDivs = false;
		/// <summary>
		/// Flag to wrap the content block in div tags
		/// </summary>
		public bool IncludeWrappingDivs
		{
			get
			{
				return _includeWrappingDivs;
			}
			set
			{
				_includeWrappingDivs = value;
			}
		}

		/// <summary>
		/// Suppresses leading and trailing whitespace around content blocks, templates, and custom tag elements.
		/// Still preserves whitespace within literals.
		/// </summary>
		public bool SuppressWhitespace { get; set; }


		private Dictionary<string, string> _views = null;

		/// <summary>
		/// Defined views for this content block.
		/// Key is top-level view.  Value is any subviews (comma delimited)
		/// </summary>
		public Dictionary<string, string> Views
		{
			get
			{
				if (_views == null)
				{
					_views = new Dictionary<string, string>();
				}
				return _views;
			}
		}


		/// <summary>
		/// Initialize an empty ContentBlock
		/// </summary>
		public ContentBlock() {
			MyParseContext = new ParseContext(typeof(GadgetMaster));
		}


		public ContentBlock(string markup, GadgetMaster master)
			:this()
		{
			SetGadgetMaster(master);
			LoadTag(markup);
		}

		/// <summary>
		/// Fully initialize the ContentBlock with master, markup, and offsets
		/// </summary>
		/// <param name="markup"></param>
		/// <param name="offset"></param>
		/// <param name="master"></param>
		public ContentBlock(string markup, OffsetItem offset, GadgetMaster master)
			:this()
		{
			SetGadgetMaster(master);
			MyOffset = offset;
			LoadTag(markup);
		}


		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			LoadViews(GetAttribute("view"));
		}

		#region View Management

		/// <summary>
		/// Loads direct string views dictionary.  Does not deal with ViewMask stored at global level
		/// </summary>
		/// <param name="views"></param>
		internal void LoadViews(string views)
		{
			Views.Clear();
			if (string.IsNullOrEmpty(views))
			{
				Views.Add("*", null);
			}
			else
			{
				string[] parts = views.ToLower().Split(new char[] { ',' });
				for (int i = 0; i < parts.Length; i++)
				{
					if (string.IsNullOrEmpty(parts[i])) continue;

					string v = parts[i].Trim();
					int dotpos = v.IndexOf(SUBVIEW_DELIM);
					if (dotpos == -1)
					{
						if (!Views.ContainsKey(v))
						{
							Views.Add(v, null);
						}
					}
					else
					{
						string root = v.Substring(0, dotpos);
						if (!Views.ContainsKey(root))
						{
							Views.Add(root, null);
						}
						//special-case profile.left & profile.right


						if (string.IsNullOrEmpty(Views[root]))
						{
							Views[root] = v.Substring(dotpos + 1);
						}
						else
						{
							Views[root] += "," + v.Substring(dotpos + 1);
						}
					}
				}
			}
		}
		/// <summary>
		/// Tests to see if this content block is an anonymous view - i.e. no view was defined.
		/// Anonymous views and global views "*" are always output.
		/// </summary>
		/// <returns></returns>
		public bool IsAnonymousView()
		{
			return Views.Count == 0 ||
				Views.ContainsKey("*");
		}

		/// <summary>
		/// Tests to see if this content is qualified by checking the ViewMask
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public bool IsQualifiedView(string view)
		{
			if (string.IsNullOrEmpty(view))
			{
				throw new ArgumentNullException("Must specify a valid view name");
			}
			if (ViewMask == 0)
			{
				return true;
			}
			else
			{
				int vmask = MyRootMaster.GetViewMask(view);
				return (vmask > -1) && (0 < (ViewMask & vmask));
			}
		}

		/// <summary>
		/// Checks to see if this view is one of the reserved profile subviews sometimes used:
		/// profile.left
		/// profile.right
		/// </summary>
		/// <param name="mountSide"></param>
		/// <returns></returns>
		public bool IsReservedProfileView(out string mountSide)
		{
			if (IsQualifiedView("profile"))
			{
				if (IsSubView(PROFILEVIEW_RESERVED_LEFT))
				{
					mountSide = "left";
					return true;
				}
				else if (IsSubView(PROFILEVIEW_RESERVED_RIGHT))
				{
					mountSide = "right";
					return true;
				}
				else
				{
					mountSide = null;
					return false;
				}
			}
			else
			{
				mountSide = null;
				return false;
			}

		}

		/// <summary>
		/// Flag to indicate this a a subview
		/// </summary>
		private bool? isSubView = null;

		/// <summary>
		/// Tests to see if this is a subview
		/// </summary>
		/// <returns></returns>
		public bool IsSubView()
		{
			if (isSubView.HasValue)
			{
				return isSubView.Value;
			}
			return false;
		}

		/// <summary>
		/// Tests to see if this is a subview of the given view
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public bool IsSubView(string view)
		{
			if (string.IsNullOrEmpty(view))
			{
				return IsSubView();
			}
			view = view.ToLower();
			string theRest = null;
			int pos = view.IndexOf(SUBVIEW_DELIM);
			if (pos > -1)
			{
				if (pos < view.Length - 1)
				{
					theRest = view.Substring(pos + 1);
				}
				view = view.Substring(0, pos);
			}
					
			if (Views.ContainsKey(view))
			{
				if (!string.IsNullOrEmpty(theRest))
				{
					return (!string.IsNullOrEmpty(Views[view])
						&& Views[view].Contains(theRest));
				}
				else
				{
					//reserved profile view branch
					if (view.Equals("profile") &&
						!string.IsNullOrEmpty(Views[view]))
					{
						if (Views[view].Contains("left")
							|| Views[view].Contains("right"))
						{
							return false;
						}
					}
					return (!string.IsNullOrEmpty(Views[view]));
				}
			}
			else
			{
				return false;
			}
		}


		#endregion

		private DataScript _autoDataScript = null;

		/// <summary>
		/// Automatically constructed data script for free-form data controls
		/// </summary>
		DataScript AutoDataScript
		{
			get
			{
				if (_autoDataScript == null)
				{
					_autoDataScript = new DataScript();
					DataScripts.Add(_autoDataScript);
				}
				return _autoDataScript;
			}
		}


		/// <summary>
		/// Adds a new control to the content block
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			base.AddControl(control);
			if (control is DataScript)
			{
				DataScript script = (DataScript)control;
				//script.ViewNames = this.ViewNames;
				DataScripts.Add(script);
				ConfirmDataItemsRegistered();
			}
			else if (control is BaseDataControl)
			{
				AutoDataScript.AddControl(control);
				AutoDataScript.ConfirmDataItemsRegistered();
			}
			else if (control is OsTagTemplate)
			{
				MyCustomTagFactory.RegisterCustomTag((OsTagTemplate)control);
			}
			else if (control is OsTemplate && !(control is OsTagTemplate))
			{
				//Templates.Add(control.ID, (OsTemplate)control);
				Templates.Add((OsTemplate)control);
			}
			else if (control is OsVar)
			{
				//also add to templates for parsing
				Templates.Add(control);
			}
			else if (control is GadgetLiteral)
			{
				((GadgetLiteral)control).SuppressCDATATags = true;
				Templates.Add(control);
			}
			else
			{
				//what the heck, add everything else directly for SimpleGadget support
				Templates.Add(control);
			}
			return control;
		}


		/// <summary>
		/// Tests to see if the type identifies a containing script tag as an os-data block.
		/// </summary>
		/// <param name="scriptType"></param>
		/// <returns></returns>
		static bool IsDataScript(string scriptType)
		{
			return recognizedScriptTypes[SCRIPTTYPEKEY_DATA].Equals(scriptType, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Tests to see if the type identifies a containing script tag as an os-template block.
		/// </summary>
		/// <param name="scriptType"></param>
		/// <returns></returns>
		static bool IsTemplateScript(string scriptType)
		{
			return recognizedScriptTypes[SCRIPTTYPEKEY_TEMPLATE].Equals(scriptType, StringComparison.InvariantCultureIgnoreCase);
		}

		private bool? _isProxiedContent = null;

		/// <summary>
		/// Accessor for isProxiedContent.
		/// Performs lazy load upon first request
		/// </summary>
		public bool IsProxiedContent
		{
			get
			{
				if (!_isProxiedContent.HasValue)
				{
					_isProxiedContent = !string.IsNullOrEmpty(GetAttribute("href"));
				}
				return _isProxiedContent.Value;
			}
		}

		/// <summary>
		/// Flag to tell renderer to render the block in a hidden div.
		/// </summary>
		public bool RenderAsHidden { get; set; }

		/// <summary>
		/// Render loaded information to the writer
		/// </summary>
		/// <param name="writer"></param>
		override public void Render(TextWriter writer)
		{
			if (!IsParsed)
			{
				Parse();
			}

			if (IsProxiedContent)
			{
				RenderProxiedContentControls(writer);
			}
			else
			{
				RenderStandardControls(writer);
			}
		}

		private void RenderStandardControls(TextWriter writer)
		{
			if (IncludeWrappingDivs || RenderAsHidden)
			{
				string classString = BuildViewClassString();
				writer.Write("<div");
				if (IncludeWrappingDivs)
				{
					writer.Write(" class=\"" + classString + "\" ");
					if (!string.IsNullOrEmpty(ID))
					{
						writer.Write(" id=");
						writer.Write(JsonData.JSSafeQuote(string.Format(ContentBlock.CLIENTID_TEMPLATE, ID)));
					}
				}
				if (RenderAsHidden)
				{
					writer.Write(" style=\"display:none;\" ");
				}
				writer.WriteLine(">");
			}

			foreach (BaseGadgetControl template in Templates)
			{
				if (!template.IsParsed)
				{
					template.Parse();
				}
				template.Render(writer);
			}

			if (IncludeWrappingDivs || RenderAsHidden)
			{
				writer.WriteLine();
				writer.WriteLine("</div>");
			}
		}

		private void RenderProxiedContentControls(TextWriter writer)
		{
			ExternalRequestControl ctl = new ExternalRequestControl();
			ctl.MyRootMaster = this.MyRootMaster;
			ctl.Parent = this;
			ctl.LoadTag(this.RawTag);
			ctl.AppendCultureInfo = true;
			//app proxied content flag
			if (ctl.Src.Contains("?"))
			{
				ctl.Src += "&opensocial_proxied_content=1";
			}
			else
			{
				ctl.Src += "?opensocial_proxied_content=1";
			}
			if (string.IsNullOrEmpty(ctl.TargetElement))
			{
				writer.Write("<div id='");
				writer.Write(ctl.ID);
				writer.Write("' ></div>");
			}
			ctl.Render(writer);
			ctl.Parent = null;
			ctl.MyRootMaster = null;
		}

		/// <summary>
		/// Confirms all data items registered with current DataContext
		/// </summary>
		/// <remarks>
		///There is a timing issue with the parse/data registration flow.
		///The data items are registered with a phantom DataContext.

		///TODO: change code to delay register data items
		///in the mean time we re-register post parse.
		/// </remarks>
		internal void ConfirmDataItemsRegistered()
		{
			//this.DataScripts
			foreach (DataScript script in DataScripts)
			{
				script.ConfirmDataItemsRegistered();				
			}
		}



		/// <summary>
		/// Builds the css view class string
		/// </summary>
		/// <returns></returns>
		private string BuildViewClassString()
		{
			string retVal = String.Empty;
			if (ViewMask > 0)
			{
				string[] views = MyRootMaster.GetViewString(ViewMask).Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < views.Length; i++)
				{
					if (!views[i].Contains("."))
					{
						retVal += String.Format(CONTENT_CLIENT_CLASSPREFIX + " ", views[i]);
					}
				}
			}
			return retVal;
		}

		

		#region IDisposable Members

		override protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (null != _templates && _templates.Count > 0)
				{
					foreach (BaseGadgetControl control in _templates)
					{
						control.Dispose();
					}
					_templates.Clear();
					_templates = null;
				}
			}
		}

		#endregion

	}
}

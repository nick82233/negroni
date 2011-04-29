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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Negroni.TemplateFramework.Parsing;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework
{

	/// <summary>
	/// Generic root element control.  
	/// This holds the global DataContext as well as all sub-controls of the markup.
	/// </summary>
	public class RootElementMaster : BaseContainerControl, IDisposable
	{
#if DEBUG
		public string myId = Guid.NewGuid().ToString();
#endif

		#region Constructors

		public RootElementMaster()
		{
			MyParseContext = ParseContext.RootContext;
			MyRootMaster = this;
		}
		public RootElementMaster(string controlFactoryKey)
			: this()
		{
			this.MyControlFactory = ControlFactory.GetControlFactory(controlFactoryKey);
		}
		public RootElementMaster(ControlFactory controlFactory)
			: this()
		{
			this.MyControlFactory = controlFactory;
		}

		public RootElementMaster(string gadgetXml, ControlFactory controlFactory)
			: this()
		{
			this.MyControlFactory = controlFactory;
			//trim whitespace
			if (!string.IsNullOrEmpty(gadgetXml)
				&& (gadgetXml.StartsWith(" ", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\t", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\r\n", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\n", StringComparison.InvariantCulture)))
			{
				gadgetXml = gadgetXml.Trim();
			}
			LoadTag(gadgetXml);
		}

		public RootElementMaster(string gadgetXml, OffsetItem masterOffsets, ControlFactory controlFactory)
			: this()
		{
			//trim whitespace
			if (!string.IsNullOrEmpty(gadgetXml)
				&& (gadgetXml.StartsWith(" ", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\t", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\r\n", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\n", StringComparison.InvariantCulture)))
			{
				gadgetXml = gadgetXml.Trim();
			}

			this.MyControlFactory = controlFactory;
			this.MyOffset = masterOffsets;
			LoadTag(gadgetXml);
		}

		public RootElementMaster(string gadgetXml, string controlFactoryKey)
			: this()
		{
			this.MyControlFactory = ControlFactory.GetControlFactory(controlFactoryKey);
			//trim whitespace
			if (!string.IsNullOrEmpty(gadgetXml)
				&& (gadgetXml.StartsWith(" ", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\t", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\r\n", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\n", StringComparison.InvariantCulture)))
			{
				gadgetXml = gadgetXml.Trim();
			}
			LoadTag(gadgetXml);
		}

		public RootElementMaster(string gadgetXml, OffsetItem masterOffsets, string controlFactoryKey)
			: this()
		{
			this.MyControlFactory = ControlFactory.GetControlFactory(controlFactoryKey);
			this.MyOffset = masterOffsets;
			//trim whitespace
			if (!string.IsNullOrEmpty(gadgetXml)
				&& (gadgetXml.StartsWith(" ", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\t", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\r\n", StringComparison.InvariantCulture)
				|| gadgetXml.StartsWith("\n", StringComparison.InvariantCulture)))
			{
				gadgetXml = gadgetXml.Trim();
			}
			LoadTag(gadgetXml);
		}

		#endregion


		/// <summary>
		/// Flag to instruct the control to render all custom templates
		/// to the client-side representation.  By default this is false
		/// </summary>
		public bool ClientRenderCustomTemplates
		{
			get
			{
				return RenderingOptions.ClientRenderCustomTemplates;
			}
			set
			{
				RenderingOptions.ClientRenderCustomTemplates = value;
			}
		}


		private RenderOptions _renderingOptions = null;

		/// <summary>
		/// Accessor for renderingOptions.
		/// Performs lazy load upon first request
		/// </summary>
		public RenderOptions RenderingOptions
		{
			get
			{
				if (_renderingOptions == null)
				{
					_renderingOptions = new RenderOptions();
				}
				return _renderingOptions;
			}
		}


		/// <summary>
		/// Load new RenderingOptions.
		/// </summary>
		/// <param name="options"></param>
		public void LoadRenderingOptions(RenderOptions options)
		{
			if (null == options) return;
			RenderingOptions.SuppressWhitespace = options.SuppressWhitespace;
			RenderingOptions.DivWrapContentBlocks = options.DivWrapContentBlocks;
			RenderingOptions.ClientRenderCustomTemplates = options.ClientRenderCustomTemplates;
			RenderingOptions.ClientRenderDataContext = options.ClientRenderDataContext;
		}



		private IOffsetParser _offsetParser = null;
		/// <summary>
		/// Offset parser to use for all items
		/// </summary>
		public IOffsetParser OffsetParser
		{
			get
			{
				if (null == _offsetParser)
				{
					_offsetParser = ParserFactory.GetOffsetParser(MyControlFactory);
				}
				return _offsetParser;
			}
			set
			{
				_offsetParser = value;
			}
		}

		/// <summary>
		/// Overrides the ControlFactory, only if not already set
		/// </summary>
		/// <param name="controlFactory"></param>
		public void ReconfirmControlFactorySet(ControlFactory controlFactory)
		{
			if (!HasControlFactorySet())
			{
				MyControlFactory = controlFactory;
			}
		}


		private DataContext _masterDataContext = null;
		/// <summary>
		/// Master DataContext object for this gadget.
		/// </summary>
		public DataContext MasterDataContext
		{
			get
			{
				if (null == _masterDataContext)
				{
					_masterDataContext = new DataContext();
				}
				return _masterDataContext;
			}
			set
			{
				_masterDataContext = value;
			}
		}

		/// <summary>
		/// Hook to data control-specific resolver.
		/// This can be null or ignored, if an external process handles the resolver.
		/// </summary>
		public IDataPipelineResolver MyDataResolver { get; set; }


		private CustomTagFactory _masterCustomTagFactory = null;

		/// <summary>
		/// Factory for all custom tags registered for this app.
		/// </summary>
		public CustomTagFactory MasterCustomTagFactory
		{
			get
			{
				if (_masterCustomTagFactory == null)
				{
					_masterCustomTagFactory = new CustomTagFactory(this);
				}
				return _masterCustomTagFactory;
			}
		}



		#region Error Logging Support


		private GadgetErrors _errors = null;

		/// <summary>
		/// Errors existing in this gadget.
		/// This includes ParseErrors, CircularReferenceErrors,
		/// obsolete control warnings, etc.
		/// </summary>
		public GadgetErrors Errors
		{
			get
			{
				if (_errors == null)
				{
					_errors = new GadgetErrors(this);
				}
				return _errors;
			}
		}

		/// <summary>
		/// Returns true if the gadget has any parsing errors.
		/// </summary>
		/// <returns></returns>
		public bool HasErrors()
		{
			return Errors.HasParseErrors();
		}


		/// <summary>
		/// Recursively checks to see if a data key is completely resolvable.
		/// To be resolvable, every dependent key must ultimately resolve to 
		/// a control that has no dynamic dependencies
		/// </summary>
		/// <param name="key"></param>
		/// <param name="keysAlreadyUsed"></param>
		/// <param name="circularKey">Variable to contain key identified as circular</param>
		/// <returns></returns>
		public bool IsResolvableKey(string key, Dictionary<string, string> keysAlreadyUsed, out string circularKey)
		{
			if (!MyDataContext.HasVariable(key))
			{
				circularKey = key;
				return false;
			}
			circularKey = null;
			DataItem item = MyDataContext.MasterData[key];
			if (null == item)
			{
				circularKey = key;
				return false;
			}
			if (item.DataControl is BaseDataControl)
			{
				BaseDataControl control = (BaseDataControl)item.DataControl;
				if (!control.HasDynamicParameters())
				{
					return true;
				}
				else
				{
					bool resolvable = true;
					string[] requiredKeys = control.GetDynamicKeyDependencies();
					for (int i = 0; i < requiredKeys.Length; i++)
					{
						if (keysAlreadyUsed.ContainsKey(requiredKeys[i]))
						{
							circularKey = requiredKeys[i];
							return false;
						}
						else
						{
							Dictionary<string, string> thisBranchKeysUsed = new Dictionary<string, string>(keysAlreadyUsed);
							thisBranchKeysUsed.Add(key, key);
							resolvable = IsResolvableKey(requiredKeys[i], thisBranchKeysUsed, out circularKey);
						}
						if (!resolvable)
						{
							return false;
						}
					}
					return true;
				}
			}
			else
			{
				return true;
			}

		}


		#endregion

		private bool _hasXmlDeclaration = false;

		/// <summary>
		/// True when the first line of the RawXML is an xml declaration
		/// ex: &lt;?xml version="1.0" encoding="UTF-8" standalone="no" ?&gt;
		/// </summary>
		public bool HasXmlDeclaration
		{
			get
			{
				return _hasXmlDeclaration;
			}
			internal set
			{
				_hasXmlDeclaration = value;
			}
		}

		/// <summary>
		/// Encapsulation of information in XML declaration.
		/// NOTE: THIS IS ALWAYS NULL
		/// </summary>
		public object XmlDeclaration
		{
			get
			{
				return null;
			}
		}


		/// <summary>
		/// Extracts the raw markup block based on the passed in OffsetItems
		/// </summary>
		/// <param name="item"></param>
		/// <param name="nextSibling"></param>
		/// <returns></returns>
        protected string GetRawBlock(OffsetItem item, OffsetItem nextSibling)
        {
            string rawBlock;
			int startIndex = item.GetAbsolutePosition();
            int closeIndex = item.GetAbsoluteEndPosition();

            if (item.EndPosition > 0)
            {
                closeIndex = item.GetAbsoluteEndPosition();
            }
            else
            {
				if (nextSibling != null)
				{
					closeIndex = item.GetAbsoluteEndPosition(nextSibling.Position);
				}
				else{
					closeIndex = RawTag.Length - 1; 
                }
			}

			rawBlock = RawTag.Substring(startIndex, closeIndex - startIndex);

            return rawBlock;
        }


		/// <summary>
		/// Returns an array of views that are defined for this gadget
		/// </summary>
		/// <returns></returns>
		virtual public string[] GetDefinedViews()
		{
			return _definedViews.ToArray();
		}

		private List<string> _definedViews = new List<string>();


		#region ExternalRenderedControls

		/// <summary>
		/// Internal structure representing controls with an external
		/// data source marked for server-side resolution
		/// </summary>
		protected struct ExternalControlDef
		{
			public int ValidViewMask;
			public IExternalDataSource ExternalSource;
		}



		private List<ExternalControlDef> _externalServerRenderControls = null;

		/// <summary>
		/// Controls
		/// </summary>
		protected List<ExternalControlDef> ExternalServerRenderControls
		{
			get
			{
				if (_externalServerRenderControls == null)
				{
					_externalServerRenderControls = new List<ExternalControlDef>();
				}
				return _externalServerRenderControls;
			}
		}

		/// <summary>
		/// Builds and returns a list of IExternalDataSource items that are identified
		/// for server-side rendering.
		/// </summary>
		/// <param name="surfaceName"></param>
		/// <returns></returns>
		public List<IExternalDataSource> GetExternalServerRenderControls()
		{
			return GetExternalServerRenderControls(null);
		}

		/// <summary>
		/// Builds and returns a list of IExternalDataSource items that are identified
		/// for server-side rendering.
		/// </summary>
		/// <param name="surfaceName"></param>
		/// <returns></returns>
		public List<IExternalDataSource> GetExternalServerRenderControls(string surfaceName)
		{
			List<IExternalDataSource> refs = new List<IExternalDataSource>();
			if (HasExternalServerRenderControls(surfaceName))
			{
				foreach (ExternalControlDef item in ExternalServerRenderControls)
				{
					if (string.IsNullOrEmpty(surfaceName) || surfaceName == "*"
						|| item.ValidViewMask == 0)
						//|| this.Get
						//|| item.ValidViews.Contains(surfaceName))
					{
						refs.Add(item.ExternalSource);
					}
				}
			}
			return refs;
		}

		private List<string> _viewsDefined = null;

		/// <summary>
		/// Defined views, indexed by their integer bitmask location
		/// </summary>
		public List<string> ViewsDefined
		{
			get
			{
				if (_viewsDefined == null)
				{
					_viewsDefined = new List<string>();
					_viewsDefined.Add("*"); //zero item is everything
				}
				return _viewsDefined;
			}
		}
		/// <summary>
		/// Tests to see if there are any views defined for this object
		/// </summary>
		/// <returns></returns>
		public bool HasViewsDefined()
		{
			return (_viewsDefined != null);
		}
		/// <summary>
		/// Tests the defined views to see if the asked for view is defined
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public bool HasViewDefined(string view)
		{
			if (string.IsNullOrEmpty(view))
			{
				return false;
			}
			if (view == "*") return true;

			return ViewsDefined.Contains(view.ToLower());
		}

		/// <summary>
		/// Tests the defined views to see if the asked for view is defined
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public bool HasViewDefined(int viewID)
		{
			if (_viewsDefined == null) return false;
			uint result = (UInt32.MaxValue >> (32 - ViewsDefined.Count));
			return viewID == (result & viewID);
		}


		/// <summary>
		/// Extracts the string representation of the view from the viewMask value.
		/// </summary>
		/// <param name="viewMask"></param>
		/// <returns></returns>
		public string GetViewString(int viewMask)
		{
			if (!HasViewsDefined())
			{
				return "*";
			}
			else
			{
				string result = string.Empty;
				for (int i = 1; i < ViewsDefined.Count; i++)
				{
					if (0 < (viewMask & (1 << (i - 1))))
					{
						if(!string.IsNullOrEmpty(result)){
							result +=",";
						}
						result += ViewsDefined[i];
					}
				}
				return result;
			}
		}


		/// <summary>
		/// Returns an integer bitmask representing the views defined in the string.
		/// Multiple views should be comma delimited.
		/// </summary>
		/// <param name="views"></param>
		/// <returns>Bit Index of view in ViewMask, Zero if anonymous/global view, -1 if view is not defined</returns>
		public int GetViewMask(string views)
		{
			if (string.IsNullOrEmpty(views) || views == "*")
			{
				return 0;
			}
			views = views.ToLower();
			if (views.Contains(","))
			{
				string[] parts = views.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				int result = 0;
				for (int i = 0; i < parts.Length; i++)
				{
					int x = GetViewMask(parts[i]);
					if (x > 0 && 0 == (x & result))
					{
						result |= x;
					}
				}

				return result;
			}
			else
			{
				if (HasViewsDefined())
				{
					views = views.Trim();
					int pos = views.IndexOf(".");
					if (pos > -1)
					{
						views = views.Substring(0, pos);
					}
					for (int i = 0; i < ViewsDefined.Count; i++)
					{
						if (views.Equals(ViewsDefined[i]))
						{
							return 1 << (i - 1);
						}
					}
					return -1;
				}
				else
				{
					return 0;
				}
			}
		}


		/// <summary>
		/// Registers a comma delimited list of views.
		/// Returns the full bitmask for the views
		/// </summary>
		/// <param name="viewString"></param>
		/// <returns></returns>
		public int RegisterViews(string viewString)
		{
			int answer = 0;
			if (string.IsNullOrEmpty(viewString))
			{
				return -1;
			}
			if (viewString == "*")
			{
				return 0;
			}
			viewString = viewString.ToLower();
			if (viewString.Contains(","))
			{
				string[] parts = viewString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < parts.Length; i++)
				{
					int x = RegisterViews(parts[i].Trim());
					if (0 == (x & answer))
					{
						answer |= x;
					}
				}
			}
			else{
				//trim subview
				int pos = viewString.IndexOf(".");
				if (pos > -1)
				{
					viewString = viewString.Substring(0, pos);
				}

				if (ViewsDefined.Contains(viewString)){
					for (int n = 0; n < ViewsDefined.Count; n++)
					{
						if (ViewsDefined[n].Equals(viewString, StringComparison.InvariantCultureIgnoreCase))
						{
							answer = 1 << (n-1);
							break;
						}
					}
				}
				else{
					if (ViewsDefined.Count >= 32)
					{
						throw new Exception("Exceeding limit of 32 views");
					}
						ViewsDefined.Add(viewString);
						answer = 1 << (ViewsDefined.Count-2);
				}
			}
			return answer;			
		}


		/// <summary>
		/// Tests the view names string to see if targetView is valid
		/// </summary>
		/// <param name="registeredViews"></param>
		/// <param name="targetView"></param>
		/// <returns></returns>
		public static bool IsValidView(string registeredViews, string targetView)
		{
			if (string.IsNullOrEmpty(registeredViews)
				|| string.IsNullOrEmpty(targetView)
				|| registeredViews.Contains("*")
				|| targetView == "*")
			{
				return true;
			}
			if (registeredViews.Contains(","))
			{
				string[] parts = registeredViews.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < parts.Length; i++)
				{
					if (parts[i].Trim().Equals(targetView, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				return registeredViews.Equals(targetView, StringComparison.InvariantCultureIgnoreCase);
			}
		}


		/// <summary>
		/// Registers a control as being an external data source for server-side rendering.
		/// Usually the controls should handle this call internally.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public IExternalDataSource RegisterExternalServerRenderControl(IExternalDataSource control)
		{
			return RegisterExternalServerRenderControl(control, 0);
		}

		/// <summary>
		/// Registers a control as being an external data source for server-side rendering.
		/// Usually the controls should handle this call internally.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public IExternalDataSource RegisterExternalServerRenderControl(IExternalDataSource control, int validViewsMask)
		{
			if (control == null)
			{
				return null;
			}
			ExternalControlDef cdef;
			cdef.ExternalSource = control;
			cdef.ValidViewMask = validViewsMask;
			ExternalServerRenderControls.Add(cdef);
			return control;
		}


		/// <summary>
		/// Tests to see if any controls are in the control tree that desire to be 
		/// server-side rendered.
		/// </summary>
		/// <returns></returns>
		public bool HasExternalServerRenderControls()
		{
			return (_externalServerRenderControls != null && _externalServerRenderControls.Count > 0);
		}

		/// <summary>
		/// Tests to see if any controls are in the control tree that desire to be 
		/// server-side rendered.
		/// </summary>
		/// <returns></returns>
		public bool HasExternalServerRenderControls(string surfaceName)
		{
			if (_externalServerRenderControls == null || _externalServerRenderControls.Count == 0)
			{
				return false;
			}
			else
			{
				if (string.IsNullOrEmpty(surfaceName) || surfaceName == "*")
				{
					return true;
				}
				int viewBit = GetViewMask(surfaceName);
				foreach (ExternalControlDef item in _externalServerRenderControls)
				{
					if (viewBit == 0 || item.ValidViewMask == 0 || (viewBit & item.ValidViewMask) > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		#endregion

		#region IDisposable Members

		override protected void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_masterDataContext != null)
			{
				_masterDataContext.Dispose();
			}
			_masterDataContext = null;
			if (_masterCustomTagFactory != null)
			{
				_masterCustomTagFactory = null;
			}
		}

		#endregion
	}
}

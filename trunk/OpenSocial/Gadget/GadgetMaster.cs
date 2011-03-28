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



using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.DataPipeline;
using Negroni.DataPipeline.RequestProcessing;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML.Templates;
using Negroni.TemplateFramework;


namespace Negroni.OpenSocial.Gadget
{
    /// <summary>
    /// Master holding object for all content.
    /// This is representative of the entire Gadget.
    /// </summary>
    [MarkupTag("Module")]
    [OffsetKey("GadgetRoot")]
    [RootElement(false)]
    public class GadgetMaster : RootElementMaster, ICloneable
    {
        public const string CLIENT_TEMPLATE_BLOCK_ID = "osml_custom_client_templates";


        /// <summary>
        /// Encapsulation of client-resolved DFS paths to raw source
        /// </summary>
        public class RawSourcePaths
        {
            internal RawSourcePaths() { }
            /// <summary>
            /// URL to latest development source code
            /// </summary>
            public string LatestDevelopment { get; set; }

            /// <summary>
            /// URL to published source code
            /// </summary>
            public string Published { get; set; }

        }



        /// <summary>
        /// Starting javascript tag, with cdata for xhtml compliance
        /// </summary>
        internal const string JS_START_BLOCK_TAGS = "<script type=\"text/javascript\" >\n//<![CDATA[\n";
        /// <summary>
        /// Closing javascript tag, with cdata for xhtml compliance
        /// </summary>
		internal const string JS_END_BLOCK_TAGS = "//]]>\n</script>\n";

        #region Caching mechanism support

        private int _appId = 0;

        /// <summary>
        /// ID of the gadget app being managed.
        /// This value is zero for non-apps, or negative for virtual (sandboxed) apps.
        /// </summary>
        public int AppID
        {
            get
            {
                //return MyDataContext.AppID;
                return _appId;
            }
            set
            {
                //MyDataContext.AppID = value;
                _appId = value;
				ID = value.ToString();
            }
        }

		/// <summary>
		/// Flag indicating the gadget needs to be re-parsed.
		/// Reparsing is required if an external template library is loaded.
		/// </summary>
		internal bool NeedsReparse { get; set; }


        private string _serializationOffsets = null;
        /// <summary>
        /// This is a value to support cache serialization.
        /// </summary>
        public string SerializationOffsets
        {
            get
            {
                if (MyOffset != null)
                {
                    if (null == _serializationOffsets || !MyOffset.ToString().Equals(_serializationOffsets))
                    {
                        _serializationOffsets = MyOffset.ToString();
                    }
                }
                return _serializationOffsets;
            }
            protected set
            {
                _serializationOffsets = value;
                MyOffset = new OffsetItem(_serializationOffsets);
            }
        }

        private string _serializationControlFactoryKey = null;

        /// <summary>
        /// Key string of the ControlFactory to use when reconstructing this Gadget.
        /// Setting this string also causes the MyControlFactory value to initialize.
        /// Throws an exception if an invalid ControlFactory key is used.
        /// </summary>
        public string SerializationControlFactoryKey
        {
            get
            {
                if (null == _serializationControlFactoryKey && HasControlFactorySet())
                {
                    _serializationControlFactoryKey = MyControlFactory.FactoryKey;
                }
                return _serializationControlFactoryKey;
            }
            set
            {
                MyControlFactory = ControlFactory.GetControlFactory(value);
                _serializationControlFactoryKey = value;
            }
        }

		private string _serializationExternalMessageBundle = null;
		/// <summary>
		/// Raw external message bundles
		/// </summary>
		public string SerializationExternalMessageBundle
		{
			get
			{
				if (null == _serializationExternalMessageBundle)
				{
					_serializationExternalMessageBundle = GetConsolidatedMessageBundles();
				}
				return _serializationExternalMessageBundle;

			}
			set
			{
				_serializationExternalMessageBundle = value;
				LoadConsolidatedMessageBundles(_serializationExternalMessageBundle);

			}
		}



        #endregion

        #region Static factory methods

        /// <summary>
        /// Create a gadget and initialize it to use the appropriate ControlFactory instance
        /// </summary>
        /// <param name="controlFactoryKey"></param>
        /// <param name="gadgetXml"></param>
        /// <returns></returns>
        static public GadgetMaster CreateGadget(string controlFactoryKey, string gadgetXml)
        {
            return CreateGadget(controlFactoryKey, gadgetXml, null);
        }

        /// <summary>
        /// Create a gadget and initialize it to use the appropriate ControlFactory instance
        /// </summary>
        /// <param name="controlFactoryKey"></param>
        /// <param name="gadgetXml"></param>
        /// <returns></returns>
        static public GadgetMaster CreateGadget(string controlFactoryKey, string gadgetXml, OffsetItem offset)
        {
            if (string.IsNullOrEmpty(controlFactoryKey))
            {
                throw new ArgumentNullException("ControlFactory must be identified");
            }
            ControlFactory fact = ControlFactory.GetControlFactory(controlFactoryKey);

            if (null == fact)
            {
                throw new ArgumentException("Specified ControlFactory not registered: " + controlFactoryKey);
            }

            return CreateGadget(fact, gadgetXml, offset);
        }

        static public GadgetMaster CreateGadget(ControlFactory fact, string gadgetXml, OffsetItem offset)
        {
            GadgetMaster gadget = new GadgetMaster();

            gadget.MyControlFactory = fact;

            if (offset != null)
            {
                gadget.MyOffset = offset;
            }
//			if(System.Text.Encoding.GetEncoding(
            gadget.LoadTag(gadgetXml);

            return gadget;
        }


        #endregion


		#region Static counter support

		static private GadgetCounters _counters = null;

        /// <summary>
        /// Accessor for counters on renderings & statistics
        /// </summary>
        static public GadgetCounters Counters
        {
            get
            {
                if (_counters == null)
                {
                    _counters = new GadgetCounters();
                }
                return _counters;
            }
        }

        #endregion

        #region Constructors

        public GadgetMaster()
            : base()
        {
            RenderingOptions.ClientRenderDataContext = false;
        }

        public GadgetMaster(ControlFactory controlFactory)
            : this()
        {
            MyControlFactory = controlFactory;
        }

        public GadgetMaster(ControlFactory controlFactory, string gadgetXml)
            : this(controlFactory)
        {
            LoadTag(gadgetXml);
        }

        public GadgetMaster(ControlFactory controlFactory, string gadgetXml, OffsetItem masterOffsets)
            : this(controlFactory)
        {
            this.MyOffset = masterOffsets;
            LoadTag(gadgetXml);
        }

        #endregion



        #region Custom object collection accessors - ModulePrefs, ContentBlocks, etc.


        private RawSourcePaths _sourcePaths = null;

        /// <summary>
        /// Accessor for sourcePaths.
        /// Performs lazy load upon first request
        /// </summary>
        public RawSourcePaths SourcePaths
        {
            get
            {
                if (_sourcePaths == null)
                {
                    _sourcePaths = new RawSourcePaths();
                }
                return _sourcePaths;
            }
        }


        /// <summary>
        /// Gets an event link.
        /// Returns null if no link defined for that event.
        /// </summary>
        /// <param name="eventKey">Key as defined in <c>LifecycleEventKey</c> object</param>
        /// <returns></returns>
        public Link GetEventLink(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                throw new ArgumentNullException("eventKey must be specified");
            }

            if (ModulePrefs.Links.Count > 0)
            {
                foreach (Link link in ModulePrefs.Links)
                {
                    if (eventKey.Equals(link.Rel, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return link;
                    }
                }
            }
            return null;
        }

		/// <summary>
		/// Returns an array of views that are defined for this gadget
		/// </summary>
		/// <returns></returns>
		public string[] GetDefinedViews()
		{
			if (ContentBlocks.Count == 0)
			{
				return new string[] { };
			}
			if (HasViewsDefined())
			{
				return ViewsDefined.ToArray();
			}
			else
			{
				return new string[] { };
			}
		}


        /// <summary>
        /// Gets the required OpenSocial version to use with this app gadget.
        /// Lowest version should be 1.0 (accepts 0.9)
        /// </summary>
        /// <returns></returns>
        public string GetOpenSocialVersion()
        {
            string defaultVersion = "1.0";

            if (this.ModulePrefs.RequiredFeatures.Count > 0)
            {
                foreach (ModuleRequire require in ModulePrefs.RequiredFeatures)
                {
                    int pos = require.Feature.IndexOf(ModuleFeature.OPENSOCIAL_FEATURE_PREFIX);
                    if (pos > -1)
                    {
                        int startPos = pos + ModuleFeature.OPENSOCIAL_FEATURE_PREFIX.Length;
                        double ver;
                        string verStr = require.Feature.Substring(startPos);
                        if (Double.TryParse(verStr, out ver))
                        {
                            return verStr;
                        }
                    }
                }
            }

            return defaultVersion;
        }


        private ModulePrefs _modulePrefs = null;

        /// <summary>
        /// ModulePrefs section for this gadget
        /// </summary>
        public ModulePrefs ModulePrefs
        {
            get
            {
                if (null == _modulePrefs)
                {
                    _modulePrefs = new ModulePrefs();
					_modulePrefs.MyRootMaster = this;
                }
                return _modulePrefs;
            }
            private set
            {
                _modulePrefs = value;
            }
        }


        private ContentBlock CreateLocalContentBlock()
        {
            ContentBlock block = new ContentBlock();
            block.SetGadgetMaster(this);
            return block;
        }

		/// <summary>
		/// Loads a template library into the custom tag factory
		/// </summary>
		/// <param name="uriKey">Original uri key to the library</param>
		/// <param name="templateLibrary">XML contents of the library file</param>
		public TemplatesRoot LoadTemplateLibrary(string uriKey, string templateLibrary)
		{
			return LoadTemplateLibrary(uriKey, templateLibrary, (string)null);
		}

		/// <summary>
		/// Loads a template library into the custom tag factory
		/// </summary>
		/// <param name="uriKey">Original uri key to the library</param>
		/// <param name="templateLibrary">XML contents of the library file</param>
		/// <param name="offsets">Offset list for the library</param>
		public TemplatesRoot LoadTemplateLibrary(string uriKey, string templateLibrary, string offsets)
		{
			NeedsReparse = true;
			OffsetItem offsetItem = null;
			if (!string.IsNullOrEmpty(offsets))
			{
				offsetItem = new OffsetItem(offsets);
			}

			return LoadTemplateLibrary(uriKey, templateLibrary, offsetItem);
		}

		/// <summary>
		/// Loads a template library into the custom tag factory
		/// </summary>
		/// <param name="uriKey">Original uri key to the library</param>
		/// <param name="templateLibrary">XML contents of the library file</param>
		/// <param name="offsets">Offset list for the library</param>
		public TemplatesRoot LoadTemplateLibrary(string uriKey, string templateLibrary, OffsetItem offsetItem)
		{
			TemplatesRoot lib = new TemplatesRoot(templateLibrary, this, offsetItem);
			MyCustomTagFactory.RegisterCustomTags(lib.CustomTags);
			foreach (TemplateLibraryDef templateDef in TemplateLibraries.Libraries)
			{
				if (uriKey == templateDef.Uri)
				{
					templateDef.LibraryXml = templateLibrary;
					templateDef.Loaded = true;
					break;
				}				
			}
			return lib;
		}


        private List<ContentBlock> _contentBlocks = null;

        /// <summary>
        /// Accessor for contentBlocks.
        /// Performs lazy load upon first request.
        /// These controls are also part of the Controls collection
        /// </summary>
        public List<ContentBlock> ContentBlocks
        {
            get
            {
                if (_contentBlocks == null)
                {
                    _contentBlocks = new List<ContentBlock>();
                }
                return _contentBlocks;
            }
        }

		private List<DataBlock> _dataBlocks = null;

		/// <summary>
		/// Accessor for dataBlocks.
		/// These controls are also part of the Controls collection
		/// </summary>
		public List<DataBlock> DataBlocks
		{
			get
			{
				if (_dataBlocks == null)
				{
					_dataBlocks = new List<DataBlock>();
				}
				return _dataBlocks;
			}
		}


		/// <summary>
		/// Shortcut to the TemplateLibraries defined in ModulePrefs
		/// </summary>
		public ExternalTemplates TemplateLibraries
		{
			get
			{
				return this.ModulePrefs.TemplateLibraries;
			}
		}

        /// <summary>
        /// Shortcut to the MySpace AppSettings defined in ModulePrefs
        /// </summary>
		public MySpaceAppSettings MySpaceAppSettings
        {
            get
            {
				return this.ModulePrefs.MySpaceAppSettings;
            }
        }

        /// <summary>
        /// Shortcut to the MySpace ViewSettings defined in ModulePrefs
        /// </summary>
        public MySpaceViewSettings MySpaceViewSettings
        {
            get
            {
                return this.ModulePrefs.MySpaceViewSettings;
            }
        }


        /// <summary>
        /// Add a new content block to this gadget
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ContentBlock AddContentBlock(ContentBlock content)
        {
            content.MyRootMaster = this;
            ContentBlocks.Add(content);
            content.IncludeWrappingDivs = RenderingOptions.DivWrapContentBlocks;
			//check for reserved profile.left / profile.right and handle side effects
			string pmount;
			if (content.IsReservedProfileView(out pmount))
			{
				this.MySpaceViewSettings.ProfileLocation = pmount;
			}

            return content;
        }

        #endregion


        #region Parse directives

        /// <summary>
        /// Reparses the current RawTag, clearing all values and data
		/// in the process
        /// </summary>
        public void ReParse()
        {
			ReParse(false);
        }

		/// <summary>
		/// Reparses the current tag.
		/// </summary>
		/// <param name="preserveData">When true the DataContext is left intact</param>
		public void ReParse(bool preserveData)
		{
			ExternalTemplates exTemplates = null;
			if (HasExternalTemplateLibraries())
			{
				exTemplates = TemplateLibraries;
			}

			IsParsed = false;
			ClearAllControls();
			MyOffset = null;
			if (!preserveData)
			{
				this.MasterDataContext = new DataContext();
			}
			//reload external templates
			if (exTemplates != null && exTemplates.Libraries.Count > 0)
			{
				foreach (TemplateLibraryDef lib in exTemplates.Libraries)
				{
					LoadTemplateLibrary(lib.Uri, lib.LibraryXml);
				}

			}

			this.Parse(); //this call also kills the data


			NeedsReparse = false;
		}


        /// <summary>
        /// Parses the control tree.  Does not re-parse if already parsed.
        /// </summary>
        public override void Parse()
        {
            if (this.IsParsed)
            {
                return;
            }


            try
            {
                base.Parse();
				NeedsReparse = false;
            }
            catch (System.Xml.XmlException ex)
            {
                this.Errors.ParseErrors.Add(ex);
            }

            //update counters
            if (GadgetMaster.Counters.ParseCount.IsGlobalCounterEnabled)
            {
                GadgetMaster.Counters.ParseCount.IncrementGlobalCount();
            }

            IsParsed = true;


        }

		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			base.AddControl(control);

			if (control is ModulePrefs)
			{
				this.ModulePrefs = (ModulePrefs)control;
			}
			else if (control is ContentBlock)
			{
				this.AddContentBlock((ContentBlock)control);
			}
			else if (control is DataBlock)
			{
				((DataBlock)control).ConfirmDataItemsRegistered();
				DataBlocks.Add((DataBlock)control);
			}
			else if (control is TemplatesRoot)
			{
				MyCustomTagFactory.RegisterCustomTags(((TemplatesRoot)control).CustomTags);
			}

			return control;
		}


        #endregion

		#region MessageBundle loading support



		public void FetchMessageBundles()
		{
			if (!this.HasExternalMessageBundles())
			{
				return;
			}

			List<KeyValuePair<Locale, IAsyncResult>> fetchResults = new List<KeyValuePair<Locale, IAsyncResult>>();
			foreach (Locale locale in this.ModulePrefs.Locales)
			{
				fetchResults.Add(new KeyValuePair<Locale, IAsyncResult>(locale, AsyncRequestProcessor.EnqueueRequest(locale.MessageSrc)));
			}

			List<KeyValuePair<Locale, IAsyncResult>> failedFetch = new List<KeyValuePair<Locale, IAsyncResult>>();
			foreach (var keyset in fetchResults)
			{
				IAsyncResult resultHandle = keyset.Value;
				resultHandle.AsyncWaitHandle.WaitOne(800); //wait 800 ms
				if (!resultHandle.IsCompleted)
				{
					failedFetch.Add(keyset);
				}
				else
				{
					try
					{
						RequestResult thisResult = AsyncRequestProcessor.EndRequest(resultHandle);
						if (thisResult.ResponseCode == 200)
						{
							keyset.Key.LoadMessageBundle(thisResult.ResponseString);
						}
					}
					catch { }
				}
			}
			//retry failed
		}


		/// <summary>
		/// Formats all referenced external messagebundles into a singular XML file,
		/// suitible for storage.
		/// </summary>
		/// <returns></returns>
		public string GetConsolidatedMessageBundles()
		{
			return GetConsolidatedMessageBundles(false);
		}

		/// <summary>
		/// Formats all messagebundles into a singular XML file,
		/// suitible for storage.
		/// </summary>
		/// <param name="includeInlineMessageBundles">true to include inline messagebundles</param>
		/// <returns></returns>
		public string GetConsolidatedMessageBundles(bool includeInlineMessageBundles)
		{
			if (this.ModulePrefs.Locales.Count == 0)
			{
				return null;
			}
			using (MemoryStream stream = new MemoryStream())
			{
				WriteConsolidatedMessageBundles(stream, includeInlineMessageBundles);
				stream.Seek(0, SeekOrigin.Begin);
				StreamReader reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Formats all messagebundles into a singular XML file,
		/// suitible for storage.
		/// </summary>
		/// <param name="includeInlineMessageBundles">true to include inline messagebundles</param>
		/// <returns></returns>
		public void WriteConsolidatedMessageBundles(Stream stream, bool includeInlineMessageBundles)
		{
			if (null == stream)
			{
				return;
			}
			StreamWriter writer = new StreamWriter(stream);
			writer.WriteLine(String.Format("<bundles updated=\"{0}\" >", DateTime.UtcNow.ToLongDateString()));

			foreach (Locale locale in this.ModulePrefs.Locales)
			{
				if (string.IsNullOrEmpty(locale.MessageSrc) && !includeInlineMessageBundles)
				{
					continue;
				}
				string tmp;
				writer.Write("<Locale ");
				if (!string.IsNullOrEmpty(locale.Lang))
				{
					tmp = locale.Lang;
					if (tmp.Contains("\""))
					{
						tmp = tmp.Replace("\"", "");
					}
					writer.Write(String.Format("lang=\"{0}\" ", tmp));
				}
				if (!string.IsNullOrEmpty(locale.Country))
				{
					tmp = locale.Country;
					if (tmp.Contains("\""))
					{
						tmp = tmp.Replace("\"", "");
					}
					writer.Write(String.Format("country=\"{0}\" ", tmp));
				}
				writer.WriteLine(">");
				writer.WriteLine(locale.MyMessageBundle.RawTag); //or wrap with <messagebundle> and InnerMarkup
				writer.WriteLine("</Locale>");
			}
			writer.WriteLine("</bundles>");
			writer.Flush();
		}

		/// <summary>
		/// Loads a single messagebundle from XML source
		/// </summary>
		/// <param name="messageBundle"></param>
		public void LoadMessageBundle(string messageBundle)
		{
			if (string.IsNullOrEmpty(messageBundle))
			{
				return;
			}
		}


		/// <summary>
		/// Loads messagebundles from a source created with 
		/// <c>GetConsolidatedMessageBundles</c>
		/// or <c>WriteConsolidatedMessageBundles</c>
		/// </summary>
		/// <param name="messageBundles"></param>
		public void LoadConsolidatedMessageBundles(string messageBundles)
		{
			
			if (string.IsNullOrEmpty(messageBundles))
			{
				return;
			}
			int pos = messageBundles.IndexOf("<bundles");
			int endPos = messageBundles.IndexOf("</bundles");
			if (pos > -1)
			{
				pos = messageBundles.IndexOf(">", pos);
				if (pos > -1)
				{
					pos = pos + 1;
					if (endPos > pos)
					{
						messageBundles = messageBundles.Substring(pos, endPos - pos);
					}
					else
					{
						messageBundles = messageBundles.Substring(pos);
					}
				}
				messageBundles = "<ModulePrefs>" + messageBundles + "</ModulePrefs>";
			}
			Dictionary<string, Locale> myLocales = new Dictionary<string, Locale>();
			foreach (Locale locale in this.ModulePrefs.Locales)
			{
				myLocales.Add(locale.MyMessageBundle.Messages.CultureCode, locale);			
			}

			ModulePrefs tmpModPrefs = new ModulePrefs(null, this);
			tmpModPrefs.LoadTag(messageBundles);
			foreach (Locale locale in tmpModPrefs.Locales)
			{
				string key = locale.MyMessageBundle.Messages.CultureCode;
				if (!myLocales.ContainsKey(key))
				{
					this.ModulePrefs.AddControl(locale);
				}
				else
				{
					myLocales[key].LoadMessageBundle(locale.MyMessageBundle.RawTag);
				}
			}
		}


		/// <summary>
		/// Tests to see if any of the Locale objects reference an external messagebundle.
		/// </summary>
		/// <returns>True if any external messagebundle references found, false otherwise</returns>
		public bool HasExternalMessageBundles()
		{
			foreach (Locale locale in this.ModulePrefs.Locales)
			{
				if (!string.IsNullOrEmpty(locale.MessageSrc))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Test to see if external template libraries are used in this gadget
		/// </summary>
		/// <returns></returns>
		public bool HasExternalTemplateLibraries()
		{
			return this.ModulePrefs.TemplateLibraries.HasLibraries();
		}


		#endregion


		#region Rendering methods


		/// <summary>
        /// Renders this gadget and returns results as a stream.
        /// Favor using <c>RenderContent</c> so the overhead of creating
        /// an internal stream is avoided.
        /// </summary>
        /// <returns></returns>
        public string RenderToString()
        {
            return RenderToString(null);
        }

        /// <summary>
        /// Renders this gadget and returns results as a stream.
        /// Favor using <c>RenderContent</c> so the overhead of creating
        /// an internal stream is avoided.
        /// </summary>
        /// <param name="surfaceName"></param>
        /// <returns></returns>
        public string RenderToString(string surfaceName)
        {
            if (string.IsNullOrEmpty(RawTag) && Controls.Count == 0)
            {
                return null;
            }
            int size = 2048;
            if (!string.IsNullOrEmpty(RawTag))
            {
                size = Math.Max(size, RawTag.Length);
            }
            MemoryStream output = new MemoryStream(size);
            TextWriter w = new StreamWriter(output);

            this.RenderContent(w, surfaceName);
            w.Flush();
            string result = GetStreamContent(output);

            return result;
        }

        private static string GetStreamContent(Stream stream)
        {
            string val = null;
            stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(stream))
            {
                val = reader.ReadToEnd();
            }
            return val;
        }


        /// <summary>
        /// Renders the default surface.
        /// Its preferrable to call RenderContent instead.
        /// </summary>
        /// <param name="writer"></param>
        public override void Render(TextWriter writer)
        {
            RenderContent(writer);
            //throw new Exception("Call RenderContent instead");
        }


        /// <summary>
        /// Parses and renders the content of all ContentBlocks.
        /// Use the overload which specifies a surfaceName instead
        /// </summary>
        /// <param name="writer"></param>
        public void RenderContent(TextWriter writer)
        {
            RenderContent(writer, null);
        }


        /// <summary>
        /// Parses the content of only the ContentBlocks identified in the views array
        /// </summary>
        /// <remarks>
        /// Passing a null surfaceName will cause all surfaces to render
        /// </remarks>
        /// <param name="writer"></param>
        /// <param name="surfaceName">Master surface name to render.  This and all sub-views will render.</param>
        public void RenderContent(TextWriter writer, string surfaceName)
        {
			if (NeedsReparse)
			{
				ReParse(true);
			}
			if (!IsParsed)
            {
                Parse();
            }

			//Look to resolver
			MasterDataContext.ActiveViewScope = surfaceName;
			if (MyDataResolver != null)
			{
				MyDataResolver.ResolveValues(MasterDataContext);
			}

            int i = 0;

            StringBuilder afterBlock = new StringBuilder();

            afterBlock.AppendLine("osmlViews = {};");

            foreach (ContentBlock content in ContentBlocks)
            {
                content.ID = "appid" + this.AppID.ToString() + "_" + string.Format(ContentBlock.CLIENTID_TEMPLATE, i);
                i++;

                if (null == surfaceName || IsQualifiedView(surfaceName, content)
					|| IsAnonymousView(content))
                {
					bool isSubview = IsSubView(content, surfaceName);
					if (isSubview)
					{
						content.IncludeWrappingDivs = RenderingOptions.DivWrapContentSubViews;
						if (true == RenderingOptions.DivWrapContentSubViews)
						{
							content.RenderAsHidden = true;
						}
					}
					else
					{
						content.IncludeWrappingDivs = RenderingOptions.DivWrapContentBlocks;
					}
                    
                    content.Render(writer);
                }
            }


            if (RenderingOptions.ClientRenderDataContext)
            {
				string clientDataPutTemplate = "opensocial.data.getDataContext().putDataSet(##KEY##, ##DATA##);";
				writer.Write(JS_START_BLOCK_TAGS);
                MyDataContext.WriteClientContext(writer, clientDataPutTemplate);

				writer.WriteLine(@"
gadgets.util.registerOnLoadHandler(function(){
	MyOpenSpace.ClientRequestProcessor.resolveRequests();
});");

				writer.Write(JS_END_BLOCK_TAGS);

				WriteClientDeferredDataControls(writer);
            }

			//render client templates?
			if (ClientRenderCustomTemplates)
			{
				RenderCustomTemplates(writer, false);

			}


            //update counters
            if (GadgetMaster.Counters.RenderCount.IsGlobalCounterEnabled)
            {
                GadgetMaster.Counters.RenderCount.IncrementGlobalCount();
            }

            writer.Flush();
        }

		/// <summary>
		/// If a single Content block is specified in the gadget and no view is identified,
		/// it is an anonymous gadget
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		private bool IsAnonymousView(ContentBlock content)
		{
			return (ContentBlocks.Count == 1 && content.ViewMask <= 1
				&& (!HasViewsDefined() || HasViewDefined(0)));
		}

		/// <summary>
		/// Renders the custom tag templates section to client
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="asDomElements">Renders the template as DOM elements if true, as javascript if false</param>
		private void RenderCustomTemplates(TextWriter writer, bool asDomElements)
		{

			if (!asDomElements)
			{
				writer.Write(JS_START_BLOCK_TAGS);
				this.MyCustomTagFactory.RenderClientTemplates(writer, asDomElements);

//            writer.WriteLine(@"
//gadgets.util.registerOnLoadHandler(function(){
//	opensocial.template.loadRegisteredTemplates(true);
//});");
				writer.Write(JS_END_BLOCK_TAGS);
			}
			else
			{
				//TODO - de-hard code this value
				writer.WriteLine("<div id=\"" + CLIENT_TEMPLATE_BLOCK_ID + "\" style=\"display:none;visibility:hidden;\">");
				this.MyCustomTagFactory.RenderClientTemplates(writer);
				writer.WriteLine("\n</div>");
				//now add call to register these templates
				writer.Write(JS_START_BLOCK_TAGS);

//            writer.WriteLine(@"
//gadgets.util.registerOnLoadHandler(function(){
//	opensocial.template.loadRegisteredTemplates();
//});");

				writer.Write(JS_END_BLOCK_TAGS);
			}
		}

		/// <summary>
		/// Invokes the render method to emit the script for loading
		/// client-deferred data controls
		/// </summary>
		/// <param name="stream"></param>
		public void WriteClientDeferredDataControls(System.IO.TextWriter writer)
		{
			//check for client-side data tags
			foreach (KeyValuePair<string, DataItem> keyset in MyDataContext.MasterData)
			{
				DataItem item = keyset.Value;
				if (null != item.DataControl
					&& item.DataControl is BaseDataControl
					&& ((BaseDataControl)item.DataControl).UseClientDataResolver)
				{
					((BaseDataControl)item.DataControl).Render(writer);
				}
			}

		}


		/// <summary>
		/// checks to see if the given data context has tags requiring
		/// client-side data resolution
		/// </summary>
		/// <param name="MyDataContext"></param>
		/// <returns></returns>
		static bool RequiresClientDataResolvers(DataContext dataContext)
		{
			if (null == dataContext)
			{
				return false;
			}
			foreach (KeyValuePair<string, DataItem> keyset in dataContext.MasterData)
			{
				DataItem item = keyset.Value;
				if (item.DataControl is BaseDataControl
					&& ((BaseDataControl)item.DataControl).UseClientDataResolver)
				{
					return true;
				}
			}
			return false;
		}

        #endregion


        /// <summary>
        /// Tests to see if this is a sub-view, but not a top-level view
        /// </summary>
        /// <param name="content"></param>
        /// <param name="surfaceName"></param>
        /// <returns></returns>
        internal bool IsSubView(ContentBlock content, string view)
        {
			return content.IsSubView(view);
        }

        /// <summary>
        /// Tests to see if a given content block qualifies as being part of a view.
        /// Content blocks may participate in multiple views or may be subviews
        /// </summary>
        /// <param name="surfaceName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private bool IsQualifiedView(string view, ContentBlock content)
        {
			return content.IsQualifiedView(view);
        }

        #region IDisposable Members

        override protected void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                ClearAllControls();
				//if (null != _errors)
				//{
				//    _errors.Parent = null;
				//    _errors = null;
				//}
            }


        }

        #endregion

        /// <summary>
        /// Clears all controls from the Controls collection
        /// and all specialized collections except external template libraries
        /// </summary>
        private void ClearAllControls()
        {
            this.InDisposer = true; //fake out to prevent stack overflow
            if (_contentBlocks != null && _contentBlocks.Count > 0)
            {
                foreach (ContentBlock block in _contentBlocks)
                {
                    block.Dispose();
                }
                _contentBlocks.Clear();
            }
            if (Controls.Count > 0)
            {
                foreach (BaseGadgetControl item in Controls)
                {
                    item.Dispose();
                }
                Controls.Clear();
            }
			//clear custom tags
			this.MyCustomTagFactory.CustomTags.Clear();
            this.InDisposer = false;
        }

		#region ICloneable Members

		public object Clone()
		{
			if (!this.IsParsed)
			{
				this.Parse();
			}
			GadgetMaster gm;
			if (MyOffset != null)
			{
				gm = new GadgetMaster(this.MyControlFactory, this.RawTag, MyOffset.Clone() as OffsetItem);
			}
			else
			{
				gm = new GadgetMaster(this.MyControlFactory, this.RawTag);
				gm.Parse();
			}

			if (gm.HasExternalMessageBundles())
			{
				gm.LoadConsolidatedMessageBundles(this.SerializationExternalMessageBundle);
			}
			if (gm.HasExternalTemplateLibraries())
			{

			}
			//todo - accomodate external templates
			return gm;
		}

		#endregion
	}
}

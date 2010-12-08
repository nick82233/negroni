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
using Negroni.OpenSocial.OSML.Controls;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Represents the &lt; ModulePrefs &gt; section in resolved GadgetXML markup
	/// </summary>
	/// <summary>
	/// Gadget module preferences section
	/// </summary>
	[MarkupTag("ModulePrefs")]
	[ContextGroup(typeof(GadgetMaster))]
	public class ModulePrefs : BaseContainerControl
	{

        private List<Locale> _locales = null;
        private List<Link> _links = null;
		private List<ModuleRequire> _requiredFeatures = null;
		private List<ModuleOptional> _optionalFeatures = null;


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


        public ModulePrefs() 
		{
			this.MyParseContext = new ParseContext(typeof(GadgetMaster));
			this.SecurityPolicy.SecurityPolicyChanged += new SecurityPolicyChangeDelegate(SecurityPolicy_SecurityPolicyChanged);
		}

        public ModulePrefs(string markup, GadgetMaster master) 
            : this()
		{
			SetGadgetMaster(master);
			LoadTag(markup);
		}

		public ModulePrefs(string markup, OffsetItem offset, GadgetMaster master) 
            : this()
		{
			SetGadgetMaster(master);
			this.MyOffset = offset;
			LoadTag(markup);
		}

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);

			if (this.HasAttributes())
			{
				this.Author = GetAttribute("author");
                this.AuthorEmail = GetAttribute("author_email");
                this.Category = GetAttribute("category");
                this.Category2 = GetAttribute("category2");
                this.Description = GetAttribute("description");
                this.Screenshot = GetAttribute("screenshot");
                this.Thumbnail = GetAttribute("thumbnail");
                this.Title = GetAttribute("title");
				this.TitleUrl = GetAttribute("title_url");
			}
		}

		void SecurityPolicy_SecurityPolicyChanged(object sender, SecurityPolicyChangedEventArgs e)
		{
			this.MyRootMaster.MasterDataContext.Settings.SecurityPolicy.EL_Escaping = e.UpdatedEL_Escaping;
			this.MyRootMaster.MasterDataContext.Settings.SecurityPolicy.UserPrefEscaping = e.UpdatedUserPrefEscaping;
		}

		/// <summary>
		/// Adds a control to the internal collections.
		/// If the control is a special control - ex: Link
		/// it will be added to the correct special List.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			base.AddControl(control); //make sure it is in the main controls collection
			if (null == control)
			{
				return null;
			}
			if (control is ModuleFeature)
			{
				ModuleFeature feature = (ModuleFeature)control;
				if (control is ModuleRequire)
				{
					RequiredFeatures.Add((ModuleRequire)control);
				}
				else if (control is ModuleOptional)
				{
					OptionalFeatures.Add((ModuleOptional)control);
				}
				//MySpace settings - should be Optional element, but we'll allow either optional or require
				if (MySpaceAppSettings.FEATURE_KEY.Equals(feature.Feature, StringComparison.InvariantCultureIgnoreCase))
				{
					MySpaceAppSettings.LoadFeatureSettings(feature);
				}
				//MySpace settings - should be Optional element, but we'll allow either optional or require
				else if (MySpaceViewSettings.FEATURE_KEY.Equals(feature.Feature, StringComparison.InvariantCultureIgnoreCase))
				{
					MySpaceViewSettings.LoadFeatureSettings(feature);
				}
				//OpenSocial templates
				else if (ExternalTemplates.FEATURE_KEY.Equals(feature.Feature, StringComparison.InvariantCultureIgnoreCase))
				{
					TemplateLibraries.LoadFeatureSettings(feature);
				}
				//Security Policy
				else if (GadgetSecurityPolicy.FEATURE_KEY.Equals(feature.Feature, StringComparison.InvariantCultureIgnoreCase))
				{
					this.SecurityPolicy.LoadFeatureSettings(feature);
				}


			}
			else if (control is Locale)
			{
				this.Locales.Add((Locale)control);
			}
			else if (control is Link)
			{
				this.Links.Add((Link)control);
			}
			else if (control is ModuleIcon)
			{
				ModuleIcon icon = (ModuleIcon)control;
				if (icon.Mode == "base64")
				{
					IconUrl = null;
				}
				else
				{
					IconUrl = icon.Src;
				}
				this.IconControl = icon;
			}
			return control;
		}


        public override void Render(TextWriter writer)
        {
            return;
        }

        public List<Link> Links
        {
            get
            {
				if (_links == null)
				{
					_links = new List<Link>();
				}

                return _links;
            }
        }

        public List<Locale> Locales
        {
            get
            {
				if (_locales == null)
				{
					_locales = new List<Locale>();
				}

                return _locales;
            }
        }

		/// <summary>
		/// Required features requested by the app gadget.
		/// These are defined with &lt;Require &gt; directives
		/// in the ModulePrefs section
		/// </summary>
		public List<ModuleRequire> RequiredFeatures
		{
			get
			{
				if (_requiredFeatures == null)
				{
					_requiredFeatures = new List<ModuleRequire>();
				}

				return _requiredFeatures;
			}
		}
		/// <summary>
		/// Optional features requested by the app gadget.
		/// These are defined with &lt;Optional &gt; directives
		/// in the ModulePrefs section
		/// </summary>
		public List<ModuleOptional> OptionalFeatures
		{
			get
			{
				if (null == _optionalFeatures)
				{
					_optionalFeatures = new List<ModuleOptional>();
				}

				return _optionalFeatures;
			}
		}

		public string Author { get; set; }
        
		public string AuthorEmail { get; set; }

        public string Category { get; set; }

        public string Category2 { get; set; }

        public string Description { get; set; }

        public string Screenshot { get; set; }

		/// <summary>
		/// Title of this gadget app
		/// </summary>
        public string Title { get; set; }

		/// <summary>
		/// Optional string that provides a URL that the gadget title links to. 
		/// For example, you could link the title to a webpage related to the gadget. 
		/// </summary>
		public string TitleUrl { get; set; }

		/// <summary>
		/// Url to the public Thumbnail, if specified.
		/// </summary>
        public string Thumbnail { get; set; }

		/// <summary>
		/// Url to the public Icon, if specified.
		/// </summary>
		public string IconUrl { get; set; }

		/// <summary>
		/// Icon control tag.  Use the IconUrl in most instances.
		/// When that is empty this might contain a base64 encoded icon,
		/// in which case use GetIconImage
		/// </summary>
		public ModuleIcon IconControl { get; set; }



		private ExternalTemplates _templateLibraries = null;

		/// <summary>
		/// Reference to external templates requested for this gadget.
		/// External templates are registered with the CustomTagFactory for
		/// this gadget as well.
		/// </summary>
		public ExternalTemplates TemplateLibraries
		{
			get
			{
				if (_templateLibraries == null)
				{
					_templateLibraries = new ExternalTemplates();
				}
				return _templateLibraries;
			}
		}


		private MySpaceAppSettings _myspaceAppSettings = null;




		/// <summary>
		/// Accessor for myspaceAppSettings.
		/// Performs lazy load upon first request
		/// </summary>
		public MySpaceAppSettings MySpaceAppSettings
		{
			get
			{
				if (_myspaceAppSettings == null)
				{
					_myspaceAppSettings = new MySpaceAppSettings();
				}
				return _myspaceAppSettings;
			}
		}


		private Dictionary<string, object> _implementationAppSettings = null;

		/// <summary>
		/// This holds implementation-specific settings
		/// </summary>
		public Dictionary<string, object> ImplementationAppSettings
		{
			get
			{
				if (_implementationAppSettings == null)
				{
					_implementationAppSettings = new Dictionary<string, object>();
				}
				return _implementationAppSettings;
			}
			set
			{
				_implementationAppSettings = value;
			}
		}

		private MySpaceViewSettings _myspaceViewSettings = null;

		/// <summary>
		/// Accessor for myspaceViewSettings.
		/// Performs lazy load upon first request
		/// </summary>
		public MySpaceViewSettings MySpaceViewSettings
		{
			get
			{
				if (_myspaceViewSettings == null)
				{
					_myspaceViewSettings = new MySpaceViewSettings();
				}
				return _myspaceViewSettings;
			}
		}

		private GadgetSecurityPolicy _securityPolicy = null;

		/// <summary>
		/// Accessor for SecurityPolicy.
		/// Performs lazy load upon first request
		/// </summary>
		public GadgetSecurityPolicy SecurityPolicy
		{
			get
			{
				if (_securityPolicy == null)
				{
					_securityPolicy = new GadgetSecurityPolicy();
				}
				return _securityPolicy;
			}
		}

	}
}

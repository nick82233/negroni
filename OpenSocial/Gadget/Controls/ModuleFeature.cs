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
	/// Base class for an optional or required feature.
	/// Features may include optional Params
	/// </summary>
	/// <summary>
	/// </summary>
	[ContextGroup(typeof(ModulePrefs))]
	abstract public class ModuleFeature : BaseContainerControl
	{
		/// <summary>
		/// Prefix string used for opensocial features
		/// </summary>
		public const string OPENSOCIAL_FEATURE_PREFIX = "opensocial-";

		public ModuleFeature() 
		{
			this.MyParseContext = new ParseContext(typeof(ModulePrefs));
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="content">Raw content characters</param>
		public ModuleFeature(string markup)
			: this()
		{
			LoadTag(markup);
		}

		string _feature = null;

		/// <summary>
		/// Feature named by this tag
		/// </summary>
		public string Feature
		{
			get
			{
				if (string.IsNullOrEmpty(_feature))
				{
					return string.Empty;
				}
				else
				{
					return _feature;
				}
			}
			set{
				_feature = value;
			}
		}


		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			if (this.HasAttributes())
			{
				this.Feature = GetAttribute("feature");
			}
		}

		private List<ParamControl> _params = null;

		/// <summary>
		/// Accessor for params.
		/// Performs lazy load upon first request
		/// </summary>
		public List<ParamControl> Params
		{
			get
			{
				if (_params == null)
				{
					_params = new List<ParamControl>();
				}
				return _params;
			}
		}

		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			base.AddControl(control);
			if (control is ParamControl)
			{
				Params.Add((ParamControl)control);
			}
			return control;
		}

		/// <summary>
		/// Render will invoke outputting this as client side javascript
		/// </summary>
		/// <param name="writer"></param>
		override public void Render(TextWriter writer)
		{
			return;
		}

	}
}

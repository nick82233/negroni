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
using System.Text;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Encapsulation of external template references used in this gadget
	/// This information is defined in markup by Param tags under the
	/// &lt;Require feature="opensocial-templates" &gt;
	/// </summary>
	/// <example>
	/// Below would load a library from example.com
	/// &lt;Require feature="opensocial-templates" &gt;
	/// &lt;Param name="requireLibrary"&gt;http://www.example.com/templates.xml&lt;/Param&gt;
	/// &lt;/Require &gt;
	/// </example>
	public class ExternalTemplates
	{
		/// <summary>
		/// Key used in gadget markup to identify MySpace settings
		/// </summary>
		public const string FEATURE_KEY = "opensocial-templates";
		/// <summary>
		/// Param name used to require a library
		/// </summary>
		public const string PARAM_REQUIRE_LIBRARY = "requirelibrary";


		public ExternalTemplates() { }

		public ExternalTemplates(ModuleFeature feature)
		{
			LoadFeatureSettings(feature);
		}

		public void LoadFeatureSettings(ModuleFeature feature)
		{
			foreach (ParamControl param in feature.Params)
			{
				AddParamSetting(param);
			}
		}

		/// <summary>
		/// Resolves a param control value and applies its setting
		/// </summary>
		/// <param name="param"></param>
		void AddParamSetting(ParamControl param)
		{
			if (param == null || string.IsNullOrEmpty(param.Name))
			{
				return;
			}
            string key = param.Name.ToLowerInvariant();
			switch (key)
			{
				case PARAM_REQUIRE_LIBRARY:
					AddExternalTemplate(param.InnerMarkup);
					break;
			}

		}

		/// <summary>
		/// Tests to see if external template libraries are defined for this gadget
		/// </summary>
		/// <returns></returns>
		public bool HasLibraries()
		{
			if (null == _libraries || _libraries.Count == 0)
			{
				return false;
			}
			return true;
		}


		private List<TemplateLibraryDef> _libraries = null;

		/// <summary>
		/// List of URLs to external template libraries
		/// </summary>
		public List<TemplateLibraryDef> Libraries
		{
			get
			{
				if (_libraries == null)
				{
					_libraries = new List<TemplateLibraryDef>();
				}
				return _libraries;
			}
		}


		/// <summary>
		/// Adds a new external template library reference
		/// </summary>
		/// <param name="libraryUri"></param>
		public void AddExternalTemplate(string libraryUri)
		{
			Libraries.Add(new TemplateLibraryDef(libraryUri));
		}


	}
}

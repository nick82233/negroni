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

namespace Negroni.TemplateFramework.Configuration
{
	/// <summary>
	/// Configuration section to handle registration of control factories utilizing
	/// the OpenSocial ControlFactory framework.
	/// </summary>
	public class OpenSocialControlFactoryConfig
	{
        /// <summary>
        /// Fallback filename for manual load if the OpenSocialControlFactories config section
        /// is not specified in the app.config/web.config
        /// </summary>
        static readonly string CONFIG_FILE = "OpenSocialControlFramework.config";

		static ControlFactorySection configSection = ControlFactorySection.GetSection();

		static private Dictionary<string, List<string>> _controlFactories = new Dictionary<string, List<string>>();

		static private Dictionary<string, IncludeExcludeSet> _filterDirectives = new Dictionary<string, IncludeExcludeSet>();


		/// <summary>
		/// Represents finer-grained control for assembly control loading
		/// </summary>
		class IncludeExcludeSet
		{
			public IncludeExcludeSet() { }

			public IncludeExcludeSet(string includePattern, string excludePattern)
			{
				IncludePattern = includePattern;
				ExcludePattern = excludePattern;
			}

			public string IncludePattern = null;
			public string ExcludePattern = null;
		}

		/// <summary>
		/// Formats the dictionary key for include/exclude filter directives
		/// </summary>
		/// <param name="controlFactoryKey"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		static string GetFilterKey(string controlFactoryKey, string assemblyName)
		{
			return controlFactoryKey + "_" + assemblyName;
		}
		/// <summary>
		/// Tests to see if there are include/exclude directives defined for the current assembly
		/// </summary>
		/// <param name="controlFactoryKey"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		static public bool HasControlFilters(string controlFactoryKey, string assemblyName)
		{
			return _filterDirectives.ContainsKey(GetFilterKey(controlFactoryKey, assemblyName));
		}
		/// <summary>
		/// Returns the include control filtering directive for this assembly
		/// </summary>
		/// <param name="controlFactoryKey"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		static public string GetIncludeFilter(string controlFactoryKey, string assemblyName)
		{
			if (!HasControlFilters(controlFactoryKey, assemblyName))
			{
				return null;
			}
			return _filterDirectives[GetFilterKey(controlFactoryKey, assemblyName)].IncludePattern;
		}

		/// <summary>
		/// Returns the exclude control filtering directive for this assembly
		/// </summary>
		/// <param name="controlFactoryKey"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		static public string GetExcludeFilter(string controlFactoryKey, string assemblyName)
		{
			if (!HasControlFilters(controlFactoryKey, assemblyName))
			{
				return null;
			}
			return _filterDirectives[GetFilterKey(controlFactoryKey, assemblyName)].ExcludePattern;
		}


		/// <summary>
		/// Registered Control Factories
		/// </summary>
		static public Dictionary<string, List<string>> ControlFactories
		{
			get{
				return _controlFactories;
			}
		}

		/// <summary>
		/// Initialize the information from config
		/// </summary>
		static OpenSocialControlFactoryConfig()
		{
			ReloadConfiguration();
		}

		public static void ReloadConfiguration(){
			_controlFactories.Clear();
			_filterDirectives.Clear();
			//_controlFactories
			if (configSection.OpenSocialControlFactories.Count > 0)
			{
				foreach (ControlFactoryElement factory in configSection.OpenSocialControlFactories)
				{
					List<string> assemblies = new List<string>();

					foreach (MySpaceAssemblyElement item in factory.MySpaceAssemblies)
					{
						assemblies.Add(item.Name);
						if (item.HasFilters())
						{
							_filterDirectives.Add(GetFilterKey(factory.Key, item.Name), 
								new IncludeExcludeSet(item.Include, item.Exclude) );
						}
					}
					_controlFactories.Add(factory.Key, assemblies);

				}

			}

		}


	}
}

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
using System.Configuration;
using System.IO;
using System.Text;
using Negroni.TemplateFramework.Configuration.ParsingControls;

namespace Negroni.TemplateFramework.Configuration
{
	/// <summary>
	/// Configuration section to handle registration of control factories utilizing
	/// the OpenSocial ControlFactory framework.
	/// </summary>
	public class NegroniFrameworkConfig
	{
		/// <summary>
		/// ControlFactory key used for the configuration parser
		/// </summary>
		public const string CONFIGPARSER_CONTROLFACTORY = "NegroniConfig";

        /// <summary>
        /// Fallback filename for manual load if the OpenSocialControlFactories config section
        /// is not specified in the app.config/web.config
        /// </summary>
        static readonly string CONFIG_FILE = "NegroniFramework.config";

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
		static NegroniFrameworkConfig()
		{
			ReloadConfiguration();
		}

		public static void ReloadConfiguration(){
			_controlFactories.Clear();
			_filterDirectives.Clear();

			INegroniFactoriesSection negroniFactoryConfig = null;

				string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NegroniFrameworkConfig.CONFIG_FILE);
				ControlFactory configParser = null;
				try
				{
					if (!ControlFactory.IsFactoryDefined(CONFIGPARSER_CONTROLFACTORY))
					{
						configParser = ControlFactory.CreateControlFactory(CONFIGPARSER_CONTROLFACTORY, System.Reflection.Assembly.GetExecutingAssembly());
					}
					else
					{
						configParser = ControlFactory.GetControlFactory(CONFIGPARSER_CONTROLFACTORY);
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("Critical failure loading config: " + ex.Message);
					return;
				}
				string configContents = null;
				try
				{
					using (StreamReader sr = File.OpenText(configPath))
					{
						configContents = sr.ReadToEnd();
					}
				}
				catch (Exception fex)
				{
					System.Diagnostics.Debug.WriteLine("Failure loading default config file: " + fex.Message);
					return;
				}
				NegroniControlFactories ncf = new NegroniControlFactories();
				ncf.MyControlFactory = configParser;
				ncf.LoadTag(configContents);

				negroniFactoryConfig = ncf;


			//_controlFactories
			if (negroniFactoryConfig != null && negroniFactoryConfig.ControlFactories.Count > 0)
			{
				foreach (INegroniControlFactory factory in negroniFactoryConfig.ControlFactories)
				{
					List<string> assemblies = new List<string>();

					foreach (INegroniAssembly item in factory.ControlAssemblies)
					{
						assemblies.Add(item.Name);
						if (item.HasFilters())
						{
							_filterDirectives.Add(GetFilterKey(factory.Key, item.Name),
								new IncludeExcludeSet(item.Include, item.Exclude));
						}
					}
					_controlFactories.Add(factory.Key, assemblies);

				}

			}

		}


	}
}

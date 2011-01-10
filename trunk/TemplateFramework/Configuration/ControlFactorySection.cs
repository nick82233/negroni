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
using System.Configuration;

namespace Negroni.TemplateFramework.Configuration
{
	/// <summary>
	/// Main configuration section for OpenSocial control factory
	/// </summary>
	internal class ControlFactorySection : ConfigurationSection, INegroniFactoriesSection
	{
		public const string SectionName = "NegroniControlFactories";
		const string FactoryCollectionName = "";


		public ControlFactorySection()
		{
			ControlFactories = new List<INegroniControlFactory>();
		}

		public static string DefaultSectionName
		{
			get { return SectionName; }
		}

		/// <summary>
		/// Get the section from configuration.  
		/// If the section is missing return an empty section
		/// </summary>
		/// <returns></returns>
		public static ControlFactorySection GetSection()
		{
			ControlFactorySection retval = ConfigurationManager.GetSection(SectionName) as ControlFactorySection;
			if (retval == null)
			{
				// Return with default values
				retval = new ControlFactorySection();
			}
			return retval;
		}

		/// <summary>
		/// Defined control factories
		/// </summary>
		[ConfigurationProperty(FactoryCollectionName, IsDefaultCollection = true)]
		public ControlFactoryElementCollection OpenSocialControlFactories
		{
			get
			{
				ControlFactoryElementCollection retval = base[FactoryCollectionName] as ControlFactoryElementCollection;
				if (retval == null)
				{
					retval = new ControlFactoryElementCollection();
					base[FactoryCollectionName] = retval;
				}
				for (int i = 0; i < retval.Count; i++)
				{
					ControlFactories.Add(retval[i]);
				}
				return retval;
			}
		}



		public List<INegroniControlFactory> ControlFactories
		{
			get; private set;
		}
	}
}

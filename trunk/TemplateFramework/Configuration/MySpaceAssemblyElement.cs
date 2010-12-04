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
	public class MySpaceAssemblyElement : ConfigurationElement
	{
		const string KEY_NAME = "name";
		const string KEY_INCLUDE = "include";
		const string KEY_EXCLUDE = "exclude";

		/// <summary>
		/// Fully qualified assembly name
		/// </summary>
		[ConfigurationProperty(KEY_NAME)]
		public string Name
		{
			get
			{
				return base[KEY_NAME] as string;
			}
			set
			{
				base[KEY_NAME] = value;
			}
		}
		/// <summary>
		/// Include control filtering pattern for this assembly
		/// </summary>
		[ConfigurationProperty(KEY_INCLUDE)]
		public string Include
		{
			get
			{
				return base[KEY_INCLUDE] as string;
			}
			set
			{
				base[KEY_INCLUDE] = value;
			}
		}
		/// <summary>
		/// Exclude control filtering pattern for this assembly
		/// </summary>
		[ConfigurationProperty(KEY_EXCLUDE)]
		public string Exclude
		{
			get
			{
				return base[KEY_EXCLUDE] as string;
			}
			set
			{
				base[KEY_EXCLUDE] = value;
			}
		}
		/// <summary>
		/// Checks to see if there are include or exclude directives for this assembly
		/// </summary>
		/// <returns></returns>
		public bool HasFilters()
		{
			return !string.IsNullOrEmpty(this.Include) || !string.IsNullOrEmpty(this.Exclude);
		}
	}
}

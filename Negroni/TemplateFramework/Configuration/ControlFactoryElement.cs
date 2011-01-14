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
	internal class ControlFactoryElement : ConfigurationElement, INegroniControlFactory
	{

		public ControlFactoryElement()
		{
			ControlAssemblies = new List<INegroniAssembly>();
		}

		const string KeyName = "key";
		const string AssemblyollectionName = "";
		[ConfigurationProperty(KeyName)]
		public string Key
		{
			get
			{
				return base[KeyName] as string;
			}
			set
			{
				base[KeyName] = value;
			}
		}

		[ConfigurationProperty(AssemblyollectionName, IsDefaultCollection = true)]
		public ControlAssemblyElementCollection NegroniAssemblies
		{
			get
			{
				ControlAssemblyElementCollection retval = base[AssemblyollectionName] as ControlAssemblyElementCollection;
				if (retval == null)
				{
					retval = new ControlAssemblyElementCollection();
					base[AssemblyollectionName] = retval;

					//duplicate into the interface collection
					for (int i = 0; i < retval.Count; i++)
					{
						ControlAssemblies.Add(retval[i]);
					}
				}
				return retval;
			}
		}

		public List<INegroniAssembly> ControlAssemblies { get; set; }
	}
}

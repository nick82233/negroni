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
	public class ControlFactoryElement : ConfigurationElement
	{
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
		public MySpaceAssemblyElementCollection MySpaceAssemblies
		{
			get
			{
				MySpaceAssemblyElementCollection retval = base[AssemblyollectionName] as MySpaceAssemblyElementCollection;
				if (retval == null)
				{
					retval = new MySpaceAssemblyElementCollection();
					base[AssemblyollectionName] = retval;
				}
				return retval;
			}
		}
	}
}

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
	[ConfigurationCollection(typeof(ControlFactoryElement), AddItemName = "NegroniControlFactory")]
	internal class ControlFactoryElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ControlFactoryElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			ControlFactoryElement elem = element as ControlFactoryElement;
			if (elem == null)
			{
				throw new ArgumentException("Argument must be non-null and of type ControlFactoryElement", "element");
			}
			return elem.Key == null ? string.Empty : elem.Key;
		}

		public void Add(ControlFactoryElement element)
		{
			this.BaseAdd(element);
		}

		public void Remove(ControlFactoryElement element)
		{
			this.BaseRemove(this.GetElementKey(element));
		}

		public void Clear()
		{
			this.BaseClear();
		}

		public ControlFactoryElement this[int index]
		{
			get
			{
				if (this.Count <= index)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return base.BaseGet(index) as ControlFactoryElement;
			}
		}

		public ControlFactoryElement LookupByKey(string key)
		{
			ControlFactoryElement retval = base.BaseGet(key) as ControlFactoryElement;
			if (retval == null)
			{
				throw new ArgumentException("key");
			}
			return retval;
		}
	}
}

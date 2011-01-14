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

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Attribute class to identify a unique key to
	/// be used when storing offsets for the Control.
	/// If not specified, a normalized version of the MarkupTag defined for the control
	/// will be used instead.
	/// This is not a required attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class OffsetKeyAttribute : System.Attribute
	{
		/// <summary>
		/// Defines Key string used in offset when matching this control
		/// </summary>
		/// <param name="key">Unique string used to identify this control in OffsetList</param>
		public OffsetKeyAttribute(string key)
		{
			Key = key;
		}

		/// <summary>
		/// Unique key string used in the offset definition
		/// </summary>
		public string Key { get; set; }
	}
}

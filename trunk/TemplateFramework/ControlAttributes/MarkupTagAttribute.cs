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
	/// Attribute class to define markup tag to search for on a given control.
	/// This is not a required attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class MarkupTagAttribute : System.Attribute
	{
		/// <summary>
		/// Defines markup tag used to define this control.
		/// </summary>
		/// <param name="markupTag">Markup tag (without angle brackets - ex: os:ControlName)</param>
		public MarkupTagAttribute(string markupTag)
		{
			MarkupTag = markupTag;
		}

		/// <summary>
		/// Markup tag defined for this control type
		/// </summary>
		public string MarkupTag { get; set; }
	}
}

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
	/// Used to define an attribute which may be used in arbitrary elements 
	/// as an alternative to the element tag identified by <c>MarkupTagAttribute</c>.
	/// This is not a required attribute.
	/// </summary>
	/// <remarks>
	/// This is used for control flow statements which may exist as an element or
	/// an attribute to an arbitrary element.  Specific examples are os:If and os:Repeat
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class AttributeTagAlternativeAttribute : System.Attribute
	{
		/// <summary>
		/// Defines attribute key which identifies this as a control tag
		/// </summary>
		/// <param name="attributeName">Attribute name in the markup tag</param>
		public AttributeTagAlternativeAttribute(string attributeName)
		{
			AttributeName = attributeName;
		}
		/// <summary>
		/// Defines attribute key which identifies this as a control tag
		/// </summary>
		/// <param name="attributeName">Attribute name in the markup tag</param>
		/// <param name="precedenceWeight">Importance of this tag relative to other attribute-defined tags. Larger values indicate greater weight.</param>
		public AttributeTagAlternativeAttribute(string attributeName, int precedenceWeight)
		{
			AttributeName = attributeName;
			PrecedenceWeight = precedenceWeight;
		}

		/// <summary>
		/// Attribute name key used in the tag definition
		/// </summary>
		public string AttributeName { get; set; }

		/// <summary>
		/// Importance of this tag relative to other attribute-defined tags.
		/// This allows multiple tags to be present where one takes precedence over another.
		/// Default value is 0 [zero]
		/// </summary>
		public int PrecedenceWeight { get; set; }
	}
}

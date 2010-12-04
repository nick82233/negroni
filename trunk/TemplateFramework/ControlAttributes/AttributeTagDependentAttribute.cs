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
	/// Used to define a dependent attribute which must also exist for the control definition
	/// to be satisfied in the markup.  Used for tags which are overridden and have different
	/// behavior depending on attributes.
	/// This is not a required attribute.
	/// </summary>
	/// <remarks>
	/// An example of this usage is the &lt;script&gt; tag.  
	/// In normal flow it represents client-side javascript.  The tag may alternately be declared
	/// with a type='os-template' (a template script) or type='os-data' (a data script).
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AttributeTagDependentAttribute : System.Attribute
	{

		/// <summary>
		/// Defines attribute key which identifies this as a control tag.
		/// Any value is considered valid if key is present.
		/// </summary>
		/// <param name="attributeName"></param>
		public AttributeTagDependentAttribute(string attributeName)
			: this(attributeName, null)
		{
		}

		/// <summary>
		/// Defines attribute key which identifies this as a control tag.
		/// Must have specified value to be considered valid.
		/// </summary>
		/// <param name="attributeName">Attribute name in the markup tag</param>
		public AttributeTagDependentAttribute(string attributeName, string attributeValue)
		{
			AttributeName = attributeName;
			AttributeValue = attributeValue;
		}

		/// <summary>
		/// Attribute name key used in the tag definition
		/// </summary>
		public string AttributeName { get; set; }

		/// <summary>
		/// Value required in the named attribute to satisfy the dependency condition.
		/// </summary>
		public string AttributeValue { get; set; }

	}
}

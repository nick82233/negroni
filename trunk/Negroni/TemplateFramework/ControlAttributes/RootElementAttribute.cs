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
	/// Attribute class to mark a class as defining to very top level root control.
	/// There should only be zero or 1 Root controls defined in a control catalog.
	/// This is not a required attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class RootElementAttribute : System.Attribute
	{
		public RootElementAttribute()
		{}

		public RootElementAttribute(bool isDefaultParseContext)
		{
			IsDefaultParseContext = isDefaultParseContext;		
		}

		private bool _isDefaultParseContext = true;

		/// <summary>
		/// Identifies if the root element should be treated as the default parsing context
		/// for all controls.  By default this is true.
		/// </summary>
		public bool IsDefaultParseContext
		{
			get { return _isDefaultParseContext; }
			set { _isDefaultParseContext = value; }
		}
		

	}
}

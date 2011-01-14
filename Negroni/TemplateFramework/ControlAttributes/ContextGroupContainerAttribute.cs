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
	/// Attribute class to mark a class as being a ContextGroup container.
	/// This attribute is only required if a class is going to act as a
	/// ContextGroup container for scoped controls, but the child controls
	/// are defined in a separate assembly.
	/// 
	/// This is not a required attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ContextGroupContainerAttribute : System.Attribute
	{
		public ContextGroupContainerAttribute()
		{ }

		public ContextGroupContainerAttribute(bool isDefaultContext)
		{
			IsDefaultContext = isDefaultContext;
		}

		private bool _isDefaultContext = false;
		/// <summary>
		/// Determines if this is also the default context
		/// </summary>
		public bool IsDefaultContext
		{
			get
			{
				return _isDefaultContext;
			}
			set
			{
				_isDefaultContext = value;
			}
		}
	}
}

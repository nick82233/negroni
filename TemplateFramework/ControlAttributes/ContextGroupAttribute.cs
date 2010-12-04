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
using Negroni.TemplateFramework.Parsing;

namespace Negroni.TemplateFramework
{
	/// <summary>
	/// Attribute class to identify what group level within the gadgetXML
	/// a control belongs to.
	/// This is not a required attribute.  If not specified the system assumes
	/// the control is an OSML control valid within a template script
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ContextGroupAttribute : System.Attribute
	{
		/// <summary>
		/// Defines context for this control
		/// </summary>
		/// <param name="context">ParseContext for which this control is valid</param>
		public ContextGroupAttribute(ParseContext context)
		{
			ContextGroup = context;
		}

		/// <summary>
		/// Defines context for this control
		/// </summary>
		/// <param name="context">ParseContext for which this control is valid</param>
		public ContextGroupAttribute(string contextName)
		{
			ContextName = contextName;
		}

		/// <summary>
		/// Defines the context for this control and identifies the Type of the
		/// associated <c>BaseContainerControl</c> which holds everything in this context.
		/// </summary>
		/// <param name="containerControlType"></param>
		public ContextGroupAttribute(Type containerControlType)
		{
			ContainerControlType = containerControlType;
		}


		private ParseContext _contextGroup = null;


		private Type _containerControlType = null;

		/// <summary>
		/// Type of the container which holds everything in this ContextGroup.
		/// If this is new initialized with a Type then the value will always be
		/// typeof(BaseContainerControl)
		/// </summary>
		public Type ContainerControlType
		{
			get
			{
				if (_containerControlType == null)
				{
					_containerControlType = typeof(BaseContainerControl);
				}
				return _containerControlType;
			}
			private set
			{
				//TODO: filter to make sure it derives from BaseContainerControl
				_containerControlType = value;
				ContextGroup = new ParseContext(_containerControlType);
			}
		}



		/// <summary>
		/// String name of ContextGroup
		/// </summary>
		public string ContextName
		{
			get
			{
				if (_contextGroup == null)
				{
					return null;
				}
				else
				{
					return _contextGroup.ContextName;
				}
			}
			set
			{
				if (null == _contextGroup)
				{
					ContextGroup = new ParseContext(value);
				}
			}
		}


		/// <summary>
		/// ParseContext object representing this group.  
		/// This is usually initialized by setting the <c>ContextName</c>
		/// or <c>ContainerControlType</c> value.
		/// </summary>
		public ParseContext ContextGroup
		{
			get
			{
				if (_contextGroup == null)
				{
					_contextGroup = new ParseContext();
				}
				return _contextGroup;
			}
			set
			{
				_contextGroup = value;
			}
		}

	}
}

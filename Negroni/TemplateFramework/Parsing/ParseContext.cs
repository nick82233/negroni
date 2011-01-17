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
using Negroni.TemplateFramework;

namespace Negroni.TemplateFramework.Parsing
{
	/// <summary>
	/// Encapsulation of current parsing context.
	/// This aids the parser in understanding what types of elements to look for.
	/// </summary>
	public class ParseContext
	{

		public ParseContext(){}

		public ParseContext(string contextName){
			ContextName = contextName;
		}

		public ParseContext(Type containerControlType)
		{
			ContainerControlType = containerControlType;
		}


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
				ContextName = value.Name;
			}
		}



		/// <summary>
		/// Unique name of this Context
		/// </summary>
		public string ContextName { get; set; }


		private static ParseContext _defaultContext = null;
		
		/// <summary>
		/// Reserved catch-all context for controls.  
		/// Use this context if there is not control scoping.
		/// In the case of GadgetControls, this represents
		/// the TemplateScript context.
		/// </summary>
		public static ParseContext DefaultContext
		{
			get
			{
				if(_defaultContext == null){
					_defaultContext = new ParseContext(typeof(BaseContainerControl));
				}
				return _defaultContext;
			}
		}

		private static ParseContext _rootContext = null;

		/// <summary>
		/// Reserved root level context.  This is the parsing context in which
		/// RootElementMaster control instances (root XML tags) exist.
		/// </summary>
		public static ParseContext RootContext
		{
			get
			{
				if (_rootContext == null)
				{
					_rootContext = new ParseContext(typeof(object));
				}
				return _rootContext;
			}
		}


		#region object method overrides

		public override bool Equals(object obj)
		{
			if (null == obj) return false;
			if(obj is ParseContext){
				return (ContextName == ((ParseContext)obj).ContextName);
			}
			else if (obj is String)
			{
				return (obj.Equals(ContextName));
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (null == ContextName)
			{
				return 0;
			}
			else
			{
				return ContextName.GetHashCode();
			}
		}

		public override string ToString()
		{
			if (null == ContextName)
			{
				return "ContextName-null";
			}
			else
			{
				return ContextName;
			}
		}

		#endregion

		/*


		Unknown =0,

		/// <summary>
		/// Indicates if parsing context is dealing with direct children of the gadget root
		/// </summary>
		InRootChildren =1,
		/// <summary>
		/// Indicates parsing is dealing with the ModulePrefs section
		/// </summary>
		InModulePrefs = 2,
		/// <summary>
		/// Inside a content block
		/// </summary>
		InContentBlock = 3,
		/// <summary>
		/// Inside a renderable template
		/// </summary>
		InTemplate = 4,
		/// <summary>
		/// Inside a data pipeline template
		/// </summary>
		InDataTemplate = 5,
        /// <summary>
		/// Indicates parsing is dealing with a UserPref section
		/// </summary>
		InUserPref = 6

		 **/ 
	}
}

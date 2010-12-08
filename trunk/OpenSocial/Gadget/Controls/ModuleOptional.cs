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
using System.IO;
using System.Text;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Represents a &lt; UserPref &gt; item in GadgetXML markup
	/// </summary>
	/// <summary>
	/// Gadget module preferences section
	/// </summary>
	[MarkupTag("Optional")]
	[ContextGroup(typeof(ModulePrefs))]
	public class ModuleOptional : ModuleFeature
	{
		public ModuleOptional() : base()
		{}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content">Raw content characters</param>
		public ModuleOptional(string markup)
			: base(markup)
		{}
	}
}

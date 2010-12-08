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
using Negroni.OpenSocial.Gadget.Controls;

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Parameter that may be passed into a control.
	/// Resolves from os:Param tag
	/// </summary>
	[MarkupTag("Param")]
	[ContextGroup(typeof(ModulePrefs))]
	[ContextGroup(typeof(BaseContainerControl))]
	public class ParamControl : BaseGadgetControl
	{
		/// <summary>
		/// Name key of this param
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Value of this param setting - either inner markup or value attribute
		/// </summary>
		public string Value { get; set; }

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);

			Name = GetAttribute("name");
			if (HasAttribute("value"))
			{
				Value = GetAttribute("value");
			}
			else
			{
				Value = InnerMarkup;
			}
		}


		public override void Render(System.IO.TextWriter writer)
		{
			return;
		}
	}
}

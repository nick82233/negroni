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
using Negroni.DataPipeline;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls; 
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.OSML.Controls
{

	/// <summary>
	/// Render contents out of data context
	/// </summary>
	[MarkupTag("os:Html")]
	public class OsHtml : BaseGadgetControl
	{


		public OsHtml() { }
		public OsHtml(string markup)
		{
			LoadTag(markup);
		}

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			if (HasAttribute("code"))
			{
				Code = GetAttribute("code");
			}
		}

		/// <summary>
		/// EL statement to the markup code to render.
		/// Specified in the @code attribute
		/// </summary>
		public string Code { get; set; }


		public override void Render(System.IO.TextWriter writer)
		{
			if (Code == null)
			{
				return;
			}
			writer.Write(MyDataContext.ResolveVariables(Code));
			
		}
	}
}

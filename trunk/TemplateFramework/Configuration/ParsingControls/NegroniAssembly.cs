/* *********************************************************************
   Copyright 2011 Chris Cole

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
using System.Linq;
using System.Text;

namespace Negroni.TemplateFramework.Configuration.ParsingControls
{
	/// <summary>
	/// Represents an assembly element for a Negroni Framework-parsed config
	/// </summary>
	[MarkupTag("assembly")]
	internal class NegroniAssembly : NonRenderedControl, INegroniAssembly
	{
		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);

			this.Name = GetAttribute("name");
			this.Include = GetAttribute("include");
			this.Exclude = GetAttribute("exclude");
		}

		/// <summary>
		/// Name of the assembly
		/// </summary>
		public string Name { get; set; }

		public string Include { get; set; }

		public string Exclude { get; set; }



		/// <summary>
		/// Checks to see if there are include or exclude directives for this assembly
		/// </summary>
		/// <returns></returns>
		public bool HasFilters()
		{
			return !string.IsNullOrEmpty(this.Include) || !string.IsNullOrEmpty(this.Exclude);
		}
	}
}

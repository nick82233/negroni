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

namespace Negroni.OpenSocial.Gadget.Controls
{
	/// <summary>
	/// Template library definition
	/// </summary>
	public class TemplateLibraryDef
	{

		public TemplateLibraryDef() { }

		public TemplateLibraryDef(string uri)
		{
			Uri = uri;
		}


		/// <summary>
		/// Original source URI of the library.
		/// This is used as the key to identify if library is loaded.
		/// </summary>
		public string Uri { get; set; }

		/// <summary>
		/// True if library has been loaded, otherwise false
		/// </summary>
		public bool Loaded { get; set; }

		/// <summary>
		/// Original XML definition for the template library
		/// </summary>
		public string LibraryXml { get; set; }

	}
}

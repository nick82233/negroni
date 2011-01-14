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
	/// Rendering options to apply when rendering an OSML app.
	/// </summary>
	public class RenderOptions
	{
		public RenderOptions()
		{
			DivWrapContentBlocks = false;
			SuppressWhitespace = false;
			ClientRenderCustomTemplates = false;
			DivWrapContentSubViews = true;
		}

		/// <summary>
		/// Suppresses leading and trailing whitespace around content blocks, templates, and custom tag elements.
		/// Still preserves whitespace within literals.
		/// </summary>
		public bool SuppressWhitespace { get; set; }

		/// <summary>
		/// Wraps all content blocks in div elements and adds default identifiers and css classes
		/// </summary>
		public bool DivWrapContentBlocks { get; set; }

		/// <summary>
		/// Wraps all subview content blocks in div elements and adds default identifiers and css classes
		/// </summary>
		public bool DivWrapContentSubViews { get; set; }

		/// <summary>
		/// Flag to instruct the control to render all custom templates
		/// to the client-side representation.  By default this is false
		/// </summary>
		public bool ClientRenderCustomTemplates { get; set; }

		/// <summary>
		/// Flag to instruct the control to render the client-side representation
		/// of the DataContext.  By default this is false.
		/// <remarks>
		/// Set to true in your implementation if you always wish context to emit.
		/// A bare RootElementMaster will not emit client data context automatically, 
		/// regardless of this setting.
		/// </remarks>
		/// </summary>
		public bool ClientRenderDataContext { get; set; }


	}
}

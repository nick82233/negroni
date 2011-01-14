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

namespace Negroni.TemplateFramework.Parsing
{
	/// <summary>
	/// Informational encapsulation of trace position information for a non-container tag
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("NonContainer: {Tag} Depth: {NodeDepth}")]
	struct NonContainerTagInfo
	{
		/// <summary>
		/// Node depth within DOM
		/// </summary>
		public int NodeDepth;
		/// <summary>
		/// Element tag name
		/// </summary>
		public string Tag;
		/// <summary>
		/// Absolute start position within the trace
		/// </summary>
		public int StartPosition;

		/// <summary>
		/// OffsetKey of the given non-container control
		/// </summary>
		public string OffsetKey;
	}
}

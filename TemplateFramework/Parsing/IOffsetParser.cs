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
	public interface IOffsetParser
	{
		/// <summary>
		/// Parse the markup from the default context.
		/// </summary>
		/// <param name="markup"></param>
		/// <returns></returns>
		OffsetItem ParseOffsets(string markup);

		/// <summary>
		/// Parse markup as appropriate for the given context
		/// </summary>
		/// <param name="markup"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		OffsetItem ParseOffsets(string markup, ParseContext context);

		/// <summary>
		/// Add a new custom namespace definition to the parse
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="uri"></param>
		void AddNamespace(string prefix, string uri);
	}
}

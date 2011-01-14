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

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Marks a data item as being serializable to JSON format.
	/// This interface is used to improve the performance of data objects
	/// that will be sent to the client-side data context
	/// </summary>
	public interface IJsonSerializable
	{
		/// <summary>
		/// Writes serialized object contents as JSON format string
		/// </summary>
		/// <returns></returns>
		void WriteAsJSON(System.IO.TextWriter writer);
	}
}

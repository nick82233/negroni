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
using System.Collections;

namespace Negroni.DataPipeline
{
	/// <summary>
	/// Interface to allow a data object to wrap an enumeration.
	/// Enumerations can cometimes interfere with DataContract serialization/deserialization.
	/// This interface allows wrapper objects to expose the desired data as an
	/// enumeration without interfering with the JsonSerializer.
	/// 
	/// Implement this interface to allow Repeaters to work with data
	/// </summary>
	public interface IEnumerableDataWrapper
	{
		/// <summary>
		/// Facilitates resolving an expression value for the current object.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IEnumerable EnumerableData { get; }
	}
}

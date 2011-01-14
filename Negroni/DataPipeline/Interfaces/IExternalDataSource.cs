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
	/// Controls implementing this interface are tagged as referencing
	/// external servers (outside MySpace) for their data
	/// </summary>
	public interface IExternalDataSource
	{
		/// <summary>
		/// Type of response expected from this request
		/// </summary>
		ExpectedResponseType ExpectedResponse { get; }

		/// <summary>
		/// Source URI for this data
		/// </summary>
		string SourceUri { get; }

		/// <summary>
		/// HTTP Method to use when resolving source
		/// </summary>
		string Method { get; }

		/// <summary>
		/// DataContext key to use for retrieving the result.
		/// This key is allowed to be nested.
		/// </summary>
		string ResultKey { get; set; }

		/// <summary>
		/// Location (client or server) where the control would like to
		/// be resolved.  This may be overridden depending on security.
		/// </summary>
		ResolveAt ResolveLocation { get; set; }
	}
}

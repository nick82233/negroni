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
using System.Net;

namespace Negroni.DataPipeline.RequestProcessing
{

	/// <summary>
	/// Wrapper around the request that is put in the processing queue.
	/// </summary>
	class QueuedRequest
	{
		/// <summary>
		/// Identifying key to this request
		/// </summary>
		public string RequestId = null;
		/// <summary>
		/// Actual request
		/// </summary>
		public HttpWebRequest Request = null;
		/// <summary>
		/// Reference to AsyncHandle used for this request processing
		/// </summary>
		public AsyncProcessingResult AsyncResultHandle = null;

		public QueuedRequest(string requestId, HttpWebRequest request, AsyncProcessingResult asyncResultHandle)
		{
			RequestId = requestId;
			Request = request;
			AsyncResultHandle = asyncResultHandle;
		}
	}
}

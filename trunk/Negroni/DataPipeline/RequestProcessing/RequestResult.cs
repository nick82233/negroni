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

namespace Negroni.DataPipeline.RequestProcessing
{
	/// <summary>
	/// Result of a web request.  Usually wrapped in an AsyncProcessingResult object
	/// </summary>
	public class RequestResult
	{

		/// <summary>
		/// Unique key to track the request
		/// </summary>
		public string Key { get; set; }

		int _responseCode = 200;
		public int ResponseCode
		{
			get
			{
				return _responseCode;
			}
			set
			{
				_responseCode = value;
			}
		}


		private byte[] _responseData = null;

		/// <summary>
		/// Binary data returned from the request.
		/// For string data this will be null and response
		/// will be in ResponseString.
		/// </summary>
		public byte[] ResponseData
		{
			get
			{
				return _responseData;
			}
			internal set
			{
				_responseData = value;
			}
		}

		private string _responseString = null;

		/// <summary>
		/// Response as a string value
		/// </summary>
		public string ResponseString
		{
			get
			{
				if (_responseString == null && ResponseData != null)
				{
					if (!string.IsNullOrEmpty(ContentEncoding))
					{
						Encoding enc = System.Text.Encoding.GetEncoding(ContentEncoding);
						if (enc != null)
						{
							_responseString = enc.GetString(ResponseData);
						}
					}
					else
					{
						_responseString = System.Text.UTF8Encoding.UTF8.GetString(ResponseData);
					}
				}
				return _responseString;
			}
			set
			{
				_responseString = value;
			}
		}


		public string ContentType { get; set; }

		public string CharacterSet { get; set; }

		public string ContentEncoding { get; set; }

		/// <summary>
		/// Populated when there was an error in the request
		/// </summary>
		public Exception InternalException { get; set; }
	}
}

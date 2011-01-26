﻿/* *********************************************************************
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
	public class ResponseCompleteEventArgs : EventArgs
	{
		public ResponseCompleteEventArgs() { }

		public ResponseCompleteEventArgs(RequestResult result)
		{
			Result = result;
		}

		public ResponseCompleteEventArgs(string key, RequestResult result)
		{
			Key = key;
			Result = result;
		}

		public RequestResult Result
		{
			get;
			set;
		}

		/// <summary>
		/// Identifying key for this request.
		/// </summary>
		public string Key { get; set; }
	}
}
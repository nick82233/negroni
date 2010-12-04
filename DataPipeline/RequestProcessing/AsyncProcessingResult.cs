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
using System.Threading;

namespace Negroni.DataPipeline.RequestProcessing
{
	public class AsyncProcessingResult : IAsyncResult
	{
		public AsyncProcessingResult() { }

		public AsyncProcessingResult(AsyncCallback callback)
		{
			this.callBack = callback;
		}

		/// <summary>
		/// Unique key to track the request
		/// </summary>
		public string Key { get; set; }


		/// <summary>
		/// Marks everything correctly for completion
		/// </summary>
		internal void CompleteRequest()
		{
			IsCompleted = true;
			lock (this)
			{
				if (_asyncWaitHandle != null)
				{
					_asyncWaitHandle.Set();
				}
			}
			//invoke callback, if registered
			if (callBack != null)
			{
				callBack(this);
			}

		}


		internal AsyncCallback callBack = null;

		#region IAsyncResult Members

		private object _asyncState = null;
		public object AsyncState
		{
			get
			{
				return _asyncState;
			}
			internal set
			{
				_asyncState = value;
			}
		}

		private ManualResetEvent _asyncWaitHandle = null;

		/// <summary>
		/// WaitHandle for async processing.
		/// Initialized on first request
		/// </summary>
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				lock (this)
				{
					if (_asyncWaitHandle == null)
					{
						lock (this)
						{
							if (IsCompleted)
							{
								_asyncWaitHandle = new ManualResetEvent(true);
							}
							else
							{
								_asyncWaitHandle = new ManualResetEvent(false);
							}
						}
					}
					return _asyncWaitHandle;
				}
			}
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public bool IsCompleted
		{
			get; set;
		}

		#endregion
	}
}

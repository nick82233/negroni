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
	/// <summary>
	/// Thread wrapper for an AsyncRequestProcessor Thread in the processor pool
	/// </summary>
	internal class RequestProcessorPoolEntry : IDisposable
	{
		/// <summary>
		/// Possible entry states for a pool entry
		/// </summary>
		public enum EntryState : byte
		{
			/// <summary>
			/// Available for checkout from the pool and use
			/// </summary>
			Free,
			/// <summary>
			/// Working on a processing job
			/// </summary>
			Working,
			/// <summary>
			/// Working on a processing job
			/// </summary>
			WorkComplete,
			/// <summary>
			/// Ready to be cleaned up
			/// </summary>
			ReadyForReaping
		}

		public RequestProcessorPoolEntry() { }

		public Thread MyThread { get; set; }

		private EntryState _myCurrentState = EntryState.Free;

		public EntryState MyCurrentState
		{
			get
			{
				return _myCurrentState;
			}
			set
			{
				_myCurrentState = value;
			}
		}

		/// <summary>
		/// Number of times this pool entry has cycled with no work to do.
		/// This flag is used for reaping the Threads
		/// </summary>
		public int EmptyLoopCount = 0;

		public AsyncRequestProcessor MyProcessorObject = null;


		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (MyProcessorObject != null)
				{
					MyProcessorObject.ExitFlag = true;
					MyProcessorObject = null;
				}
				if (MyThread != null)
				{
					MyThread = null;
				}
			}
		}

		#endregion
	}
}

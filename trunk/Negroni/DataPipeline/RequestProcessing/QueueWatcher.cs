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
using System.Net;
using System.Text;
using System.Threading;

namespace Negroni.DataPipeline.RequestProcessing
{
	/// <summary>
	/// Class to run on seperate thread for queue management.
	/// </summary>
	class QueueWatcher
	{
		const int REAP_COUNT = 10;

		Queue<QueuedRequest> queue;

		public QueueWatcher(Queue<QueuedRequest> requestQueue)
		{
			queue = requestQueue;
		}

		public void PauseWatching()
		{
			paused = true;
			_myWaitHandle.Reset();
		}

		public void ResumeWatching()
		{
			paused = false;
			_myWaitHandle.Set();
		}

		private bool paused = false;

		public bool IsPaused()
		{
			return paused;
		}

		ManualResetEvent _myWaitHandle = new ManualResetEvent(false);

		public void CheckQueue()
		{
			ResumeWatching();
		}

		public void ThreadWatchQueue()
		{

			while (!exitThread)
			{
				if (paused)
				{
					//_myWaitHandle.WaitOne(-1);
					_myWaitHandle.WaitOne(10000);
				}
				try
				{
					if (queue.Count == 0)
					{
						//reap any pool entries without work
						AsyncRequestProcessor.RequestProcessorThreadPool.ReapUnusedPoolEntries();
						this.PauseWatching();
						_myWaitHandle.WaitOne(5000);
						continue;
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.Write("Error in Reaper");
					continue;
				}
				RequestProcessorPoolEntry poolEntry = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
				if (poolEntry == null)
				{
					continue;
				}
				QueuedRequest request = null;
				lock (queue)
				{
					if (queue.Count > 0)
					{
						request = queue.Dequeue();
					}
				}
				if (request != null)
				{
					poolEntry.MyProcessorObject.MyCurrentRequest = request;
					poolEntry.MyProcessorObject.ResumeRequestProcessing();
					if (poolEntry.MyThread.ThreadState == ThreadState.Unstarted)
					{
						try
						{
							poolEntry.MyThread.Start();
						}
						catch { }
					}
				}
				else
				{
					AsyncRequestProcessor.RequestProcessorThreadPool.ReleaseRequestProcessor(poolEntry);
				}
			}
			AsyncRequestProcessor.RequestProcessorThreadPool.ClearPool();
		}

		bool exitThread = false;

		public void StopWatchingQueue()
		{
			exitThread = true;
//			HttpWebRequest x = HttpWebRequest.Create("http://google.com") as HttpWebRequest;
//			IAsyncResult y = x.BeginGetResponse(
//			HttpWebResponse r = x.GetResponse();
			//r.
		}

		
	}



}

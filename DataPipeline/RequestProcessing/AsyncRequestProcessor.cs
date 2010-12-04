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
using System.Threading;
using System.IO;

namespace Negroni.DataPipeline.RequestProcessing
{
	public delegate void ResponseCompleteDelegate(object sender, ResponseCompleteEventArgs e);

	/// <summary>
	/// Handle making an async request for data.
	/// </summary>
	/// <remarks>
	/// This class probably needs it's own private thread pool, queue, and blacklist
	/// for managing non-responsive sites.
	/// It also should be going thru the proxy servers so that they can handle caching
	/// </remarks>
	public class AsyncRequestProcessor
	{
		/// <summary>
		/// Maximum number of threads allowed in the thread pool.
		/// </summary>
		internal const int MAX_POOL_SIZE = 5;

		/// <summary>
		/// Maximum number of items allowed to be backed up in the queue
		/// </summary>
		const int MAX_QUEUE_LENGTH = 100;

		#region Internal Processor Pool
		/// <summary>
		/// Internal processor pool class
		/// </summary>
		internal static class RequestProcessorThreadPool
		{
			const int MAX_NOWORK_LOOPS = 100;

			static public void ClearPool()
			{
				lock (poolLock)
				{
					foreach (RequestProcessorPoolEntry pe in processorPool)
					{
						pe.Dispose();
					}
					processorPool.Clear();
				}
			}

			static public void ReapUnusedPoolEntries()
			{
				for (int i = processorPool.Count-1; i > -1; i--)
				{
					RequestProcessorPoolEntry pe = processorPool[i];
					if (pe.MyCurrentState == RequestProcessorPoolEntry.EntryState.Free
						&& pe.EmptyLoopCount >= MAX_NOWORK_LOOPS)
					{
						lock (poolLock)
						{
							processorPool.RemoveAt(i);
							pe.Dispose();
						}
					}
					//look for invalid state pool entries
					if (pe.MyCurrentState == RequestProcessorPoolEntry.EntryState.Working
						&& pe.MyProcessorObject.MyCurrentRequest == null)
					{
						lock (poolLock)
						{
							if (pe.MyProcessorObject.MyCurrentRequest == null)
							{
								ReleaseRequestProcessor(pe);
							}
						}
					}

				}
			}


			static List<RequestProcessorPoolEntry> processorPool;

			static RequestProcessorThreadPool()
			{
				processorPool = new List<RequestProcessorPoolEntry>();
			}
			static object poolLock = new object();
			/// <summary>
			/// Checks a processor out of the pool
			/// </summary>
			/// <returns></returns>
			static public RequestProcessorPoolEntry GetRequestProcessor()
			{
				RequestProcessorPoolEntry retval = null;
				lock (poolLock)
				{
					for (int i = 0; i < processorPool.Count; i++)
					{
						if (processorPool[i].MyCurrentState == RequestProcessorPoolEntry.EntryState.Free
							|| 
							(
								processorPool[i].MyCurrentState == RequestProcessorPoolEntry.EntryState.Working
								&& processorPool[i].MyProcessorObject.MyCurrentRequest == null
								&& processorPool[i].MyThread.ThreadState == ThreadState.Unstarted)
							)
						{
							retval = processorPool[i];
							break;
						}
						
					}
					if (retval == null && processorPool.Count < MAX_POOL_SIZE)
					{
						retval = new RequestProcessorPoolEntry();
						AsyncRequestProcessor proc = AsyncRequestProcessor.CreateProcessor();
						proc.MyPoolEntry = retval;
						retval.MyProcessorObject = proc;

						retval.MyThread = new Thread(new ThreadStart(proc.ThreadStartRequestProcessing));
						processorPool.Add(retval);
					}
					if (retval != null)
					{
						retval.MyCurrentState = RequestProcessorPoolEntry.EntryState.Working;
					}
				}
				return retval;
			}

			/// <summary>
			/// Releases the processor back to the pool
			/// </summary>
			/// <param name="processor"></param>
			static public void ReleaseRequestProcessor(RequestProcessorPoolEntry processor)
			{
				processor.MyCurrentState = RequestProcessorPoolEntry.EntryState.Free;
			}


		}

		/// <summary>
		/// Pauses the queue watcher.  Used for unit testing.
		/// </summary>
		internal static void PauseProcessing()
		{
			QueueWatcherObject.PauseWatching();
		}
		/// <summary>
		/// Restarts the queue watcher. Used for unit testing
		/// </summary>
		internal static void ResumeProcessing()
		{
			QueueWatcherObject.ResumeWatching();
		}

		internal static void InitializeQueueWatcher()
		{
			if (QueueWatcherObject != null)
			{
				QueueWatcherObject.StopWatchingQueue();
				QueueWatcherThread = null;
				QueueWatcherObject = null;
			}
			QueueWatcherObject = new QueueWatcher(requestQueue);
			QueueWatcherThread = new Thread(new ThreadStart(QueueWatcherObject.ThreadWatchQueue));
			QueueWatcherThread.Start();

		}

		#endregion


		#region Static request processor management methods

		/// <summary>
		/// Enqueues a request for processing as a GET request
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Unique handle to the request</returns>
		static public AsyncProcessingResult EnqueueRequest(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return null;
			}
			return EnqueueRequest(HttpWebRequest.Create(url) as HttpWebRequest);
		}

		/// <summary>
		/// Enqueues a request for processing.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Unique handle to the request</returns>
		static public AsyncProcessingResult EnqueueRequest(HttpWebRequest request)
		{
			string reqId = Guid.NewGuid().ToString();
			AsyncProcessingResult resultHandle = new AsyncProcessingResult();
			resultHandle.Key = reqId;
			lock (requestQueue)
			{
				requestQueue.Enqueue(new QueuedRequest(reqId, request, resultHandle));
			}
			QueueWatcherObject.CheckQueue();

			return resultHandle;
		}

		/// <summary>
		/// Finishes the async request, releases wait handle resources, 
		/// and returns the results.
		/// </summary>
		/// <param name="asyncResult"></param>
		/// <returns></returns>
		static public RequestResult EndRequest(IAsyncResult asyncResult)
		{
			if (!asyncResult.IsCompleted)
			{
				return null;
			}
			else
			{
				return asyncResult.AsyncState as RequestResult;
			}
		}

		/// <summary>
		/// Purges all pending requests from the queue
		/// </summary>
		static public void PurgeQueue()
		{
			lock (requestQueue)
			{
				requestQueue.Clear();
			}
		}

		/// <summary>
		/// Gets the number of items in the queue waiting to be processed.
		/// </summary>
		/// <returns></returns>
		static public int QueueLength()
		{
			return requestQueue.Count;
		}

		static public void ReleaseResources()
		{
			QueueWatcherObject.ResumeWatching();
		}

		/// <summary>
		/// Queue of pending requests to process
		/// </summary>
		private static Queue<QueuedRequest> requestQueue;

		#endregion

		/// <summary>
		/// Static constructor to initialize objects
		/// </summary>
		static AsyncRequestProcessor()
		{
			requestQueue = new Queue<QueuedRequest>();
			InitializeQueueWatcher();

		}


		static QueueWatcher QueueWatcherObject;
		static Thread QueueWatcherThread;



		/// <summary>
		/// Create a new instance of an AsyncRequestProcessor
		/// </summary>
		/// <returns></returns>
		static public AsyncRequestProcessor CreateProcessor()
		{
			return new AsyncRequestProcessor();
		}

	
		private int _timeout = 6;
		/// <summary>
		/// Timeout, in seconds
		/// </summary>
		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}


		/// <summary>
		/// Thread Pool Entry associated with this processor
		/// </summary>
		internal RequestProcessorPoolEntry MyPoolEntry = null;

		/// <summary>
		/// Current request being processed
		/// </summary>
		internal QueuedRequest MyCurrentRequest = null;

		/// <summary>
		/// Flag to exit processing thread
		/// </summary>
		internal bool ExitFlag = false;

		/// <summary>
		/// Entry point for Threaded Processing
		/// </summary>
		public void ThreadStartRequestProcessing()
		{
			while (!ExitFlag || MyCurrentRequest != null)
			{
				if (MyCurrentRequest == null || MyCurrentRequest.Request == null)
				{
					if (MyPoolEntry.EmptyLoopCount < Int32.MaxValue)
					{
						MyPoolEntry.EmptyLoopCount++;
					}
					Waiting = true;
					_myWaitHandle.WaitOne(-1);
					Waiting = false;
					continue;
				}
				HttpWebRequest request = null;
				if (MyCurrentRequest != null && MyCurrentRequest.Request != null)
				{
					request = MyCurrentRequest.Request;
				}
				MyPoolEntry.EmptyLoopCount = 0;
				try
				{
					
					using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
					{
						Stream resultStream = response.GetResponseStream();

						RequestResult result = MyCurrentRequest.AsyncResultHandle.AsyncState as RequestResult;
						if (null == result)
						{
							result = new RequestResult();
							MyCurrentRequest.AsyncResultHandle.AsyncState = result;
						}
						result.ContentType = response.ContentType;
						result.CharacterSet = response.CharacterSet;
						result.ContentEncoding = response.ContentEncoding;
						result.Key = MyCurrentRequest.RequestId;

						string jsMIMEs = "application/json|application/x-javascript|application/javascript|text/ecmascript|application/ecmascript|text/jscript|text/vbscript|";

						if (!string.IsNullOrEmpty(response.ContentType) && 
							response.ContentType.Contains("text")
							|| jsMIMEs.Contains(response.ContentType.ToLower()))
						{
							StreamReader reader = new StreamReader(resultStream);
							result.ResponseString = reader.ReadToEnd();
						}
						else
						{
							BinaryReader br = new BinaryReader(resultStream);
							long contentLength = response.ContentLength;
							int maxBytes = 2048;
							if (contentLength > maxBytes)
							{
								return;
							}
							else
							{
								int arLen = Convert.ToInt32(Math.Min(contentLength, maxBytes));
								byte[] b = new byte[arLen];
								int read = br.Read(b, 0, arLen);
								//todo error check
								result.ResponseData = b;
							}
						}
						response.Close();
						resultStream.Close();
					}
					MyCurrentRequest.AsyncResultHandle.CompleteRequest();
				}
				catch (Exception ex)
				{
						RequestResult result = MyCurrentRequest.AsyncResultHandle.AsyncState as RequestResult;
						if (null == result)
						{
							result = new RequestResult();
							MyCurrentRequest.AsyncResultHandle.AsyncState = result;
						}
					result.ResponseCode = 500;
					result.InternalException = ex;

					System.Diagnostics.Debug.WriteLine("Error processing: " + ex.Message);
					System.Diagnostics.Debug.WriteLine("ReqID: " + MyCurrentRequest.RequestId);
					System.Diagnostics.Debug.WriteLine("Err Request: " + MyCurrentRequest.Request.RequestUri.ToString());
					System.Diagnostics.Debug.WriteLine(ex.StackTrace);
					
					MyCurrentRequest.AsyncResultHandle.CompleteRequest();
					//OnResponseError(this.Result);
				}
				finally
				{
					MyCurrentRequest = null;
					if (MyPoolEntry != null)
					{
						AsyncRequestProcessor.RequestProcessorThreadPool.ReleaseRequestProcessor(MyPoolEntry);
					}
				}
			}
			//cleanup
			MyPoolEntry = null;
		}




		/// <summary>
		/// Flag to indicate if processing is complete.
		/// </summary>
		public bool Complete { get; set; }

		/// <summary>
		/// Event fired when async response completes
		/// </summary>
		public event ResponseCompleteDelegate ResponseComplete;

		protected void OnResponseComplete(RequestResult result)
		{
			if (ResponseComplete != null)
			{
				ResponseComplete(this, new ResponseCompleteEventArgs(result));
			}
		}

		/// <summary>
		/// Event fired when async response completes with error
		/// </summary>
		public event ResponseCompleteDelegate ResponseError;

		protected void OnResponseError(RequestResult result)
		{
			if (ResponseError != null)
			{
				ResponseError(this, new ResponseCompleteEventArgs(result));
			}
		}

		ManualResetEvent _myWaitHandle = null; 

		private AsyncRequestProcessor() {
			_myWaitHandle = new ManualResetEvent(true);
		}

		/// <summary>
		/// Flags the thread to enter a wait state when
		/// the current request completes
		/// </summary>
		public void SetWaiting()
		{
			_myWaitHandle.Reset();
		}

		/// <summary>
		/// Signals the thread to resume processing requests
		/// </summary>
		public void ResumeRequestProcessing()
		{
			Waiting = false;
			_myWaitHandle.Set();
		}

		bool _waiting = false;
		/// <summary>
		/// True if processor is currently waiting
		/// </summary>
		public bool Waiting { get
		{
			return _waiting;
		}
			private set
			{
				_waiting = value;
			}
		}


		/// <summary>
		/// Target site to be contacted
		/// </summary>
		public Uri TargetUri { get; set; }

		private string _method = "get";

		/// <summary>
		/// Identifying key used to track this request and its response.
		/// </summary>
		public string Key { get; set; }


		public string ProxyUrl { get; set; }

		/// <summary>
		/// Request Method
		/// </summary>
		public string Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
			}
		}
	}


}

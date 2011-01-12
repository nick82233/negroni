using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Negroni.DataPipeline.Tests.TestData;
using Negroni.DataPipeline.RequestProcessing;
using System.Threading;
using System.Net;

#if XUNIT
using Xunit;
#elif NUNIT
using NUnit;
using NUnit.Framework;
using NUnitExtension.RowTest;
#else
using MbUnit.Framework;
#endif


namespace Negroni.DataPipeline.Tests
{
	[Ignore("Async often fails test for timing reasons.  Uncomment this attribute to run")]
	[TestFixture]
	[TestsOn(typeof(AsyncRequestProcessor))]
	public class AsyncRequestProcessorTestFixture
	{

		string response = null;


#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void AddToQueue()
		{
			HttpWebRequest request = HttpWebRequest.Create("http://www.google.com") as HttpWebRequest;

			AsyncRequestProcessor.PurgeQueue(); 
			AsyncRequestProcessor.PauseProcessing();
			Thread.Sleep(10);
			int qCnt = AsyncRequestProcessor.QueueLength();
			try
			{
				Assert.AreEqual(0, qCnt, "Queue not initially empty");
				IAsyncResult resultHook = AsyncRequestProcessor.EnqueueRequest(request);
				Assert.Greater(AsyncRequestProcessor.QueueLength(), qCnt);
				AsyncRequestProcessor.ResumeProcessing();
				Thread.Sleep(10);
				WaitHandle evt = resultHook.AsyncWaitHandle;
				evt.WaitOne(10000);
				Assert.IsTrue(resultHook.IsCompleted, "Not flagged as complete after wait event");
				Assert.AreEqual(0, AsyncRequestProcessor.QueueLength(), "Queue not emptied");
			}
			finally
			{
				AsyncRequestProcessor.ResumeProcessing();
				AsyncRequestProcessor.ReleaseResources();
			}

		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void QueueRequest()
		{
			AsyncRequestProcessor.ResumeProcessing();
			HttpWebRequest request = HttpWebRequest.Create("http://www.google.com") as HttpWebRequest;

			IAsyncResult resultHandle = AsyncRequestProcessor.EnqueueRequest(request);
			WaitHandle waitHandle = resultHandle.AsyncWaitHandle;
			waitHandle.WaitOne(5000);
			Assert.IsTrue(resultHandle.IsCompleted, "Request is not complete after WaitEvent releases");

			RequestResult result = AsyncRequestProcessor.EndRequest(resultHandle);
			Assert.IsNotNull(result);
			Assert.IsFalse(string.IsNullOrEmpty(result.ResponseString), "Response is empty");

		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void QueueRequestWithStrings()
		{
			AsyncRequestProcessor.ResumeProcessing();

			IAsyncResult resultHandle = AsyncRequestProcessor.EnqueueRequest("http://www.cnet.com");
			WaitHandle waitHandle = resultHandle.AsyncWaitHandle;
			waitHandle.WaitOne(5000);
			Assert.IsTrue(resultHandle.IsCompleted, "Request is not complete after WaitEvent releases");

			RequestResult result = AsyncRequestProcessor.EndRequest(resultHandle);
			Assert.IsNotNull(result);
			Assert.IsFalse(string.IsNullOrEmpty(result.ResponseString), "Response is empty");
			Assert.IsTrue(result.ResponseString.Contains("cnet"), "Response doesn't have cnet");

		}
#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void FullRequestQueueClears()
		{
			AsyncRequestProcessor.PauseProcessing();

			List<IAsyncResult> handles = new List<IAsyncResult>();

			for (int i = 0; i < AsyncRequestProcessor.MAX_POOL_SIZE; i++)
			{
				handles.Add(AsyncRequestProcessor.EnqueueRequest(HttpWebRequest.Create("http://www.google.com") as HttpWebRequest));
			}

			HttpWebRequest requestCnn = HttpWebRequest.Create("http://www.cnn.com") as HttpWebRequest;

			IAsyncResult hook = AsyncRequestProcessor.EnqueueRequest(requestCnn);
			WaitHandle waitEvent = hook.AsyncWaitHandle;
			AsyncRequestProcessor.ResumeProcessing();
			bool wr = waitEvent.WaitOne(5000);
			//Assert.IsTrue(wr, "WaitOne timed out");
			Assert.IsTrue(hook.IsCompleted, "Request is not complete after WaitEvent releases");
			RequestResult result = AsyncRequestProcessor.EndRequest(hook);
			Assert.IsNotNull(result);
			Assert.IsFalse(string.IsNullOrEmpty(result.ResponseString), "Response is empty");
			Assert.IsTrue(result.ResponseString.Contains("cnn"), "CNN doesn't appear to recognize its self");
			List<WaitHandle> whs = new List<WaitHandle>();
			foreach (IAsyncResult item in handles)
			{
				//if (!item.IsCompleted)
				//{
				whs.Add(item.AsyncWaitHandle);
				//				}
			}
			if (whs.Count > 0)
			{
				wr = WaitHandle.WaitAll(whs.ToArray(), 5000);
				//Assert.IsTrue(wr, "WaitAll timed out");
			}
			foreach (IAsyncResult item in handles)
			{
				Assert.IsTrue(item.IsCompleted, "Item did not complete");

				RequestResult x = AsyncRequestProcessor.EndRequest(item);
				Assert.AreEqual(200, x.ResponseCode, "Didn't get a 200 back");
				if (x == null || x.ResponseString == null)
				{
					Assert.Fail("Empty result");
				}
				Assert.Greater(x.ResponseString.Length, 10, "Response appears empty");
			}
			Assert.IsTrue(AsyncRequestProcessor.QueueLength() == 0, "Queue is not empty");
			AsyncRequestProcessor.ReleaseResources();
		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void LotsOfRequestsClearQuickly()
		{
			AsyncRequestProcessor.PurgeQueue();
			string[] urls = new string[]{
				"http://www.cnn.com",
				"http://www.cnet.com",
				"http://www.expedia.com",
				"http://www.yahoo.com",
				"http://m.yahoo.com",
				"http://www.cbs.com",
				"http://google.com",
				"http://www.myspace.com",
				"http://www.facebook.com"

			};
			long startTicks = DateTime.Now.Ticks;
			List<IAsyncResult> handles = new List<IAsyncResult>();

			for (int i = 0; i < urls.Length; i++)
			{
				handles.Add(AsyncRequestProcessor.EnqueueRequest(HttpWebRequest.Create(urls[i]) as HttpWebRequest));
			}
			IAsyncResult rslt = handles[handles.Count - 1];
			WaitHandle hook = rslt.AsyncWaitHandle;
			hook.WaitOne(5000);
			Assert.IsTrue(rslt.IsCompleted, "Request is not complete after WaitEvent releases");
			RequestResult result = AsyncRequestProcessor.EndRequest(rslt);
			Assert.IsNotNull(result);
			Assert.IsFalse(string.IsNullOrEmpty(result.ResponseString), "Response is empty");

			bool ok = true;
			for (int i = 0; i < handles.Count - 1; i++)
			{
				IAsyncResult result2 = handles[i];
				if(!result2.IsCompleted){
					result2.AsyncWaitHandle.WaitOne(500);
				}
				if (!result2.IsCompleted)
				{
					ok = false;
				}
				else
				{
					result = AsyncRequestProcessor.EndRequest(result2);
					Assert.IsNotNull(result.ResponseString);
				}
			}

			Assert.IsTrue(ok, "All processing not complete");
			TimeSpan t = new TimeSpan(DateTime.Now.Ticks - startTicks);
			Assert.Less(t.Seconds, 5, "Requests took more than five seconds");
			AsyncRequestProcessor.ReleaseResources();
		}


	}
}

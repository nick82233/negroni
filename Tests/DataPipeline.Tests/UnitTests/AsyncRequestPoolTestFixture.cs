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
#if XUNIT	
#else
	[TestFixture]
#endif
#if MBUNIT
	[TestsOn(typeof(AsyncRequestProcessor.RequestProcessorThreadPool))]
#endif
	public class AsyncRequestPoolTestFixture
	{

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void PoolCheckoutGetsProcessor()
		{
			AsyncRequestProcessor.RequestProcessorThreadPool.ClearPool();
			RequestProcessorPoolEntry proc = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
			Assert.IsNotNull(proc);
			Assert.AreEqual(RequestProcessorPoolEntry.EntryState.Working, proc.MyCurrentState);
		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void TwoCheckoutGetProcessors()
		{
			AsyncRequestProcessor.RequestProcessorThreadPool.ClearPool();
			RequestProcessorPoolEntry proc = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
			RequestProcessorPoolEntry proc2 = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();

			RequestProcessorPoolEntry[] items = {proc, proc2};

			for (int i = 0; i < items.Length; i++)
			{
				Assert.IsNotNull(items[i], String.Format("Item {0} is null", i) );
				Assert.AreEqual(RequestProcessorPoolEntry.EntryState.Working, proc.MyCurrentState);
			}
		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void CheckoutAllBlocksMore()
		{
			AsyncRequestProcessor.RequestProcessorThreadPool.ClearPool();

			try
			{
				RequestProcessorPoolEntry proc = null;
				for (int i = 0; i < AsyncRequestProcessor.MAX_POOL_SIZE; i++)
				{
					proc = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
					proc.MyProcessorObject.MyCurrentRequest = new QueuedRequest("a", WebRequest.Create("http://www.google.com") as HttpWebRequest, null);
					Assert.IsNotNull(proc, String.Format("Item {0} is null", i));
				}
				//try additional checkout
				proc = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
				Assert.IsNull(proc, "Item incorrectly allowed when pull tapped out");
			}
			finally
			{
				AsyncRequestProcessor.RequestProcessorThreadPool.ClearPool();
			}
		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void ReleaseAllowsReCheckout()
		{
			AsyncRequestProcessor.RequestProcessorThreadPool.ClearPool();

			try
			{
				List<RequestProcessorPoolEntry> items = new List<RequestProcessorPoolEntry>();

				RequestProcessorPoolEntry proc = null;
				for (int i = 0; i < AsyncRequestProcessor.MAX_POOL_SIZE; i++)
				{
					proc = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
					proc.MyProcessorObject.MyCurrentRequest = new QueuedRequest("a", WebRequest.Create("http://www.google.com") as HttpWebRequest, null);
					Assert.IsNotNull(proc, String.Format("Item {0} is null", i));
					items.Add(proc);
				}
				//try additional checkout
				proc = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
				Assert.IsNull(proc, "Item incorrectly allowed when pull tapped out");
				//release one
				proc = items[0];
				items.RemoveAt(0);
				AsyncRequestProcessor.RequestProcessorThreadPool.ReleaseRequestProcessor(proc);
				proc = AsyncRequestProcessor.RequestProcessorThreadPool.GetRequestProcessor();
				Assert.IsNotNull(proc, "Release failed");
			}
			finally
			{
				AsyncRequestProcessor.RequestProcessorThreadPool.ClearPool();
			}
		}

	}
}

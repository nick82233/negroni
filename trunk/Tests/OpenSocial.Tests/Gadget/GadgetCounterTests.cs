using System;
using System.IO;

using MbUnit.Framework;
using Negroni.TemplateFramework;

using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.Controls;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.DataPipeline;


namespace Negroni.OpenSocial.Tests.Gadget
{
    /// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="GadgetMaster"/> class.
	/// Exercises the counter functionality
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(GadgetMaster))]
	public class GadgetCounterTests : OsmlControlTestBase
    {
		[Test]
		public void RenderCountInitiallyZero()
		{
			GadgetMaster.Counters.RenderCount.ResetCounters();
			Assert.AreEqual(0, GadgetMaster.Counters.RenderCount.GetGlobalCount());

		}


		[Test]
		public void RenderOnceCount()
		{
			GadgetMaster.Counters.RenderCount.ResetCounters();
			GadgetMaster.Counters.RenderCount.IsGlobalCounterEnabled = true;

			GadgetMaster target = this.CreateInitialTestGadget(99);

			Assert.AreEqual(0, GadgetMaster.Counters.RenderCount.GetGlobalCount());

			string result = target.RenderToString();

			Assert.AreEqual(1, GadgetMaster.Counters.RenderCount.GetGlobalCount());

		}


		[Test]
		public void ParseCountZero()
		{
			GadgetMaster.Counters.ParseCount.ResetCounters();
			GadgetMaster.Counters.ParseCount.IsGlobalCounterEnabled = true;

			Assert.AreEqual(0, GadgetMaster.Counters.ParseCount.GetGlobalCount());
		}


		[Test]
		public void ParseCountIncrementOnce()
		{
			GadgetMaster.Counters.ParseCount.ResetCounters();
			GadgetMaster.Counters.ParseCount.IsGlobalCounterEnabled = true;

			Assert.AreEqual(0, GadgetMaster.Counters.ParseCount.GetGlobalCount());

			GadgetMaster target = this.CreateInitialTestGadget(99);

			Assert.AreEqual(1, GadgetMaster.Counters.ParseCount.GetGlobalCount(), "Count did not increment");
		}

		[Test]
		public void ParseCountParseTwice()
		{
			GadgetMaster.Counters.ParseCount.ResetCounters();
			GadgetMaster.Counters.ParseCount.IsGlobalCounterEnabled = true;

			Assert.AreEqual(0, GadgetMaster.Counters.ParseCount.GetGlobalCount());

			GadgetMaster target = this.CreateInitialTestGadget(99);
			Assert.AreEqual(1, GadgetMaster.Counters.ParseCount.GetGlobalCount());

			//re-parsing should not trigger if already parsed
			target.Parse();
			Assert.AreEqual(1, GadgetMaster.Counters.ParseCount.GetGlobalCount());
		}

		[Test]
		public void ParseCountReParse()
		{
			GadgetMaster.Counters.ParseCount.ResetCounters();
			GadgetMaster.Counters.ParseCount.IsGlobalCounterEnabled = true;

			Assert.AreEqual(0, GadgetMaster.Counters.ParseCount.GetGlobalCount());

			GadgetMaster target = this.CreateInitialTestGadget(99);
			Assert.AreEqual(1, GadgetMaster.Counters.ParseCount.GetGlobalCount());

			//re-parsing should not trigger if already parsed
			target.ReParse();
			Assert.AreEqual(2, GadgetMaster.Counters.ParseCount.GetGlobalCount());
			target.ReParse();
			Assert.AreEqual(3, GadgetMaster.Counters.ParseCount.GetGlobalCount());
		}


		#region Support methods

		/// <summary>
		/// Initializes and returns a test GadgetMaster
		/// </summary>
		/// <param name="appId"></param>
		/// <returns></returns>
		GadgetMaster CreateInitialTestGadget(int appId)
		{
			GadgetMaster target = new GadgetMaster(testFactory, GadgetTestData.FullGadget, new OffsetItem(GadgetTestData.GadgetOffsetListString));
			if (!target.IsParsed)
			{
				target.Parse();
			}

			Assert.IsTrue(target.RawTag.Length > 0);
			Assert.IsTrue(target.Controls.Count > 0, "No controls created");

			target.AppID = appId;

			return target;
		}

		#endregion

	}
}
	
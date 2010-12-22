using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.OSML;
using Negroni.DataPipeline;
using Negroni.TemplateFramework.Tooling;

namespace Negroni.OpenSocial.Test.Controls
{
	/// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="ControlFactoryCounters"/> class.
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(ControlFactoryCounters))]
	public class ControlFactoryCountersTests : OsmlControlTestBase
	{
		[Test]
		public void TestGlobalCountersDisabled()
		{
			ControlFactoryCounters target = new ControlFactoryCounters();
			Assert.IsFalse(target.IsGlobalCounterEnabled, "Global counter not disabled by default");
			Assert.AreEqual(0, target.GetGlobalCount(), "Count not initially zero");

			target.IncrementGlobalCount();
			Assert.AreEqual(0, target.GetGlobalCount(), "Global count incorrectly incremented when disabled");
		}

		[Test]
		public void TestGlobalCountersEnabled()
		{
			ControlFactoryCounters target = new ControlFactoryCounters();
			Assert.IsFalse(target.IsGlobalCounterEnabled, "Global counter not disabled by default");
			target.IsGlobalCounterEnabled = true;
			Assert.IsTrue(target.IsGlobalCounterEnabled, "Global counter did not enable");
			Assert.AreEqual(0, target.GetGlobalCount(), "Count not initially zero");

			target.IncrementGlobalCount();
			Assert.AreEqual(1, target.GetGlobalCount());
			target.IncrementGlobalCount();
			Assert.AreEqual(2, target.GetGlobalCount());
		}

		[Test]
		public void InFactoryGlobalCountersEnabled()
		{
			InitGadgetControlFactory();
			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			factory.Counters.IsGlobalCounterEnabled = true;

			Assert.AreEqual(0, factory.Counters.GetGlobalCount(), "Count not initially zero");


			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, GadgetTestData.FullGadget, new OffsetItem(GadgetTestData.GadgetOffsetListString));
			ResolveDataControlValues(master.MyDataContext, GadgetTestData.Viewer, GadgetTestData.Viewer, null);

			string result = master.RenderToString("profile");

			Assert.IsFalse(String.IsNullOrEmpty(result), "Empty rendered results");
			Assert.Greater(master.Controls.Count, 0, "Empty control tree");

			Assert.Greater(factory.Counters.GetGlobalCount(), 0, "Counter is zero after render");
			Assert.AreEqual(master.CountInternalControls(), factory.Counters.GetGlobalCount(), "Counts do not match");
		}


		[Test]
		public void InFactoryTimesliceCounters()
		{
			InitGadgetControlFactory();
			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			factory.Counters.IsGlobalCounterEnabled = true;

			Assert.AreEqual(0, factory.Counters.GetGlobalCount(), "Count not initially zero");

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, GadgetTestData.FullGadget, new OffsetItem(GadgetTestData.GadgetOffsetListString));
			ResolveDataControlValues(master.MyDataContext, GadgetTestData.Viewer, GadgetTestData.Viewer, null);
			string result = master.RenderToString("profile");

			Assert.Greater(master.Controls.Count, 0, "Empty control tree");

			CounterSlice slice = factory.Counters.GetCurrentCountTimeslice();

			Assert.Greater(slice.Count, 0, "Slice empty");
		}

		[Test]
		public void ResliceCounters()
		{
			InitGadgetControlFactory();
			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			factory.Counters.IsGlobalCounterEnabled = true;

			Assert.AreEqual(0, factory.Counters.GetGlobalCount(), "Count not initially zero");

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, GadgetTestData.FullGadget, new OffsetItem(GadgetTestData.GadgetOffsetListString));
			ResolveDataControlValues(master.MyDataContext, GadgetTestData.Viewer, GadgetTestData.Viewer, null);
			string result = master.RenderToString("profile");

			Assert.Greater(master.Controls.Count, 0, "Empty control tree");

			CounterSlice slice = factory.Counters.GetCurrentCountTimeslice();

			Assert.Greater(slice.Count, 0, "Slice empty");

			factory.Counters.ResetTimesliceCounter();
			slice = factory.Counters.GetCurrentCountTimeslice();
			Assert.AreEqual(0, slice.Count, "Resetting time did not clear slice");
		}


		[Test]
		public void PerControlCountSimple()
		{
			InitGadgetControlFactory();
			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			factory.Counters.IsControlUseCountingEnabled = true;

			Assert.AreEqual(0, factory.Counters.GetControlUsageCount(typeof(OsmlName)), "Name not initially zero");
			Assert.AreEqual(0, factory.Counters.GetControlUsageCount(typeof(OsmlPeopleSelector)), "OsmlPeopleSelector not initially zero");

			string nameTag = "<os:Name person='${foo}' />";
			factory.CreateControl(nameTag, ParseContext.DefaultContext, new GadgetMaster() );
			Assert.AreEqual(1, factory.Counters.GetControlUsageCount(typeof(OsmlName)), "Name not incremented");
			Assert.AreEqual(0, factory.Counters.GetControlUsageCount(typeof(OsmlPeopleSelector)), "OsmlPeopleSelector incorrectly initially");
		}


		[Test]
		public void PerControlCountDisabled()
		{
			InitGadgetControlFactory();
			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			factory.Counters.IsControlUseCountingEnabled = false;

			Assert.AreEqual(0, factory.Counters.GetControlUsageCount(typeof(OsmlName)), "Name not initially zero");

			string nameTag = "<os:Name person='${foo}' />";
			factory.CreateControl(nameTag, ParseContext.DefaultContext, new GadgetMaster());
			Assert.AreEqual(0, factory.Counters.GetControlUsageCount(typeof(OsmlName)), "Counter returned value when disabled");
			factory.Counters.IsControlUseCountingEnabled = true;
			Assert.AreEqual(0, factory.Counters.GetControlUsageCount(typeof(OsmlName)), "Counter incremented incorrectly when disabled");
		}

		[Test]
		public void ResetPerControlCount()
		{
			InitGadgetControlFactory();
			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			factory.Counters.IsControlUseCountingEnabled = true;

			Assert.AreEqual(0, factory.Counters.GetControlUsageCount(typeof(OsmlName)), "Name not initially zero");

			string nameTag = "<os:Name person='${foo}' />";
			factory.CreateControl(nameTag, ParseContext.DefaultContext, new GadgetMaster());
			Assert.AreEqual(1, factory.Counters.GetControlUsageCount(typeof(OsmlName)));
			factory.CreateControl(nameTag, ParseContext.DefaultContext, new GadgetMaster());
			Assert.AreEqual(2, factory.Counters.GetControlUsageCount(typeof(OsmlName)));

			for (int i = 0; i < 5; i++)
			{
				factory.CreateControl(nameTag, ParseContext.DefaultContext, new GadgetMaster());
			}
			Assert.AreEqual(7, factory.Counters.GetControlUsageCount(typeof(OsmlName)));

		}


	}
}

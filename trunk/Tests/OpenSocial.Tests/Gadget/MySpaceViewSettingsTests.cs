using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.TestData.Partials;
using Negroni.OpenSocial.Test.OSML;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Test.Gadget
{
	[TestFixture]
	[TestsOn(typeof(MySpaceViewSettings))]
	public class MySpaceViewSettingsTests : OsmlControlTestBase
	{

		[Test]
		public void ViewSettingsDefinedInGadget()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceViewSettings.Source);

			Assert.IsNotNull(target, "Target gadget failed to parse");
			Assert.IsNotNull(target.ModulePrefs, "ModulePrefs is null");
			Assert.IsNotNull(target.ModulePrefs.MySpaceViewSettings, "Module prefs view settings is null");
			Assert.IsNotNull(target.MySpaceViewSettings, "MySpace view settings is null");
		}

		[Test]
		public void CanvasSizeCorrect()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceViewSettings.Source);

			Assert.AreEqual(ModPrefMySpaceViewSettings.expectedCanvasSize.Width, target.MySpaceViewSettings.CanvasSize.Width, "Canvas width wrong");
			Assert.AreEqual(ModPrefMySpaceViewSettings.expectedCanvasSize.Height, target.MySpaceViewSettings.CanvasSize.Height, "Canvas height wrong");
		}
		[Test]
		public void ProfileSizeCorrect()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceViewSettings.Source);

			Assert.AreEqual(ModPrefMySpaceViewSettings.expectedProfileSize.Width, target.MySpaceViewSettings.ProfileSize.Width, "Profile width wrong");
			Assert.AreEqual(ModPrefMySpaceViewSettings.expectedProfileSize.Height, target.MySpaceViewSettings.ProfileSize.Height, "Profile height wrong");
			Assert.AreEqual(ModPrefMySpaceViewSettings.expectedProfileHeight, target.MySpaceViewSettings.ProfileSize.Height);
		}
		[Test]
		public void HomeSizeNullWhenNotSet()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceViewSettings.Source);

			Assert.IsNull(target.MySpaceViewSettings.HomeSize, "Home size not null when not set");
		}
		[Test]
		public void ProfileLocationAndMountCorrect()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceViewSettings.Source);

			Assert.AreEqual(ModPrefMySpaceViewSettings.expectedProfileLocation, target.MySpaceViewSettings.ProfileLocation, "Profile location wrong");
			Assert.AreEqual(ModPrefMySpaceViewSettings.expectedProfileMount, target.MySpaceViewSettings.GetProfileMount(), "Profile mount wrong");
		}
	}
}

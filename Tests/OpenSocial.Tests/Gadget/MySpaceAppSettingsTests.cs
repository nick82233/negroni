using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.TestData.Partials;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Tests.Gadget
{
	[TestFixture]
	[TestsOn(typeof(MySpaceAppSettings))]
	public class MySpaceAppSettingsTests : OsmlControlTestBase
	{

		[Test]
		public void AppSettingsDefinedInGadget()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceAppSettings.Source);

			Assert.IsNotNull(target, "Target gadget failed to parse");
			Assert.IsNotNull(target.ModulePrefs, "ModulePrefs is null");
			Assert.IsNotNull(target.ModulePrefs.MySpaceAppSettings, "Module prefs app settings is null");
			Assert.IsNotNull(target.MySpaceAppSettings, "MySpace app settings is null");
		}

		[Test]
		public void AgeRestrictionCorrect()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceAppSettings.Source);

			Assert.AreEqual(ModPrefMySpaceAppSettings.ExpectedAgeRestriction, target.MySpaceAppSettings.AppAgeRestriction, "Age restriction wrong");
		}


		[Test]
		public void AgeRestrictionFromDirectLoad()
		{
			GadgetMaster gadget = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceAppSettings.Source);
			ModuleOptional myOpt = gadget.ModulePrefs.OptionalFeatures[0];

			MySpaceAppSettings target = new MySpaceAppSettings(myOpt);

			Assert.AreEqual(ModPrefMySpaceAppSettings.ExpectedAgeRestriction, target.AppAgeRestriction, "Age restriction wrong");
		}

		//[Test]
		//public void PrimaryCategoryCorrect()
		//{
		//    GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceAppSettings.Source);

		//    Assert.AreEqual(ModPrefMySpaceAppSettings.ExpectedCategory1, target.MySpaceAppSettings.PrimaryCategory.DisplayName, "Primary category wrong");
		//}
		//[Test]
		//public void SecondaryCategoryCorrect()
		//{
		//    GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, ModPrefMySpaceAppSettings.Source);

		//    Assert.AreEqual(ModPrefMySpaceAppSettings.ExpectedCategory2, target.MySpaceAppSettings.SecondaryCategory.DisplayName, "Secondary category wrong");
		//}


	}
}

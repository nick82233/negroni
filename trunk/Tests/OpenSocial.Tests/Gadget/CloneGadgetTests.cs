using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.Controls;
using Negroni.OpenSocial.Test.OSML;

using Negroni.DataPipeline;
using Negroni.DataPipeline.Security;

namespace Negroni.OpenSocial.Test.Gadget
{
    [TestFixture]
    [TestsOn(typeof(GadgetMaster))]
    public class CloneGadgetTests : OsmlControlTestBase
	{

		ExternalMessageBundlesGadgetTestData testData;

		public CloneGadgetTests()
		{
			testData = new ExternalMessageBundlesGadgetTestData();
		}

		[Test]
		public void ClonedGadgetHasExternalMessages()
		{

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source);

			//This takes a long time.  Manual add of info instead.
			//GadgetProvider.FetchExternalMessageBundles(master);

			MessageBundleData.LoadSampleMessageBundles(master);


			string[] cultures = master.MyDataContext.ResourceStringCatalog.GetDefinedCultures();
			string cult = "";
			for (int i = 0; i < cultures.Length; i++)
			{
				cult += "|" + cultures[i];
			}
			Assert.IsTrue(cult.Contains("ru"));
			Assert.IsTrue(cult.Contains("fr"));


			Assert.IsTrue(master.HasExternalMessageBundles(), "External bundles not defined in main gadget");
			Assert.Greater(master.MasterDataContext.ResourceStringCatalog.GetDefinedCultures().Length, 0, "No locales defined in main gadget");

			GadgetMaster target = master.Clone() as GadgetMaster;

			Assert.IsNotNull(target);
			Assert.IsTrue(target.HasExternalMessageBundles(), "Clone has no bundles");
			Assert.Greater(target.MasterDataContext.ResourceStringCatalog.GetDefinedCultures().Length, 0, "No locales defined in clone gadget");

			string allMessages = master.GetConsolidatedMessageBundles();
		}

	}
}

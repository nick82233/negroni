using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Tests.Helpers;
using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.TestData.Partials;
using Negroni.OpenSocial.Tests.TestData.TemplateLibraries;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Tests.Gadget
{
	[TestFixture]
	[TestsOn(typeof(ExternalTemplates))]
	public class GadgetExternalTemplatesTestFixture : OsmlControlTestBase
	{

		public BasicLibrary templateData = new BasicLibrary();

		[Test]
		public void ExternalTemplateReferencesFound()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.IsTrue(master.TemplateLibraries.HasLibraries());
		}

		[Test]
		public void ExternalTemplatesEmptyWhenNotDefined()
		{
			TestableMarkupDef testData = new DataGadgetEmptyAttrRepeatTestData();

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.IsFalse(master.TemplateLibraries.HasLibraries());
		}

		[Test]
		public void ExternalTemplateURISet()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.AreEqual(testData.ExpectedTemplateLibraryUri, master.TemplateLibraries.Libraries[0].Uri, "Library URI incorrect");
		}

		[Test]
		public void TemplateLoadStateInitiallyEmpty()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.IsTrue(master.TemplateLibraries.HasLibraries(), "No libraries defined");
			Assert.IsFalse(master.TemplateLibraries.Libraries[0].Loaded, "Library incorrectly marked as loaded");

		}

		public void TemplateLoadStateSetOn()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.IsTrue(master.TemplateLibraries.HasLibraries(), "No libraries defined");
			Assert.IsFalse(master.TemplateLibraries.Libraries[0].Loaded, "Library incorrectly marked as loaded");

			BasicLibrary testLib = new BasicLibrary();
			master.LoadTemplateLibrary(testData.ExpectedTemplateLibraryUri, BasicLibrary.Source);

			Assert.IsFalse(master.TemplateLibraries.HasLibraries(), "No libraries defined");
			Assert.IsTrue(master.TemplateLibraries.Libraries[0].Loaded, "Library incorrectly marked as loaded");

			Assert.IsFalse(string.IsNullOrEmpty(master.TemplateLibraries.Libraries[0].LibraryXml), "LibraryXml value not set on load");

		}


		[Test]
		public void ExternalTemplateCustomTagsRegistered()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();

			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.Greater(testData.ExpectedCustomTags.Count, 0, "No expected tags");

			BasicLibrary testLib = new BasicLibrary();

			master.LoadTemplateLibrary(testData.ExpectedTemplateLibraryUri, BasicLibrary.Source);

			foreach (string tag in testData.ExpectedCustomTags)
			{
				Assert.IsTrue(master.MasterCustomTagFactory.IsCustomTag(tag), "Tag: " + tag + " is not registered with tag factory");
				
			}

		}


	}
}

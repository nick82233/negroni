using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.OSML.Templates;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Tests.Helpers;
using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.TestData.Partials;
using Negroni.OpenSocial.Tests.TestData.TemplateLibraries;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Tests.Controls;

namespace Negroni.OpenSocial.Tests.Gadget
{
	[TestFixture]
	[TestsOn(typeof(Template))]
	public class TemplateLibrariesTestFixture : OsmlControlTestBase
	{

		[Test]
		public void TemplateLibraryHasTag()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();
			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.Greater(testData.ExpectedCustomTags.Count, 0, "No expected tags");

			BasicLibrary testLib = new BasicLibrary();
			TemplatesRoot library = master.LoadTemplateLibrary(testData.ExpectedTemplateLibraryUri, BasicLibrary.Source);

			Assert.AreEqual(2, library.CustomTags.Count, "Incorrect template count");
			Assert.IsFalse(String.IsNullOrEmpty(library.CustomTags[0].Tag), "tag is empty");


		}

		[Test]
		public void RenderWithExternalTemplates()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();
			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			BasicLibrary testLib = new BasicLibrary();
			TemplatesRoot library = master.LoadTemplateLibrary(testData.ExpectedTemplateLibraryUri, BasicLibrary.Source);

			master.RenderingOptions.ClientRenderCustomTemplates = false;
			master.RenderingOptions.ClientRenderDataContext = false;
			master.RenderingOptions.DivWrapContentBlocks = false;
			master.RenderingOptions.SuppressWhitespace = true;

			master.ReParse();
			ResolveDataControlValues(master.MyDataContext, testData.ExpectedViewer, testData.ExpectedViewer, testData.ExpectedFriends);

			string result = ControlTestHelper.NormalizeRenderResult(master.RenderToString("canvas"));
			result = result.Replace("  ", " ");
			string expected = ControlTestHelper.NormalizeRenderResult(testData.ExpectedCanvas);
			Assert.AreEqual(expected, result, "Rendered results don't match expected");

		}

		[Test]
		public void CountCustomTemplates()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();
			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.AreEqual(0, master.MasterCustomTagFactory.CustomTags.Count, "Custom tags not initially zero");
			Assert.IsFalse(master.NeedsReparse);
			BasicLibrary testLib = new BasicLibrary();
			TemplatesRoot library = master.LoadTemplateLibrary(testData.ExpectedTemplateLibraryUri, BasicLibrary.Source);

			int tcount = master.MasterCustomTagFactory.CustomTags.Count;

			Assert.Greater(master.MasterCustomTagFactory.CustomTags.Count, 0, "No Custom tags registered");

			string result = master.RenderToString("canvas");

			Assert.AreEqual(tcount, master.MasterCustomTagFactory.CustomTags.Count);

		}

		[Test]
		public void CustomTemplatesTagsRecognized()
		{
			GadgetReferencingTemplateLibrary testData = new GadgetReferencingTemplateLibrary();
			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source); //new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.AreEqual(0, master.MasterCustomTagFactory.CustomTags.Count, "Custom tags not initially zero");

			BasicLibrary testLib = new BasicLibrary();
			TemplatesRoot library = master.LoadTemplateLibrary(testData.ExpectedTemplateLibraryUri, BasicLibrary.Source);

			Assert.IsTrue(master.MasterCustomTagFactory.IsCustomTag("foo:bar"));
			Assert.IsTrue(master.MasterCustomTagFactory.IsCustomTag("foo:dog"));

		}

	}
}

using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.DataContracts;

using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.TestData.Partials;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Tests.Gadget
{
	[TestFixture]
	[TestsOn(typeof(ModulePrefs))]
	public class ModulesPrefsTests : OsmlControlTestBase
	{
		Person viewer = GadgetTestData.Viewer;

		/// <summary>
		/// Creates the test object
		/// </summary>
		/// <returns></returns>
		ModulePrefs GetModulePrefsObject()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			ModulePrefs target = new ModulePrefs(ModulePrefsData.Source, master);
			return target;
		}

		/// <summary>
		/// Creates the test object
		/// </summary>
		/// <returns></returns>
		ModulePrefs GetModulePrefsFullDataObject()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			ModulePrefs target = new ModulePrefs(ModulePrefsFullData.Source, master);
			return target;
		}


		[Test]
		public void TestOffsetExistsParse()
		{
			//ModulePrefsData data = new ModulePrefsData();

			ModulePrefs target = GetModulePrefsObject();
			
			Assert.IsNotNull(target.MyOffset, "Root offset is null");
			Assert.AreEqual(new ModulePrefs().OffsetKey, target.MyOffset.OffsetKey, "incorrect root offset type");
			Assert.IsTrue(target.MyOffset.ChildOffsets.Count > 1);
		}

		[Test]
		public void TestOffsetParseCorrect()
		{
			//ModulePrefsData data = new ModulePrefsData();

			ModulePrefs target = GetModulePrefsObject();

			Assert.AreEqual(ModulePrefsData.ExpectedOffsets, target.MyOffset.ToString(), "Offset list bad");
			
		}

		[Test]
		public void TestAttributeParse()
		{
			//ModulePrefsData data = new ModulePrefsData();

			ModulePrefs target = GetModulePrefsObject();

			Assert.AreEqual(ModulePrefsData.ExpectedTitle, target.Title, "Incorrect title set");
		}

		[Test]
		public void ThumbnailSetTest()
		{
			//ModulePrefsFullData
			ModulePrefs target = GetModulePrefsFullDataObject();

			Assert.IsFalse(string.IsNullOrEmpty(target.Thumbnail));
			Assert.AreEqual(ModulePrefsFullData.ExpectedThumbnail, target.Thumbnail);
		}
		[Test]
		public void IconSetTest()
		{
			//ModulePrefsFullData
			ModulePrefs target = GetModulePrefsFullDataObject();

			Assert.IsFalse(string.IsNullOrEmpty(target.IconUrl));
			Assert.AreEqual(ModulePrefsFullData.ExpectedIcon, target.IconUrl);
		}
		[Test]
		public void TitleSetTest()
		{
			//ModulePrefsFullData
			ModulePrefs target = GetModulePrefsFullDataObject();

			Assert.IsFalse(string.IsNullOrEmpty(target.Title));
			Assert.AreEqual(ModulePrefsFullData.ExpectedTitle, target.Title);
		}

		[Test]
		public void FormattedModulePrefsParse()
		{
			//ModulePrefsFullData
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			ModulePrefs target = new ModulePrefs(ModulePrefsFormattedLargeData.Source, master);

			Assert.IsFalse(string.IsNullOrEmpty(target.Title), "Title not set");
			Assert.AreEqual(ModulePrefsFormattedLargeData.ExpectedTitle, target.Title, "Title incorrect value");
		}

		


	}
}

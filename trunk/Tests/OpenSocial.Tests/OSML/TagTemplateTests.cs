using System;
using System.IO;

using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Tests.Controls;
using Negroni.OpenSocial.Tests.Helpers;
using Negroni.OpenSocial.Tests.TestData;

namespace Negroni.OpenSocial.Tests.OSML
{
	[TestFixture]
	[TestsOn(typeof(OsTagTemplate))]
	public class TagTemplateTests : OsmlControlTestBase
	{

		[Test]
		public void TestDefineTag()
		{
			TestTagTemplate data = GadgetTestData.TagTemplates.SimpleTemplate;
			OsTagTemplate target = new OsTagTemplate(data.Tag);

			Assert.AreEqual(data.Tag, target.Tag);
		}

		[Test]
		public void TestTagAndSource()
		{
			TestTagTemplate data = GadgetTestData.TagTemplates.SimpleTemplate;
			OsTagTemplate target = new OsTagTemplate();
			//			OsTagTemplate target = new OsTagTemplate(data.Tag, data.Source, TEST_FACTORY_KEY);
			target.MyRootMaster.MyControlFactory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			target.LoadTag(data.Source);

			Assert.AreEqual(data.Source, target.RawTag);
		}

		[Test]
		public void TestMarkupLoad()
		{
			TestTagTemplate data = GadgetTestData.TagTemplates.SimpleTemplate;
			OsTagTemplate target = new OsTagTemplate();
			//target.MyRootMaster = new RootElementMaster();
			target.MyRootMaster.MyControlFactory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			target.LoadTag(data.Source);

			Assert.AreEqual(data.Source, target.RawTag);
			Assert.AreEqual(data.Tag, target.Tag);
		}

		[Test]
		public void TestTagParse()
		{
			TestTagTemplate data = GadgetTestData.TagTemplates.SimpleTemplate;
			OsTagTemplate target = new OsTagTemplate();
			target.MyRootMaster.MyControlFactory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			target.LoadTag(data.Source);

			Assert.AreEqual(data.Tag, target.Tag);
			CustomTagFactory fact = new CustomTagFactory();
			fact.RegisterCustomTag(target);

			CustomTag result = fact.CreateTagInstance(data.Tag);
			Assert.AreEqual(data.Tag, result.MarkupTag);

			Assert.AreEqual("my", target.Prefix);
			Assert.AreEqual("Thing", target.LocalTag);

		}

		[Test]
		public void TagParameterRegistrationVariantsTest()
		{
			GadgetCustomTagTemplates testData = new GadgetCustomTagTemplates();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source);

			string result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));

			Assert.IsNotNull(result, "Empty render result");
			Assert.IsTrue(result.Contains(testData.ExpectedParamAttributeValue), "Attribute parameter not correctly rendered");
			Assert.IsTrue(result.Contains(testData.ExpectedParamElementValue), "Element parameter not correctly rendered");
			Assert.IsTrue(result.Contains(testData.ExpectedVariableValue), "osVar parameter not correctly rendered");

		}

	}
}

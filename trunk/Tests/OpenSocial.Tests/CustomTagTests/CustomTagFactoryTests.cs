using System;
using System.IO;

using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.OSML;
using Negroni.OpenSocial.Test.Controls;

namespace Negroni.OpenSocial.Test
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="CustomTagFactory"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(CustomTagFactory))]
	public class CustomTagFactoryTests : OsmlControlTestBase
    {
		public const string TEMPLATE_CONTENTS = @"<h1>hello foo</h1>";
		public const string CUSTOM_TAG_TEMPLATE = @"<script type='text/os-template' tag=""my:Tag"">" + TEMPLATE_CONTENTS + "</script>";
		public const string CUSTOM_TAG_INSTANCE = "<my:Tag></my:Tag>";
		public const string CUSTOM_TAG = "my:Tag";
		public const string CUSTOM_TAG_RESULT = "<h1>hello foo</h1>";

		[Test]
		public void RegisterTag()
		{
			CustomTagFactory factory = new CustomTagFactory();
			factory.MyControlFactory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			Assert.IsTrue(0 == factory.CustomTags.Count);

			string tag = CUSTOM_TAG;

			factory.RegisterCustomTag(tag, CUSTOM_TAG_TEMPLATE);
			Assert.IsTrue(1 == factory.CustomTags.Count);

			CustomTag inst = factory.CreateTagInstance(tag);
			Assert.AreEqual(tag, inst.MarkupTag);

		}

		CustomTagFactory BuildInitTagFactoryWithTemplate()
		{
			CustomTagFactory factory = new CustomTagFactory();
			factory.MyControlFactory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);

			RootElementMaster tmpMaster = new RootElementMaster(TEST_FACTORY_KEY);

			CustomTagTemplate tagTemplate = new CustomTagTemplate();
			tagTemplate.MyRootMaster = tmpMaster;
			tagTemplate.LoadTag(CUSTOM_TAG_TEMPLATE);
			tagTemplate.OverrideInstanceMarkupTag("script");
			factory.RegisterCustomTag(tagTemplate);

			return factory;
		}


		[Test]
		public void RegisterTagTemplate()
		{
			CustomTagFactory factory = BuildInitTagFactoryWithTemplate();

			Assert.IsTrue(factory.IsCustomTag(CUSTOM_TAG));
		}


		[Test]
		public void RenderClientSide()
		{
			CustomTagFactory factory = BuildInitTagFactoryWithTemplate();

			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			factory.RenderClientTemplates(writer);
			writer.Flush();

			string result = ControlTestHelper.GetStreamContent(stream);

			//string templateDefinedTestString = CustomTagFactory.CLIENT_TEMPLATE_TAG_ATTRIBUTE + "=\"" + CUSTOM_TAG + "\"";
			string templateDefinedTestString = "<Template"; // + CUSTOM_TAG + "\"";

			Assert.IsFalse(string.IsNullOrEmpty(result), "Client template is empty");
			Assert.IsTrue(-1 != result.IndexOf(TEMPLATE_CONTENTS), "Expected template contents not found:\n\n" + result);
			Assert.IsTrue(-1 != result.IndexOf(templateDefinedTestString), "Client template identifier not found:\n\n" + result);
			
		}


		[Test]
		public void SuppressRenderClientSide()
		{
			CustomTagFactory factory = BuildInitTagFactoryWithTemplate();

			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			CustomTagTemplate template = factory.CustomTags[CUSTOM_TAG];
			template.ClientRegister = false;

			factory.RenderClientTemplates(writer);
			writer.Flush();

			string result = ControlTestHelper.GetStreamContent(stream);

			string templateDefinedTestString = CustomTagFactory.CLIENT_TEMPLATE_TAG_ATTRIBUTE + "=\"" + CUSTOM_TAG + "\"";

			Assert.IsTrue(-1 == result.IndexOf(templateDefinedTestString), "Client template incorrectly rendered" + result);

		}


	}
}

using System;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.Test.TestData;

namespace Negroni.OpenSocial.Test.OSML
{
    /// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="IOffsetParser"/> class.
    /// </summary>
	[TestFixture]
	[TestsOn(typeof(IOffsetParser))]
	public class OffsetParserTest : OsmlControlTestBase
	{


		//[TestFixtureSetUp]
		//public void FixtureSetup()
		//{
		//    base.
		//}

		IOffsetParser target = null;

		[Test]
		public void TestSimpleTraceParse()
		{
			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			target = ParserFactory.GetTraceOffsetParser(factory);
			OffsetItem testItem = target.ParseOffsets(GadgetTestData.Templates.RawSimpleMarkup, new ParseContext(typeof(ContentBlock)));
			OffsetItem item = new OffsetItem(GadgetTestData.Templates.ExpectedSimpleOffsets);
			Assert.AreEqual(item.ToString(), testItem.ToString());
		}


		[Test]
		public void TestTemplateParse()
		{
			target = ParserFactory.GetOffsetParser(testFactory);
			OffsetItem testItem = target.ParseOffsets(GadgetTestData.Templates.RawSimpleMarkup, new ParseContext(typeof(ContentBlock)));
			OffsetItem item = new OffsetItem(GadgetTestData.Templates.ExpectedSimpleOffsets);
			Assert.AreEqual(item.ToString(), testItem.ToString());
		}

		[Test]
		public void TestContentParse()
		{
			target = ParserFactory.GetOffsetParser(testFactory);
			OffsetItem testItem = target.ParseOffsets(GadgetTestData.ContentBlock.RawSimpleMarkup, new ParseContext(typeof(GadgetMaster)));
			OffsetItem item = new OffsetItem(GadgetTestData.ContentBlock.ExpectedSimpleOffsets);
			Assert.AreEqual(item.ToString(), testItem.ToString());
		}

		[Test]
		public void TestDataPipelineParse()
		{
			target = ParserFactory.GetOffsetParser(testFactory);
			OffsetItem testItem = target.ParseOffsets(GadgetTestData.DataContentBlock.RawSimpleMarkup, new ParseContext(typeof(GadgetMaster)));
			OffsetItem item = new OffsetItem(GadgetTestData.DataContentBlock.ExpectedSimpleOffsets);
			Assert.AreEqual(item.ToString(), testItem.ToString());
		}


	}
}
	
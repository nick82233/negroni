using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Test.Controls;


namespace Negroni.OpenSocial.Test.OSML
{
    /// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="OsmlLiteral"/> class.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(GadgetLiteral))]
	public class OsmlLiteralTest : OsmlControlTestBase
    {

		[RowTest]
		[Row("Hello World")]
		[Row("<h1>Hello</h1>\nHello World")]
		public void TestLiteralTextRender(string raw)
		{
			GadgetLiteral control = new GadgetLiteral(raw);

			Assert.IsTrue(AssertRenderResultsEqual(control, raw), "Value not expected - wanted " + raw);
		}

		[RowTest]
		[Row("Hello World")]
		[Row("<h1>Hello</h1>\nHello World")]
		[Row("<h1>Hello</h1>\nHello <World I am mal>")]
		public void LiteralTextInnerMarkupMatches(string raw)
		{
			GadgetLiteral control = new GadgetLiteral(raw);

			Assert.AreEqual(control.RawTag, control.InnerMarkup);
		}


		[RowTest]
		[Row("Hello World", true, "Hello World")]
		[Row("Hello World", false, "Hello World")]
		[Row("<![CDATA[Hello World]]>", true, "Hello World")]
		[Row("<![CDATA[Hello World]]>", false, "<![CDATA[Hello World]]>")]
		[Row(@"<![CDATA[
		Hello World
<H1> I am zool</h1>
]]>", false, @"<![CDATA[
		Hello World
<H1> I am zool</h1>
]]>")]
		[Row(@"<![CDATA[
		Hello World
<H1> I am zool</h1>
]]>", true, @"Hello World<H1> I am zool</h1>")]
		[Row(@"<![CDATA[
		Hello World
 <H1> I am zool</h1>
<script>
//<![CDATA[
var x=1;
//]]>
</script>
]]>", true, @"Hello World <H1> I am zool</h1>
<script>
//<![CDATA[
var x=1;
//]]>
</script>
")]
		[Row("<![CDATA[Hello World]]> I rule", true, "Hello World I rule")]
		public void LiteralsWithCDataHonored(string raw, bool suppressCDATA, string expected)
		{
			GadgetLiteral control = new GadgetLiteral(raw);

			Assert.AreEqual(raw, control.RawTag);
			Assert.AreEqual(raw, control.InnerMarkup);

			control.SuppressCDATATags = suppressCDATA;
			string rendered = ControlTestHelper.NormalizeRenderResult(
				ControlTestHelper.GetRenderedContents(control));

			expected = ControlTestHelper.NormalizeRenderResult(expected);

			Assert.AreEqual(expected, rendered, "Rendered results do not match expectations");
		}


	}
}
	
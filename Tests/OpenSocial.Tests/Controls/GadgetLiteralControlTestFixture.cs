using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Tests.TestData;


namespace Negroni.OpenSocial.Tests.Controls
{
	/// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="BaseContainerControl"/> class.
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(GadgetLiteral))]
	public class GadgetLiteralControlTestFixture
	{

		[RowTest]
		[Row("<help>Hello World</help>")]
		[Row("Hello World<help/>")]
		[Row(GadgetLiteral.CDATA_START_TAG + "Hello World" + GadgetLiteral.CDATA_END_TAG)]
		public void RawTagSet(string markup)
		{
			BaseGadgetControl target = new GadgetLiteral(markup);

			Assert.IsFalse(string.IsNullOrEmpty(target.RawTag));
			Assert.AreEqual(markup, target.RawTag);
		}

		[RowTest]
		[Row("<help>Hello World</help>", "help", "Hello World")]
		[Row("Hello World<help/>", "", "Hello World<help/>")]
		[Row("<h1>Hello World<h1/> What is this?", "", "<h1>Hello World<h1/> What is this?")]
		[Row("<h1>Hello World<h1/> What is this? <b>You rule</b>", "", "<h1>Hello World<h1/> What is this? <b>You rule</b>")]
		[Row("<h1>Hello</h1>Hello <World I am mal>", "", "<h1>Hello</h1>Hello <World I am mal>")]
		[Row(GadgetLiteral.CDATA_START_TAG + "Hello World" + GadgetLiteral.CDATA_END_TAG, "", GadgetLiteral.CDATA_START_TAG + "Hello World" + GadgetLiteral.CDATA_END_TAG)]
		public void InnerMarkupAndMarkupTagSet(string markup, string expectedTag, string expectedInner)
		{
			BaseGadgetControl target = new GadgetLiteral(markup);

			Assert.IsFalse(string.IsNullOrEmpty(target.RawTag));
			Assert.AreEqual(expectedTag, target.MarkupTag, "MarkupTag incorrect");
			Assert.AreEqual(expectedInner, target.InnerMarkup, "InnerMarkup incorrect");
		}

	}
}

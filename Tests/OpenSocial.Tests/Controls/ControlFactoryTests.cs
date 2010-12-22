using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;

using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.OSML;

namespace Negroni.OpenSocial.Test.Controls
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="ControlFactory.Instance"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(ControlFactory))]
	public class ControlFactoryTests : OsmlControlTestBase
    {

		[Test]
		[ExpectedException(typeof(ControlFactoryNotDefinedException))]
		public void UndefinedControlFactoryGetThrows()
		{
			string badFactoryKey = "zpooozabaza";

			ControlFactory.GetControlFactory(badFactoryKey);
		}



		[RowTest]
		[Row("Hello world", null)]
		[Row("<h1>Hello</h1> world", "h1")]
		[Row("<h1>Hello</h1>", "h1")]
		[Row("<foo/>", "foo")]
		[Row("<foo />", "foo")]
		[Row("<foo altern='ff'/>", "foo")]
		[Row("<foo\nxx='zn'/>", "foo")]
		[Row("<foo\txx='zt'/>", "foo")]
		[Row("<foo\n\txx='znt'/>", "foo")]
		[Row("<!-- foo\nxx='z'-->", null)]
		public void GetTagNameTasts(string markup, string expectedTag)
		{
			Assert.AreEqual(expectedTag, ControlFactory.GetTagName(markup));
		}


		[RowTest]
		[Row("Hello world", true)]
		[Row("<h1>Hello</h1> world", true)]
		[Row("<h1>Hello</h1 world", false)]
		public void IsTagBalancedTest(string markup, bool expected)
		{
			Assert.AreEqual(expected, ControlFactory.IsTagBalancedElement(markup));			
		}

		[RowTest]
		[Row("Hello world", true)]
		[Row("<h1>Hello</h1> world", false)]
		[Row("<h1>Hello world</h1> this is not single<h1>help</h1>", false)]
		[Row("<h1>Hello</h1>Hello <World I am mal>", false)]
		[Row("<New/>", true)]
		[Row("<New test='one'/>", true)]
		[Row("<New test='one' />", true)]
		[Row("<h1>Hello</h1>Hello <empty/>", false)]
		[Row("<h1>Hello</h1> <h1>goodbye</h1>", false)]
		[Row("<!-- Hello world <b>something</b> -->", true)]
		public void IsEncapsulatedElementTest(string markup, bool expected)
		{
			Assert.AreEqual(expected, ControlFactory.IsEncapsulatedElement(markup));
		}


		[RowTest]
		[Row("MySpace.OpenSocial.OSML.OsHtml", null, null, true)]
		[Row("MySpace.OpenSocial.OSML.OsHtml", "OsHtml", null, true)]
		[Row("MySpace.OpenSocial.OSML.OsHtml", "FogBugz", null, false)]
		[Row("MySpace.OpenSocial.OSML.OsHtml", "Doof.FogBugz.*", null, false)]
		[Row("MySpace.OpenSocial.OSML.OsHtml", "MySpace.OpenSocial.OSML.*", null, true)]
		[Row("MySpace.OpenSocial.OSML.OsHtml", null, "OsHtml", false)]
		[Row("MySpace.OpenSocial.OSML.OsHtml", "MySpace.Xyz.*", null, false)]
		[Row("MySpace.OpenSocial.Gadget.GadgetMaster", null, "GadgetMaster", false)]
		[Row("MySpace.OpenSocial.Gadget.GadgetMaster", null, "dogmeat|GadgetMaster", false)]
		[Row("MySpace.OpenSocial.Gadget.GadgetMaster", null, "GadgetMaster|dogmeat", false)]
		[Row("MySpace.OpenSocial.Gadget.GadgetMaster","GadgetMaster|dogmeat", null, true)]
		[Row("MySpace.OpenSocial.Gadget.GadgetMaster", "dogmeat|GadgetMaster|foo", null, true)]
		[Row("MySpace.OpenSocial.Gadget.GadgetMaster", "dogmeat|MySpace.OpenSocial.Gadget.*", null, true)]
		[Row("MySpace.OpenSocial.Gadget.GadgetMaster", null, "dogmeat|MySpace.OpenSocial.Gadget.*", false)]
		public void ControlFiltersFunction(string className, string includeFilter, string excludeFilter, bool expectedResult)
		{
			Assert.AreEqual(expectedResult, ControlFactory.ClassPassesFilters(className, includeFilter, excludeFilter));
		}

	}
}
	
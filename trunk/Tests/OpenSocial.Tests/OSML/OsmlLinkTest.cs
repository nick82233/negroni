using System;
using System.IO;
using MbUnit.Framework;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;

namespace Negroni.OpenSocial.Test.OSML
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlNavTest"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(OsmlNavTest))]
	public class OsmlNavTest : OsmlControlTestBase
    {
		const string viewName = "friendList";
		const string content = "My Friends";
		readonly string expectedTag = "<a href=\"javascript:OSML.navToView('" + viewName + "');\">" + content + "</a>";
		readonly string rawMarkup = "<os:Nav view='friendList'>My Friends</os:Nav>";

		[Test]
		public void TestConstructorRender()
		{
			//Assert.IsTrue(true);

			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			OsmlNav control = new OsmlNav(rawMarkup, master);

			Assert.IsTrue(AssertRenderResultsEqual(control, expectedTag));
			
		}


		[Test]
		public void TestLoadRawTagRender()
		{
			//Assert.IsTrue(true);

			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			OsmlNav control = new OsmlNav();
			control.MyRootMaster = master;
			control.LoadTag(rawMarkup);

			Assert.IsTrue(AssertRenderResultsEqual(control, expectedTag));
		}

		[Test]
		public void InnerMarkupText()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			OsmlNav control = new OsmlNav(rawMarkup, master);
			Assert.AreEqual(content, control.InnerMarkup);
		}


	}
}
	
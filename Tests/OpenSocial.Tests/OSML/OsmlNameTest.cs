using System;
using System.IO;
using MbUnit.Framework;
using Negroni.DataPipeline;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Tests.Controls;

namespace Negroni.OpenSocial.Tests.OSML
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlName"/> class.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(OsmlName))]
	public class OsmlNameTest : OsmlControlTestBase
    {
		const string baseTag = "<os:Name person=\"${Viewer}\" />";
		const int vUid = 99;
		const string vname = "Steve";
		const string expectedTag = "<a href=\"\">" + vname + "</a>";

		[Test]
		public void TestRenderFromMarkup()
		{
			//Assert.IsTrue(true);
			

			Person viewer = ControlTestHelper.CreatePerson(vUid, vname, null);

			OsmlName control = new OsmlName(baseTag);
			DataContext dc = control.MyDataContext;
			GenericExpressionEvalWrapper wrapper = new GenericExpressionEvalWrapper(viewer);
			dc.RegisterDataItem("Viewer", wrapper);
			
			Assert.IsTrue(AssertRenderResultsEqual(control, expectedTag));
			
		}


		[Test]
		public void InnerMarkupIsEmpty()
		{
			Person viewer = ControlTestHelper.CreatePerson(vUid, vname, null);

			OsmlName target = new OsmlName(baseTag);

			Assert.IsEmpty(target.InnerMarkup, "Markup not empty on empty tag");

		}

	}
}
	
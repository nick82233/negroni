using System;
using System.IO;
using MbUnit.Framework;
using Negroni.DataPipeline;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.Controls;

namespace Negroni.OpenSocial.Test.OSML
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlName"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(OsHtml))]
	public class OsHtmlTest : OsmlControlTestBase
    {
		const string baseTag = "<os:Html code=\"${foo}\" />";
		const string baseKey = "foo";

		[Test]
		public void TestRenderFromMarkup()
		{
			string markup = "<h1>Hello World</h1>";

			OsHtml control = new OsHtml(baseTag);
			DataContext dc = control.MyDataContext;
			dc.RegisterDataItem(baseKey, markup);

			Assert.IsTrue(AssertRenderResultsEqual(control, markup));
			
		}


	}
}
	
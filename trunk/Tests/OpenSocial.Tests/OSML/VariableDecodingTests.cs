using System;
using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.Tests.Controls;
using Negroni.DataPipeline;
using Negroni.OpenSocial.Tests.TestData;

namespace Negroni.OpenSocial.Tests.OSML
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlControl"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(BaseGadgetControl))]
	public class VariableDecodingTests
    {

		const string vname = "Steve";
		const int vUid = 99;

		Person viewer = null;
		DataContext context = null;

		[SetUp]
		public void SetupTest(){
			viewer = new Person { Id = vUid.ToString(), DisplayName = vname };
			context = new DataContext();
			OsViewerRequest vreq = new OsViewerRequest();
			vreq.Key = "Viewer";
			context.RegisterDataItem(vreq);
			AccountTestData.ResolveDataControlValues(context, viewer, viewer, null);
			
		}

		[RowTest]
		[Row(new object[]{"Hello ${Viewer.displayName}.  You suck", "Hello " + vname + ".  You suck"})]
		[Row(new object[] { "Static", "Static"})]
		[Row(new object[] { "${Viewer.displayName}", vname })]
		[Row(new object[] { " ${Viewer.displayName}", " " + vname })]
		[Row(new object[] { "${Viewer.displayName} noon.", vname + " noon."})]
		[Row(new object[] { "who ${Viewer.displayName}", "who " + vname})]
		[Row(new object[] { "${Viewer.displayName}${Viewer.displayName}${Viewer.displayName}sucks${Viewer.displayName}", vname + vname + vname + "sucks" + vname })]
		public void TestVariableReplacement(string sourceString, string expectedResult)
		{
			string result = BaseGadgetControl.ResolveDataContextVariables(sourceString, context);
			Assert.AreEqual(expectedResult, result, String.Format("Resolved values do not match:\ngot:\n{0}\n\nwanted:\n{1}", result, expectedResult));
		}

	}
}
	
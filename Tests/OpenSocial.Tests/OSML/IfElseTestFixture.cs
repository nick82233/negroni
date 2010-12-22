using System;
using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;



using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.TestData.Partials;
using Negroni.OpenSocial.Test.Controls;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Test.OSML
{
	/// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsIfTag"/> class.
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(OsIfTag))]
	public class IfElseTestFixture : OsmlControlTestBase
	{


		[RowTest]
		[Row("${true}", true)]
		[Row("${false}", false)]
		public void SimpleIfConditionRender(string conditionString, bool expectedTrue)
		{
			IfTestData testData = new IfTestData(conditionString, expectedTrue);
			TestRenderResult(testData);
		}

		[Test]
		public void OffsetParsed()
		{
			IfTestData testData = new IfTestData("true", true);
			GadgetMaster master = null;
			master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source);
			master.Parse();

			string offsets = master.MyOffset.ToString();
			Assert.IsTrue(offsets.IndexOf("os_If") > -1, "If offset not found in: " + offsets);
		}



		public void TestRenderResult(IfTestData testData)
		{
			GadgetMaster master = null;
			master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source);
			ResolveDataControlValues(master.MyDataContext, testData.ExpectedViewer, testData.ExpectedViewer, testData.ExpectedFriends);
			master.RenderingOptions.DivWrapContentBlocks = false;

			string result = master.RenderToString("canvas");
			string expected = testData.ExpectedCanvas; //gadget.ExpectedCanvas.Trim().Replace("\r\n", "\n");
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(expected), ControlTestHelper.NormalizeRenderResult(result), "Rendered results incorrect");

		}


	}
}

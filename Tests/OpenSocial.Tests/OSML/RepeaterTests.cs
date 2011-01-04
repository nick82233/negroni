using System;
using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;



using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.TestData.Partials;
using Negroni.OpenSocial.Tests.Controls;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Tests.OSML
{
	/// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlTemplateTest"/> class.
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(OsmlRepeater))]
	public class RepeaterTests : OsmlControlTestBase
	{


		[Test]
		public void TestTagRepeater()
		{
			DataGadgetTestData data = new DataGadgetTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}
		[Test]
		public void TestTagRepeaterInJavaScript()
		{
			DataGadgetRepeatInJavaScriptBlock data = new DataGadgetRepeatInJavaScriptBlock();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}
		

		[Test]
		public void TestEmptyElementAttributeRepeater()
		{
			DataGadgetTestData data = new DataGadgetEmptyAttrRepeatTestData();
			//TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}
		[Test]
		public void TestAttributeRepeater()
		{
			DataGadgetTestData data = new DataGadgetAttrRepeatTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}
		[Test]
		public void AttributeRepeaterNestedInDuplicateTag()
		{
			DataGadgetTestData data = new DataGadgetNestedTagAttrRepeatTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}
		[Test]
		public void TestStaticAttributeRepeater()
		{
			DataGadgetTestData data = new DataGadgetStaticAttrRepeatTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}

		[Test]
		public void RepeatingWithIfMod2()
		{
			DataGadgetTestData data = new DataGadgetRepeatIfTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}

		[Test]
		public void AttributeRepeatingWithIf()
		{
			DataGadgetTestData data = new DataGadgetAttrRepeatIfTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}

		


		[Test]
		public void RepeaterContextIndexAndCountTest()
		{
			DataGadgetTestData data = new DataGadgetRepeatContextTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}


		[Test]
		public void BuiltInVariableRenamingTest()
		{
			DataGadgetTestData data = new DataGadgetRepeatRenamedContextTestData();
			TestOsRepeaterTagFoundNotEmpty(data);
			TestRepeaterRenderResult(data);
		}


		/// <summary>
		/// Helper method to test that the tag doesn't render blank results
		/// </summary>
		/// <param name="gadget"></param>
		void TestOsRepeaterTagFoundNotEmpty(DataGadgetTestData gadget)
		{
			GadgetMaster master = null;
			MemoryStream stream = null;
			StreamWriter writer = null;

			stream = new MemoryStream();
			writer = new StreamWriter(stream);

			master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, gadget.Source);
			ResolveDataControlValues(master.MyDataContext, gadget.ExpectedViewer, gadget.ExpectedViewer, gadget.ExpectedFriends);

			master.RenderContent(writer);

			Assert.AreEqual(1, master.ContentBlocks.Count);
			ContentBlock block = master.ContentBlocks[0];
			Assert.IsNotNull(block);

			bool found = false;
			OsmlRepeater repeater = null;
			foreach (BaseGadgetControl control in block.Templates)
			{
				
				OsTemplate template = control as OsTemplate;
				if (template != null)
				{
					foreach (BaseGadgetControl innerControl in template.Controls)
					{
						if (innerControl is OsmlRepeater)
						{
							found = true;
							repeater = (OsmlRepeater)innerControl;
							break;
						}
					}
				}
			}

			Assert.IsTrue(found, "Repeater not found in gadget results");
			Assert.IsNotNull(repeater, "Repeater control is null - i.e. wasn't created");
			Assert.Greater(repeater.Controls.Count, 0, "Repeater control tree not built");
		}


		/// <summary>
		/// Helper to check the rendered results
		/// </summary>
		/// <param name="gadget"></param>
		void TestRepeaterRenderResult(DataGadgetTestData gadget)
		{
			GadgetMaster master = null;
			master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, gadget.Source);
			master.RenderingOptions.DivWrapContentBlocks = false;
			ResolveDataControlValues(master.MyDataContext, gadget.ExpectedViewer, gadget.ExpectedViewer, gadget.ExpectedFriends);
			
			string result = ControlTestHelper.GetRenderedContents(master, "canvas");
			string expected = gadget.ExpectedCanvas.Trim().Replace("\r\n", "\n");
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(expected), ControlTestHelper.NormalizeRenderResult(result), "Rendered results incorrect");

		}


	}
}

using System;
using System.IO;
using MbUnit.Framework;
using Negroni.DataPipeline;
using Negroni.OpenSocial.OSML;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Tests.Controls;
using Negroni.OpenSocial.Tests.TestData;

namespace Negroni.OpenSocial.Tests.OSML
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlTemplateTest"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(OsmlTemplateTest))]
	public class OsmlTemplateTest : OsmlControlTestBase
    {

		const int vUid = 99;
		const string vPix = "http://example.com/images/viewer.png";

		Person Viewer = null;

		
		public OsmlTemplateTest()
		{
			Viewer = new Person
			{
				Id = vUid.ToString(),
				DisplayName = GadgetTestData.Templates.vname,
				ThumbnailUrl = vPix
			};
		}


		[Test]
		public void TestControlLoadTemplate()
		{

			OsTemplate template = new OsTemplate();
			GadgetMaster master = new GadgetMaster();
			DataContext dc = master.MyDataContext;
			template.MyRootMaster = master;
			dc.RegisterDataItem("Viewer", Viewer);

			for (int i = 0; i < GadgetTestData.Templates.SimpleControls.Length; i++)
			{
				template.AddControl(GadgetTestData.Templates.SimpleControls[i]);
			}
			

			Assert.IsTrue(AssertRenderResultsEqual(template, GadgetTestData.Templates.ExpectedSimpleMarkup), "Render results do not match");

		}

		[Test]
		public void TestParsedOffsets()
		{
			MemoryStream stream = new MemoryStream(1024);
			StreamWriter w = new StreamWriter(stream);

			ControlFactory factory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
			OsTemplate template = new OsTemplate();
			template.MyControlFactory = factory;
			template.MyRootMaster = new RootElementMaster();
			template.MyRootMaster.MyControlFactory = factory;


			template.Name = "test";
			template.LoadTag(GadgetTestData.Templates.RawSimpleMarkup);
			
			template.Render(w);
			string builtOffsets = template.MyOffset.ToString();

			Assert.AreEqual(GadgetTestData.Templates.ExpectedSimpleOffsets, builtOffsets, "Offsets not matching");
		}

		



	}
}
	
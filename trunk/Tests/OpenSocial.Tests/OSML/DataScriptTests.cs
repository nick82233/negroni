using System;
using System.IO;
using System.Reflection;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.TestData.Partials;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Test.OSML
{
	/// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlTemplateTest"/> class.
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(DataScript))]
	public class DataScriptTests : OsmlControlTestBase
	{

		[Test]
		public void TestOffsetLoad()
		{
			OffsetItem offsets = new OffsetItem(DataScriptBlockData.ExpectedOffsets);
			DataScript target = new DataScript(DataScriptBlockData.Source, offsets, TEST_FACTORY_KEY);
			//if (!target.IsParsed) target.Parse();

			Assert.AreEqual(2, target.Controls.Count, "Incorrect number of controls");
			Assert.AreEqual(typeof(OsViewerRequest), target.Controls[0].GetType(), "Incorrect type of control loaded");
			Assert.AreEqual(typeof(OsPeopleRequest), target.Controls[1].GetType(), "Incorrect type of control loaded");
		}

		[Test]
		public void TestOffsetParse()
		{
			RootElementMaster root = new RootElementMaster(TEST_FACTORY_KEY);
			DataScript target = new DataScript(DataScriptBlockData.Source, null, root);

			if (!target.IsParsed) target.Parse();

			Assert.AreEqual(DataScriptBlockData.ExpectedOffsets, target.MyOffset.ToString());
		}


		[Test]
		public void TestDataItemParse()
		{
			RootElementMaster root = new RootElementMaster(TEST_FACTORY_KEY);
			DataScript target = new DataScript(DataScriptBlockData.Source, null, root);

			//DataContext dc = target.My. .MyDataContext;
			DataContext dc = target.MyRootMaster.MasterDataContext;
			Assert.IsNotNull(dc, "Data context is null");

			OsViewerRequest vreq = target.Controls[0] as OsViewerRequest;
			Assert.IsNotNull(vreq, "ViewerRequest is null");
			Assert.IsFalse(String.IsNullOrEmpty(vreq.Key), "ViewerRequest key is null");
			Assert.AreEqual("foo", vreq.Key, "Incorrect key on ViewerRequest");
		}

		[Test]
		public void TestRegisterDataItem()
		{
			RootElementMaster root = new RootElementMaster(TEST_FACTORY_KEY);
			DataScript target = new DataScript(DataScriptBlockData.Source, null, root);

			DataContext dc = target.MyRootMaster.MasterDataContext;
			target.ConfirmDataItemsRegistered();

			Assert.IsTrue(dc.HasVariable("foo"), "Viewer key not registered with DataContext");

		}

	}
}

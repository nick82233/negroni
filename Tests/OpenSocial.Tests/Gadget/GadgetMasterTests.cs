using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.Controls;
using Negroni.OpenSocial.Tests.OSML;

using Negroni.DataPipeline;
using Negroni.DataPipeline.Security;

namespace Negroni.OpenSocial.Tests.Gadget
{
	/// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="GadgetMaster"/> class.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(GadgetMaster))]
    public class GadgetMasterTests : OsmlControlTestBase
    {

		[Test]
		[ExpectedException(typeof(Exception))]
		public void RenderThrowsError()
		{
			GadgetMaster target = new GadgetMaster(testFactory, GadgetTestData.FullGadget, new OffsetItem(GadgetTestData.GadgetOffsetListString));
			MemoryStream s = new MemoryStream();
			target.Render(new StreamWriter(s));
		}


		[Test]
		public void TestLoadFromOffsets()
		{

			GadgetMaster target = new GadgetMaster(testFactory, GadgetTestData.FullGadget, new OffsetItem(GadgetTestData.GadgetOffsetListString));
			if (!target.IsParsed)
			{
				target.Parse();
			}

			Assert.IsTrue(target.RawTag.Length > 0);
			Assert.IsTrue(target.Controls.Count > 0, "No controls created");
		}


		[Test]
		public void TestGetOffsetsPostRender()
		{
			GadgetMaster target;

			target = new GadgetMaster(testFactory, GadgetTestData.FullGadget);
			
			MemoryStream output = new MemoryStream(1024);
			StreamWriter w = new StreamWriter(output);


			target.RenderContent(w);

			Assert.AreEqual(GadgetTestData.GadgetOffsetListString, target.MyOffset.ToString(), "Constructed offset list incorrect after render");
		}


		[Test]
		public void TestModulePrefsExists()
		{
			GadgetMaster target;

			target = new GadgetMaster(testFactory, GadgetTestData.FullGadget);

			target.Parse();

			Assert.IsNotNull(target.ModulePrefs);
			Assert.AreEqual("Hello World", target.ModulePrefs.Title, "Title not set in ModulePrefs");
		}




		[RowTest]
		[Row(GadgetXmlVersioned.VERSION_NONE_GADGET, "1.0")]
		[Row(GadgetXmlVersioned.VERSION_NONE_EMPTY_GADGET, "1.0")]
		[Row(GadgetXmlVersioned.VERSION_08_GADGET, "0.8")]
		[Row(GadgetXmlVersioned.VERSION_09_GADGET, "0.9")]
		[Row(GadgetXmlVersioned.VERSION_10_GADGET, "1.0")]
		public void GadgetVersionTest(string gadgetSource, string expectedVersion)
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, gadgetSource);
			Assert.AreEqual(expectedVersion, target.GetOpenSocialVersion(), "Bad version info - expected: " + expectedVersion);
		}


		[Test]
		public void LifecycleEventsDefined()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, GadgetXmlEvents.SOURCE);

			Assert.AreEqual(GadgetXmlEvents.ADD_APP_EVENT_HREF, target.GetEventLink(LifecycleEventKey.ADD_APP).Href);
			Assert.AreEqual(GadgetXmlEvents.REMOVE_APP_EVENT_HREF, target.GetEventLink(LifecycleEventKey.REMOVE_APP).Href);
		}

		[Test]
		public void MissingLifecycleEventNull()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, GadgetXmlVersioned.VERSION_09_GADGET);

			Assert.IsNull(target.GetEventLink(LifecycleEventKey.ADD_APP), "Add App is not null");
			Assert.IsNull(target.GetEventLink(LifecycleEventKey.REMOVE_APP), "Remove App is not null");
		}

		[Test]
		public void ProfileLeftViewRenders()
		{
			GadgetWithProfileLeftView data = new GadgetWithProfileLeftView();

			GadgetMaster target = new GadgetMaster(testFactory, data.Source);
			CheckRender(target, "Profile", data.ExpectedProfile);
		}

		[Test]
		public void ProfileLeftSetsGadgetLocation()
		{
			GadgetWithProfileLeftView data = new GadgetWithProfileLeftView();

			GadgetMaster target = new GadgetMaster(testFactory, data.Source);
			CheckRender(target, "Profile", data.ExpectedProfile);

			Assert.AreEqual(target.MySpaceViewSettings.ProfileLocation, "left");
		}
		[Test]
		public void ProfileRightSetsGadgetLocation()
		{
			GadgetWithProfileRightView data = new GadgetWithProfileRightView();

			GadgetMaster target = new GadgetMaster(testFactory, data.Source);
			CheckRender(target, "profile", data.ExpectedProfile);

			Assert.AreEqual(target.MySpaceViewSettings.ProfileLocation, "right");
		}

		void CheckRender(GadgetMaster target, string view, string expected)
		{
			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.ClientRenderDataContext = false;

			string result = ControlTestHelper.NormalizeRenderResult(target.RenderToString(view));
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(expected), result, view + " view incorrect");
		}


		[Test]
		public void ViewsStandardTest()
		{
			GadgetWithViews data = new GadgetWithViews();

			GadgetMaster target = new GadgetMaster(testFactory, data.Source);

			target.RenderingOptions.DivWrapContentSubViews = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.ClientRenderDataContext = false;

			string result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));
			Assert.AreEqual(data.ExpectedCanvas, result, "Canvas incorrect");

			String[,] td = new String[,]{
				{"canvas", data.ExpectedCanvas},
				{"Canvas", data.ExpectedCanvas},
				{"mobilecanvas", data.ExpectedMobileCanvas},
				{"home", data.ExpectedHome},
				{"profile", data.ExpectedProfile}
			};

			for (int i = 0; i < td.Length/2; i++)
			{
				result = ControlTestHelper.NormalizeRenderResult(target.RenderToString(td[i,0]));
				Assert.AreEqual(td[i,1], result, td[i,0] + " render incorrect");
			}
		}

		[Test]
		public void MissingViewTest()
		{
			GadgetWithViews data = new GadgetWithViews();

			GadgetMaster target = new GadgetMaster(testFactory, data.Source);

			target.RenderingOptions.DivWrapContentSubViews = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.ClientRenderDataContext = false;

			string result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("mobilecanvas"));
			Assert.AreEqual(data.ExpectedMobileCanvas, result, "Mobile Canvas incorrect");
		}
		[Test]
		public void OverlappingViewTest()
		{
			GadgetWithViewsOverlap data = new GadgetWithViewsOverlap();

			GadgetMaster target = new GadgetMaster(testFactory, data.Source);

			target.RenderingOptions.DivWrapContentSubViews = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.ClientRenderDataContext = false;
			string result;
			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("mobilecanvas"));
			Assert.AreEqual(data.ExpectedMobileCanvas, result, "Mobile Canvas incorrect");

			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));
			Assert.AreEqual(data.ExpectedCanvas, result, "Canvas incorrect");

			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("profilecanvas"));
			Assert.AreEqual(data.ExpectedProfileCanvas, result, "Profile Canvas incorrect");

		}

		[RowTest]
		[Row(new object[]{GadgetTestData.GadgetCode.MultiView.Source, "canvas", GadgetTestData.GadgetCode.MultiView.Expected_Canvas})]
		[Row(new object[]{GadgetTestData.GadgetCode.MultiView.Source, "home", GadgetTestData.GadgetCode.MultiView.Expected_Home})]
		[Row(new object[] { GadgetTestData.GadgetCode.SubView.Source, "profile", GadgetTestData.GadgetCode.SubView.Expected_Profile })]
		[Row(new object[] { GadgetTestData.GadgetCode.SubView.Source, "canvas", GadgetTestData.GadgetCode.SubView.Expected_Canvas })]
		public void TestSurfaceViewRender(string source, string surfaceName, string expected)
		{
			GadgetMaster target;

			target = new GadgetMaster(testFactory, source);
			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.DivWrapContentSubViews = false;
			AccountTestData.ResolveDataControlValues(target.MyDataContext, GadgetTestData.Viewer, GadgetTestData.Viewer, null);

			string written = ControlTestHelper.GetRenderedContents(target, surfaceName);

			expected = ControlTestHelper.NormalizeRenderResult(expected);
			written = ControlTestHelper.NormalizeRenderResult(written);

			Assert.AreEqual(expected.Length, written.Length, "Lengths do not match");

			Assert.AreEqual(expected, written, "Rendered gadget not expected from view: [" + surfaceName + "]\nEXPECTED:\n" + expected + "\n\nWRITTEN:\n" + written);
		}

		[Test]
		public void TestRenderOptions()
		{
			GadgetMaster target;
			SuppressedWhitespaceAndDivGadget gadget = new SuppressedWhitespaceAndDivGadget();

			target = new GadgetMaster(testFactory, gadget.Source);
			AccountTestData.ResolveDataControlValues(target.MyDataContext, GadgetTestData.Viewer, GadgetTestData.Viewer, null);

			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;

			string written = ControlTestHelper.GetRenderedContents(target, "canvas");

			Assert.AreEqual(gadget.ExpectedCanvas, written, "Canvas does not match");

			written = ControlTestHelper.GetRenderedContents(target, "profile");

			Assert.AreEqual(gadget.ExpectedProfile, written, "Profile does not match");
			
		}


		[Test]
		public void TestRenderCustomTag()
		{
			GadgetMaster target;
			TemplateGadgetTestData gadget = new TemplateGadgetTestData();

			target = new GadgetMaster(testFactory, gadget.Source);
			AccountTestData.ResolveDataControlValues(target.MyDataContext, GadgetTestData.Viewer, GadgetTestData.Viewer, null);

			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;

			string written = ControlTestHelper.GetRenderedContents(target, "canvas");

			written = written.Trim();
			string expected = gadget.ExpectedCanvas.Trim();

			Assert.AreEqual(expected, written, "Canvas does not match");
		}


		[Test]
		public void RenderCustomTagWithClientTemplateIncluded()
		{
			GadgetMaster target;
			TemplateGadgetTestData gadget = new TemplateGadgetTestData();

			target = new GadgetMaster(testFactory, gadget.Source);
			AccountTestData.ResolveDataControlValues(target.MyDataContext, GadgetTestData.Viewer, GadgetTestData.Viewer, null);

			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;
			target.ClientRenderCustomTemplates = true;

			string written = ControlTestHelper.GetRenderedContents(target, "canvas");

			Assert.IsTrue(written.IndexOf(TemplateGadgetTestData.CUSTOM_TAG_CONTENTS) > -1, "Client template not found");

		}

		[Test]
		public void RenderGadgetWithoutTemplates()
		{
			GadgetWithoutTemplatesData data = new GadgetWithoutTemplatesData();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);

			Assert.IsTrue(target.IsParsed);
			Assert.IsTrue(target.MyOffset.ToString().IndexOf("ContentBlock") > -1, "Content Block not found in offsets");

			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;
			target.ClientRenderCustomTemplates = false;

			string result = target.RenderToString("canvas");
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.IsFalse(string.IsNullOrEmpty(result), "empty result");

			Assert.AreEqual(data.ExpectedCanvas, result, "Rendered results are incorrect");

		}


		[Test]
		public void RenderGadgetSimpleTemplates()
		{
			GadgetWithSimpleTemplate data = new GadgetWithSimpleTemplate();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);
			ResolveDataControlValues(target.MyDataContext, data.ExpectedViewer, data.ExpectedViewer, data.ExpectedFriends);


			Assert.IsTrue(target.IsParsed);
			Assert.IsTrue(target.MyOffset.ToString().IndexOf("ContentBlock") > -1, "Content Block not found in offsets");

			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;
			target.ClientRenderCustomTemplates = false;

			string result = target.RenderToString("canvas");
			result = ControlTestHelper.NormalizeRenderResult(result);
			//remove double spaces
			while (result.Contains("  "))
			{
				result = result.Replace("  ", " ");
			}
			string expectedCanvas = ControlTestHelper.NormalizeRenderResult(data.ExpectedCanvas);
			Assert.IsFalse(string.IsNullOrEmpty(result), "empty result");

			Assert.AreEqual(expectedCanvas, result, "Rendered results are incorrect");

		}




		[Test]
		public void TestDefinedDataKeys()
		{
			GadgetMaster target;
			DataGadgetTestData gadget = new DataGadgetTestData();

			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			target = new GadgetMaster(testFactory, gadget.Source);

			ResolveDataControlValues(target.MyDataContext, gadget.ExpectedViewer, gadget.ExpectedViewer, gadget.ExpectedFriends);


			target.RenderContent(writer);

			string written = ControlTestHelper.GetStreamContent(stream);

			for (int i = 0; i < gadget.ExpectedDataKeys.Length; i++)
			{
				string key = gadget.ExpectedDataKeys[i];
				Assert.IsTrue(target.MasterDataContext.HasVariable(key), "Key not registered: " + key);
			}

		}

		[Test]
		public void TestDataKeyViews()
		{
			GadgetMaster target;
			DataGadgetViewScopedTestData gadget = new DataGadgetViewScopedTestData();

			target = new GadgetMaster(testFactory, gadget.Source);

			ResolveDataControlValues(target.MyDataContext, gadget.ExpectedViewer, gadget.ExpectedViewer, gadget.ExpectedFriends);

			//vwr, myfriends

			DataContext dc = target.MasterDataContext;

			Assert.IsTrue(dc.HasVariable("vwr"), "vwr key not defined");
			Assert.IsTrue(dc.HasVariable("myfriends"), "myfriends key not defined");

			Assert.IsFalse(dc.HasVariable("foo"), "Bad variable foo appears registered");

			DataItem di;
			di = dc.MasterData["vwr"];
			Assert.IsFalse(di.IsValidForView("profile"), "incorrectly valid on profile");
			Assert.IsTrue(di.IsValidForView("canvas"), "incorrectly invalid on canvas");

		}


		[Test]
		public void RenderWithDuplicateDataKeys()
		{
			GadgetMaster target;
			DataGadgetTestData data = new DataGadgetDupeDataKeys();

			target = new GadgetMaster(testFactory, data.Source);

			ResolveDataControlValues(target.MyDataContext, data.ExpectedViewer, data.ExpectedViewer, data.ExpectedFriends);
			target.RenderingOptions.ClientRenderCustomTemplates = false;
			target.RenderingOptions.ClientRenderDataContext = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;
			
			//vwr, myfriends
			string result;
			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(data.ExpectedCanvas), result, "Canvas Incorrect");

			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("home"));
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(data.ExpectedHome), result, "Home Incorrect");

			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("profile"));
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(data.ExpectedProfile), result, "Profile Incorrect");

		}

		[Test]
		public void RenderWithSharedDataKeys()
		{
			GadgetMaster target;
			DataGadgetTestData data = new DataGadgetSharedDataKeys();

			target = new GadgetMaster(testFactory, data.Source);

			ResolveDataControlValues(target.MyDataContext, data.ExpectedViewer, data.ExpectedViewer, data.ExpectedFriends);
			target.RenderingOptions.ClientRenderCustomTemplates = false;
			target.RenderingOptions.ClientRenderDataContext = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;

			//vwr, myfriends
			string result;
			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(data.ExpectedCanvas), result, "Canvas Incorrect");

			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("home"));
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(data.ExpectedHome), result, "Home Incorrect");

			result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("profile"));
			Assert.AreEqual(ControlTestHelper.NormalizeRenderResult(data.ExpectedProfile), result, "Profile Incorrect");

		}



		[Test]
		public void TestMultiViewDataKeys()
		{
			GadgetMaster target;
			DataGadgetViewScopedTestData gadget = new DataGadgetViewScopedTestData();

			target = new GadgetMaster(testFactory, gadget.Source);

			ResolveDataControlValues(target.MyDataContext, gadget.ExpectedViewer, gadget.ExpectedViewer, gadget.ExpectedFriends);

			//vwr, myfriends

			DataContext dc = target.MasterDataContext;

			Assert.IsTrue(dc.HasVariable(gadget.GlobalDataItemKey), "global data key not defined");
			DataItem di;
			di = dc.MasterData[gadget.GlobalDataItemKey];
			Assert.IsTrue(di.IsValidForView("profile"), "incorrectly invalid on profile");
			Assert.IsTrue(di.IsValidForView("canvas"), "incorrectly invalid on canvas");

		}


		[Test]
		public void TestResolvedDataKeys()
		{
			GadgetMaster target;
			DataGadgetTestData gadget = new DataGadgetTestData();

			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			target = new GadgetMaster(testFactory, gadget.Source);
			ResolveDataControlValues(target.MyDataContext, gadget.ExpectedViewer, gadget.ExpectedViewer, gadget.ExpectedFriends);
			target.MyDataContext.ResolveDataValues();
			target.RenderContent(writer);

			string written = ControlTestHelper.GetStreamContent(stream);

			object data;
			for (int i = 0; i < gadget.ExpectedDataKeys.Length; i++)
			{
				string key = gadget.ExpectedDataKeys[i];
				Assert.IsTrue(target.MasterDataContext.HasVariable(key), "Key not registered: " + key);
				data = target.MasterDataContext.GetVariableValue(gadget.ExpectedDataKeys[i]);
				Assert.IsNotNull(data, "Data not resolved for: " + key);
			}

		}


		[Test]
		public void RepeaterGadgetTest()
		{
			DataGadgetTestData gadget = null;
			GadgetMaster target = null;
			MemoryStream stream = null;
			StreamWriter writer = null;

			stream = new MemoryStream();
			writer = new StreamWriter(stream);

			gadget = new DataGadgetTestData();
			target = new GadgetMaster(testFactory, gadget.Source);


			target.RenderingOptions.DivWrapContentBlocks = false;
			ResolveDataControlValues(target.MyDataContext, gadget.ExpectedViewer, gadget.ExpectedViewer, gadget.ExpectedFriends);
			string written = ControlTestHelper.GetRenderedContents(target, "canvas");

			written = written.Trim();
			string expected = gadget.ExpectedCanvas.Trim();

			Assert.AreEqual(expected, written, "Repeater canvas does not match");
		}

		[Test]
		public void ReParseWithMessageBundle()
		{
			InternationalGadgetTestData data = new InternationalGadgetTestData();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);
			int ctlCount = target.CountInternalControls();

			Assert.Greater(ctlCount, 0);
			target.ReParse();
			Assert.AreEqual(ctlCount, target.CountInternalControls());
		}



		[Test]
		public void SecurityPolicyDefaultCorrect()
		{
			InternationalGadgetTestData data = new InternationalGadgetTestData();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);

			Assert.AreEqual(target.ModulePrefs.SecurityPolicy.EL_Escaping, DataPipeline.Security.EL_Escaping.None,
				"Incorrect EL Escaping set");

			Assert.AreEqual(target.ModulePrefs.SecurityPolicy.EL_Escaping, target.MasterDataContext.Settings.SecurityPolicy.EL_Escaping,
				"DataContext policy doesn't match ModulePrefs policy");
		}

		[Test]
		public void SecurityPolicyAffectsRender()
		{
			SecurityPolicyGadget data = new SecurityPolicyGadget();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);
			target.RenderingOptions.DivWrapContentBlocks = false;

			string key = "markup";
			string result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));

			Assert.AreEqual(data.ExpectedCanvas, result);

		}


		[Test]
		public void SecurityPolicyChangeAffectsRender()
		{
			SecurityPolicyGadget data = new SecurityPolicyGadget();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);
			target.RenderingOptions.DivWrapContentBlocks = false;

			string key = "markup";

			target.MasterDataContext.Settings.SecurityPolicy.EL_Escaping = EL_Escaping.None;
			string elResult = target.MasterDataContext.CalculateVariableValue(key);

			target.MasterDataContext.Settings.SecurityPolicy.EL_Escaping = EL_Escaping.Html;
			string elEscapeResult = target.MasterDataContext.CalculateVariableValue(key);

			Assert.AreNotEqual(elResult, elEscapeResult, "Security Policy Manual set did not work");

		}


		[Test]
		public void ExternalSourceGadgetRegisters()
		{
			ExternalSourceGadgetTestData data = new ExternalSourceGadgetTestData();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);

			Assert.IsTrue(target.HasExternalServerRenderControls(), "No external render controls registered");
			Assert.IsTrue(target.HasExternalServerRenderControls("home"));
			//Assert.IsFalse(target.HasExternalServerRenderControls("canvas"));

			List<IExternalDataSource> srcs = target.GetExternalServerRenderControls("home");
			Assert.AreEqual(data.ExpectedExternalControlCount, srcs.Count, "Ext control count wrong");

		}

		[Test]
		public void EmbeddedObjectRenders()
		{
			GadgetWithObjectTag data = new GadgetWithObjectTag();

			GadgetMaster target = new GadgetMaster(testFactory, data.Source);
			string errs = null;
			if (target.Errors.HasParseErrors() && target.Errors.ParseErrors != null && target.Errors.ParseErrors.Count > 0)
			{
				errs = target.Errors.ParseErrors[0].Message;
			}
			Assert.IsFalse(target.Errors.HasParseErrors(), "Parse errors found: " + errs);

			CheckRender(target, "canvas", data.ExpectedCanvas);
		}

    }
}
	
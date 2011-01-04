using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.OSML;

namespace Negroni.OpenSocial.Tests.Controls
{
	/// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsVar"/> class.
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(OsVar))]
	public class OsVarTestFixture : OsmlControlTestBase
	{


		[Test]
		public void OsVarFoundInDataScriptParse()
		{
			GadgetVarsInDataPipeline testData = new GadgetVarsInDataPipeline();

			GadgetMaster target = new GadgetMaster(testFactory, testData.Source);
			Assert.IsTrue(target.MyOffset.ToString().Contains("os_Var"), "Offset not found for os:Var");
		}

		[Test]
		public void OsVarSetVarAlternativeFoundInDataScriptParse()
		{
			GadgetVarsInDataPipeline testData = new GadgetVarsInDataPipeline();
			string src = testData.Source.Replace("os:Var", "os:SetVar");
			GadgetMaster target = new GadgetMaster(testFactory, src);
			Assert.IsTrue(target.MyOffset.ToString().Contains("os_Var"), "Offset not found for os:SetVar");
		}

		[Test]
		public void OsVarFoundInTemplateParse()
		{
			GadgetVarsInTemplate testData = new GadgetVarsInTemplate();

			GadgetMaster target = new GadgetMaster(testFactory, testData.Source);
			Assert.IsTrue(target.MyOffset.ToString().Contains("os_Var"), "Offset not found for os:Var");
		}

		[Test]
		public void OsVarRegisteredInDataContext_FromDataScript()
		{
			GadgetVarsInDataPipeline testData = new GadgetVarsInDataPipeline();
			GadgetMaster target = new GadgetMaster(testFactory, testData.Source);
			Assert.IsTrue(target.MasterDataContext.MasterData.ContainsKey(testData.ExpectedVariableKey),
				"Key not defined");
			Assert.AreEqual(testData.ExpectedVariableValue, target.MasterDataContext.CalculateVariableValue(testData.ExpectedVariableKey),
				"Value not resolved");
		}
		[Test]
		public void OsVarRegisteredInDataContext_FromTemplate()
		{
			GadgetVarsInDataPipeline testData = new GadgetVarsInDataPipeline();
			GadgetMaster target = new GadgetMaster(testFactory, testData.Source);
			target.RenderingOptions.ClientRenderCustomTemplates = false;
			target.RenderingOptions.ClientRenderDataContext = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			string result = target.RenderToString();
			Assert.IsTrue(target.MasterDataContext.MasterData.ContainsKey(testData.ExpectedVariableKey),
				"Key not defined");
			Assert.AreEqual(testData.ExpectedVariableValue, target.MasterDataContext.CalculateVariableValue(testData.ExpectedVariableKey),
				"Value not resolved");
		}


		[Test]
		public void SimpleVarDataResolvesInEL()
		{
			GadgetVarsInDataPipeline testData = new GadgetVarsInDataPipeline();

			GadgetMaster target = new GadgetMaster(testFactory, testData.Source);
			target.RenderingOptions.ClientRenderCustomTemplates = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			string result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));
			string expected = ControlTestHelper.NormalizeRenderResult(testData.ExpectedCanvas);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void SimpleVarRendersInCustomTag()
		{
			GadgetVarsInCustomTemplateInstance testData = new GadgetVarsInCustomTemplateInstance();

			GadgetMaster target = new GadgetMaster(testFactory, testData.Source);
			target.RenderingOptions.ClientRenderCustomTemplates = false;
			target.RenderingOptions.DivWrapContentBlocks = false;
			string result = ControlTestHelper.NormalizeRenderResult(target.RenderToString("canvas"));
			string expected = ControlTestHelper.NormalizeRenderResult(testData.ExpectedCanvas);

			Assert.AreEqual(expected, result);
		}
	}
}

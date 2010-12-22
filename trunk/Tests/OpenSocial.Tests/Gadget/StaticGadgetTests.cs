using System;
using System.IO;
using System.Reflection;

using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.Controls;
using Negroni.OpenSocial.Test.OSML;
using Negroni.OpenSocial.Test.Helpers;

using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Test.Gadget
{
	/// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="GadgetMaster"/> class.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(GadgetMaster))]
    public class StaticGadgetTests : OsmlControlTestBase
    {

		[Test]
		public void StaticGadgetRender()
		{
			TestableMarkupDef data = new StaticGadgetApp();

			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);

			target.RenderingOptions.DivWrapContentBlocks = false;
			target.RenderingOptions.SuppressWhitespace = true;
			target.ClientRenderCustomTemplates = false;

			string result = target.RenderToString("canvas");
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.IsFalse(string.IsNullOrEmpty(result), "empty result");

			Assert.AreEqual(data.ExpectedCanvas, result, "Rendered Canvas results are incorrect");

			result = target.RenderToString("profile");
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.IsFalse(string.IsNullOrEmpty(result), "empty result");

			Assert.AreEqual(data.ExpectedProfile, result, "Rendered Profile results are incorrect");


		}

    }
}
	
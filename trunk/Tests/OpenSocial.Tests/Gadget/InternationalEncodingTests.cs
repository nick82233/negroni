using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.TestData.Partials;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Tests.Gadget
{
	[TestFixture]
	[TestsOn(typeof(Negroni.TemplateFramework.Parsing.TraceOffsetParser))]
	public class InternationalEncodingTests : OsmlControlTestBase
	{


		[Test]
		public void RenderNotEmpty()
		{
			InternationalGadgetTestData testData = new InternationalGadgetTestData();
			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source);

			master.RenderingOptions.ClientRenderCustomTemplates = false;
			master.RenderingOptions.ClientRenderDataContext = false;
			master.RenderingOptions.DivWrapContentBlocks = false;
			string result = master.RenderToString("canvas");

			Assert.IsFalse(string.IsNullOrEmpty(result));
		}

		[Test]
		public void RenderDiffersByCulture()
		{
			InternationalGadgetTestData testData = new InternationalGadgetTestData();
			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source);

			master.RenderingOptions.ClientRenderCustomTemplates = false;
			master.RenderingOptions.ClientRenderDataContext = false;
			master.RenderingOptions.DivWrapContentBlocks = false;

			string resultEN = master.RenderToString("canvas");
			Assert.IsFalse(string.IsNullOrEmpty(resultEN));

			master.MyDataContext.Culture = "ja";
			string resultJA = master.RenderToString("canvas");

			Assert.AreNotEqual(resultEN, resultJA);


			master.MyDataContext.Culture = "de";
			string resultDE = master.RenderToString("canvas");

			Assert.AreNotEqual(resultEN, resultDE);
	
		}


		[Test]
		public void RefetchUtf8EncodedRenderNotEmpty()
		{
			InternationalGadgetTestData testData = new InternationalGadgetTestData();
			GadgetMaster master = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, testData.Source);

			string offsets = master.MyOffset.ToString();

			byte[] encodedBytes = Encoding.UTF8.GetBytes(testData.Source);
			Decoder decoder = Encoding.UTF8.GetDecoder();
			int len = decoder.GetCharCount(encodedBytes, 0, encodedBytes.Length);
			char[] buffer = new char[len];
			decoder.GetChars(encodedBytes, 0, encodedBytes.Length, buffer, 0);

			String encodedSrc = new string(buffer);


			byte[] encodedUnicodeBytes = Encoding.Unicode.GetBytes(testData.Source);
			Decoder unicodeDecoder = Encoding.Unicode.GetDecoder();
			int lenUnicode = decoder.GetCharCount(encodedUnicodeBytes, 0, encodedUnicodeBytes.Length);
			char[] bufferUnicode = new char[lenUnicode];
			unicodeDecoder.GetChars(encodedUnicodeBytes, 0, encodedUnicodeBytes.Length, bufferUnicode, 0);

			String uniEncodedSrc = new string(buffer);



			GadgetMaster remaster = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, encodedSrc, new OffsetItem(offsets));

			GadgetMaster unicodeMaster = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, uniEncodedSrc, new OffsetItem(offsets));

			Assert.IsFalse(remaster.Errors.HasParseErrors());

			master.RenderingOptions.ClientRenderCustomTemplates = false;
			master.RenderingOptions.ClientRenderDataContext = false;
			master.RenderingOptions.DivWrapContentBlocks = false;
			string result = master.RenderToString("canvas");

			Assert.IsFalse(string.IsNullOrEmpty(result));

			remaster.RenderingOptions.ClientRenderCustomTemplates = false;
			remaster.RenderingOptions.ClientRenderDataContext = false;
			remaster.RenderingOptions.DivWrapContentBlocks = false;
			string secondResult = remaster.RenderToString("canvas");

			Assert.IsFalse(string.IsNullOrEmpty(secondResult));

		}

	}
}

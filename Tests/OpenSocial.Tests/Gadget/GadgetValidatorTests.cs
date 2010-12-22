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

using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Test.Gadget
{
	/// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="GadgetMaster"/> class.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(GadgetMaster))]
	public class GadgetValidatorTests : OsmlControlTestBase
    {



		[Test]
		public void ValidGadgetHasNoParseErrors()
		{
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, GadgetTestData.FullGadget, new OffsetItem(GadgetTestData.GadgetOffsetListString));
			Assert.IsFalse(target.Errors.HasParseErrors());
		}

		[Test]
		public void InValidGadgetHasParseErrors()
		{
			string source = GadgetTestData.FullGadget;
			//remove a bracket from the middle of the gadget
			int startPos = source.Length / 2;
			int bracketPos = source.IndexOf(">", startPos);
			string newSource = source.Substring(0, bracketPos) + source.Substring(bracketPos + 1);

			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, newSource);
			Assert.IsTrue(target.Errors.HasParseErrors());
		}

		/// <summary>
		/// Tests to see if the given array contains the passed value 
		/// </summary>
		/// <param name="testArray"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		bool ContainsValue(string[] testArray, string value)
		{
			for (int i = 0; i < testArray.Length; i++)
			{
				if (testArray[i] == value)
				{
					return true;
				}
			}
			return false;
		}


		[Test]
		public void DataCrossPointersFlagged()
		{
			DataGadgetTestData data = new DataGadgetCrosspointerParams();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);
			string[] circulars;
			Assert.IsTrue(target.Errors.HasCircularControlParameterReferences(out circulars));
			Assert.Greater(circulars.Length, 0);
			Assert.IsTrue(ContainsValue(circulars, "f1"));
			Assert.IsTrue(ContainsValue(circulars, "f2"));
			Assert.IsTrue(target.Errors.CyclicDataReferenceKeys.Count > 0);
		}

		[Test]
		public void DataThreewayPointersFlagged()
		{
			DataGadgetTestData data = new DataGadgetTriplePointerParams();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);
			string[] circulars;
			Assert.IsTrue(target.Errors.HasCircularControlParameterReferences(out circulars));
			Assert.Greater(circulars.Length, 0);
			Assert.IsTrue(ContainsValue(circulars, "f1"));
			Assert.IsTrue(ContainsValue(circulars, "f2"));
			Assert.IsTrue(ContainsValue(circulars, "f3"));
		}

		[Test]
		public void DataResolvablePointersOk()
		{
			DataGadgetTestData data = new DataGadgetResolvablePointerParams();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);
			string[] circulars;
			Assert.IsFalse(target.Errors.HasCircularControlParameterReferences(out circulars));
			Assert.AreEqual(0, circulars.Length);
		}





    }
}
	
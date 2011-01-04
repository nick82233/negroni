using System;
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
using Negroni.OpenSocial.Tests.OSML;

namespace Negroni.OpenSocial.Tests.Controls
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="ContentBlock"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(BaseDataControl))]
	public class BaseDataControlTests : OsmlControlTestBase
    {

		string controlStatic = "<os:ViewerRequest key='vwr' />";

		string dynamicPeopleControl = @"<os:PeopleRequest key='vf' startIndex=""${xyz.startIndex}"" count=""10"" />";
		string crossRefDynamicPeopleControl = @"<os:PeopleRequest key='xyz' startIndex=""1"" count=""${vf.count}"" />";

		string dynamicValueOnlyControl = @"<os:PeopleRequest key='vfo' startIndex=""1"" count=""${vf.count}"" />";
		string dynamicEmbeddedValueControl = @"<os:PeopleRequest key='vfembed' falseurl=""http://something.com/${vwr.id}"" />";


		[Test]
		public void StaticControlKeyTest()
		{
			OsViewerRequest target = new OsViewerRequest(controlStatic);
			Assert.AreEqual("vwr", target.Key);
		}

		[Test]
		public void DynamicParamTest()
		{
			OsPeopleRequest target = new OsPeopleRequest(dynamicPeopleControl);
			Assert.IsTrue(target.HasDynamicParameters());
		}

		[Test]
		public void NonDynamicParamTest()
		{
			OsViewerRequest target = new OsViewerRequest(controlStatic);
			Assert.IsFalse(target.HasDynamicParameters());
		}

		[Test]
		public void NonDynamicParamHasNoDynamicKeysTest()
		{
			OsViewerRequest target = new OsViewerRequest(controlStatic);
			Assert.AreEqual(0, target.GetDynamicKeyDependencies().Length);
		}

		[Test]
		public void GetDynamicKeysExistTest()
		{
			OsPeopleRequest tag1 = new OsPeopleRequest(dynamicPeopleControl);
			OsPeopleRequest tag2 = new OsPeopleRequest(crossRefDynamicPeopleControl);

			string[] dynamicKeys = tag1.GetDynamicKeyDependencies();
			Assert.Greater(dynamicKeys.Length, 0);
		}



		[Test]
		public void DynamicOnlyValueFound()
		{
			string expectedKey = "vf";
			Assert.IsTrue(PeopleRequestTagHasDynamicKey(dynamicValueOnlyControl, expectedKey),
				"Dynamic key not found: " + expectedKey);
		}

		[Test]
		public void EmbeddedDynamicValueFound()
		{
			string expectedKey = "vwr";
			Assert.IsTrue(PeopleRequestTagHasDynamicKey(dynamicEmbeddedValueControl, expectedKey),
				"Dynamic key not found: " + expectedKey);
		}


		[Test]
		public void EmbeddedDynamicExtraBracketsFound()
		{
			string expectedKey = "vwr";
			string ctlStr = @"<os:PeopleRequest key='vfembed' 
falseurl=""http}://something.com/${vwr.id}"" />";
			Assert.IsTrue(PeopleRequestTagHasDynamicKey(ctlStr, expectedKey),
				"Dynamic key not found: " + expectedKey);
			
		}

		[Test]
		public void MissingCloseDynamicNotFound()
		{
			string expectedKey = "vwr";
			string ctlStr = @"<os:PeopleRequest key='vfembed' 
falseurl=""http://something.com/${vwr.id"" />";
			Assert.IsFalse(PeopleRequestTagHasDynamicKey(ctlStr, expectedKey),
				"Key found incorrectly: " + expectedKey);

		}

		[Test]
		public void LiteralDynamicFound()
		{
			string expectedKey = "val";
			string ctlStr = @"<os:PeopleRequest key='vfembed' startindex=""${val}"" />";
			Assert.IsTrue(PeopleRequestTagHasDynamicKey(ctlStr, expectedKey),
				"Dynamic key not found: " + expectedKey);
		}

		[Test]
		public void MultipleDynamicKeysAllowed()
		{
			string expectedKey = "val";
			string ctlStr = @"<os:PeopleRequest key='vfembed' startindex=""${val}"" count='${other}' third=""${vwr.id}"" />";
			Assert.IsTrue(PeopleRequestTagHasDynamicKey(ctlStr, expectedKey),
				"Dynamic key not found: " + expectedKey);
		}


		bool PeopleRequestTagHasDynamicKey(string source, string expectedKey)
		{
			OsPeopleRequest tag1 = new OsPeopleRequest(source);

			string[] dynamicKeys = tag1.GetDynamicKeyDependencies();
			bool found = ContainsDynamicKey(dynamicKeys, expectedKey);
			return found;
		}

		/// <summary>
		/// Tests for a single key in the dynamicKeys arrray
		/// </summary>
		/// <param name="dynamicKeys"></param>
		/// <param name="testKey"></param>
		/// <returns></returns>
		bool ContainsDynamicKey(string[] dynamicKeys, string testKey)
		{
			bool found = false;
			for (int i = 0; i < dynamicKeys.Length; i++)
			{
				if (dynamicKeys[i] == testKey)
				{
					found = true;
					break;
				}
			}
			return found;
		}



	}
}
	
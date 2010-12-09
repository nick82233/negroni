using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Negroni.DataPipeline.Tests.TestData;

#if XUNIT
using Xunit;
#elif NUNIT
using NUnit;
using NUnit.Framework;
using NUnitExtension.RowTest;
#else
using MbUnit.Framework;
#endif

namespace Negroni.DataPipeline.Tests
{
	[TestFixture]
	[TestsOn(typeof(DataItem))]
	public class DataItemTestFixture
	{
#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void JSONSerializeData()
		{
			string val = "myfoo";
			DataItem target = new DataItem("key", null);
			SampleIJsonSerializable data = new SampleIJsonSerializable(val);
			target.Data = data;

			MemoryStream s = new MemoryStream();
			StreamWriter w = new StreamWriter(s);

			target.WriteAsJSON(w);
			w.Flush();
			string result = DataContext.GetStreamContent(s);

			Assert.AreEqual(data.ToJSON(), result, "Did not match IJsonSerializable result");

			Assert.IsTrue(result.IndexOf(val) > -1, "Value not found in serialized string");
			Assert.AreNotEqual(val, result, "Data not JSON wrapped");

		}


#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void JSONSerializeGenericObjectWithReflections()
		{

			DataItem target = Samples.GetSimpleDataItem("foo");
			target.AllowReflectiveJsonSerialization = true;

			MemoryStream s = new MemoryStream();
			StreamWriter w = new StreamWriter(s);

			target.WriteAsJSON(w);
			w.Flush();
			string result = DataContext.GetStreamContent(s);


			Assert.IsTrue(result.IndexOf(Samples.STRVAL) > -1, "Value not found in serialized string");
			Assert.AreNotEqual(Samples.STRVAL, result, "Data not JSON wrapped");

		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void ChainedDataItemRecognized()
		{
			DataItem target = Samples.GetSimpleDataItem("foo");
			DataItem target2 = Samples.GetSimpleDataItem("foo");
			target.Data = "one";
			target2.Data = "two";
			target.ViewContext = "v1";
			target2.ViewContext = "v2";

			target.ChainedDataItem = target2;

			Assert.AreEqual("two", target2.Data);
			Assert.IsTrue(target.IsValidForView("v1"));
			Assert.IsTrue(target.IsValidForView("v2"), "Chained view not recognized");
			Assert.IsFalse(target2.IsValidForView("v1"), "Second item incorrectly recognized for view");

		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void ChainedDataItemValues()
		{
			DataItem target = Samples.GetSimpleDataItem("foo");
			DataItem target2 = Samples.GetSimpleDataItem("foo");
			target.Data = "one";
			target2.Data = "two";
			target.ViewContext = "v1";
			target2.ViewContext = "v2";

			target.ChainedDataItem = target2;

			Assert.AreEqual("two", target2.Data);
			Assert.AreEqual(target2.Data, target2.GetData(), "Empty view didn't return default value");
			Assert.AreEqual("two", target.GetData("v2"), "Chained data lookup failed for view");
			Assert.AreEqual("one", target.GetData("v1"));

			Assert.IsNull(target.GetData("invalid"));

		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void ChainedDataItemGet()
		{
			DataItem target = Samples.GetSimpleDataItem("foo");
			DataItem target2 = Samples.GetSimpleDataItem("foo");
			target.Data = "one";
			target2.Data = "two";
			target.ViewContext = "v1";
			target2.ViewContext = "v2";

			target.ChainedDataItem = target2;

			DataItem dt = target.GetViewSpecificDataItem("v1");
			Assert.AreEqual(target, dt, "Root data item not returned");
			dt = target.GetViewSpecificDataItem("v2");
			Assert.AreEqual(target2, dt, "Chained data item not returned");
			dt = target.GetViewSpecificDataItem("undefined");
			Assert.IsNull(dt, "Undefined view not null");

		}

		[RowTest]
		[Row("canvas", "canvas", true)]
		[Row("home", "canvas", false)]
		[Row("canvas,home", "canvas", true)]
		[Row("home, canvas", "canvas", true)]
		[Row("profile, canvas", "home", false)]
		[Row("profile.left", "profile", true)]
		[Row("*", "profile", true)]
		[Row("*", "", true)]
		[Row("", "profile", true)]
		public void ViewValidityTests(string itemScope, string testScope, bool expectedResult)
		{
			DataItem target = Samples.GetSimpleDataItem("foo");
			target.ViewContext = itemScope;
			Assert.AreEqual(expectedResult, target.IsValidForView(testScope));
		}

	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Negroni.DataPipeline;
using Negroni.DataPipeline.Tests.TestData;
using Negroni.OpenSocial.DataContracts;

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
	[TestsOn(typeof(DataContext))]
	public class DataContextTestFixture
	{
		Person viewer;

		public DataContextTestFixture()
		{
			viewer = AccountTestData.Viewer;
		}


		[RowTest]
		[Row("one", "one", "home", "profile", "<os:Val key='foo' />", "<os:Val key='foo' />", "home", "profile", true)]
		[Row("one", "two", "home", "profile", "<os:Val key='foo' />", "<os:Val key='foo' startindex='2' />", "home", "profile", true)]
		public void DuplicateKeyAcrossViews(string val1, string val2, string viewDef1, string viewDef2, 
			string ctlDef1, string ctlDef2,
			string testView1, string testView2, bool expectedView2Defined)
		{
			string key = "foo";
			DataContext dc = new DataContext();

			SimpleDataObject data1 = new SimpleDataObject();
			data1.MyString = val1;
			SampleSimpleDataControl ctl1 = new SampleSimpleDataControl(data1);
			ctl1.Key = key;
			ctl1.RawTag = ctlDef1;

			SimpleDataObject data2 = new SimpleDataObject();
			data2.MyString = val2;
			SampleSimpleDataControl ctl2 = new SampleSimpleDataControl(data2);
			ctl2.Key = key;
			ctl2.RawTag = ctlDef2;

			dc.RegisterDataItem(ctl1, viewDef1);
			dc.RegisterDataItem(ctl2, viewDef2);

			Assert.AreEqual(true, dc.MasterData[key].IsValidForView(testView1), "View1 validity failed");
			Assert.AreEqual(expectedView2Defined, dc.MasterData[key].IsValidForView(testView2), "View2 validity not expected");

			string result;
			string var = "${foo.MyString}";
			
			dc.ActiveViewScope = testView1;
			Assert.AreEqual(val1, dc.GetVariableValue(var), "Failed check on testView1");
			dc.ActiveViewScope = testView2;
			Assert.AreEqual(val2, dc.GetVariableValue(var), "Failed check on testView2");
			

		}



#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void TestJSONInvokeSingle()
		{
			string key = "foo";
			DataContext context = new DataContext();

			SampleSimpleDataControl ctl = Samples.GetSimpleDataControl();

			int startingCount = context.DataItemCount;
			context.RegisterDataItem(key, ctl);
			context.SetAllowReflectiveSerialization(key, true);

			Assert.AreEqual(startingCount+1, context.DataItemCount);
			//context.ResolveDataValues();

			MemoryStream s = new MemoryStream();
			StreamWriter writer = new StreamWriter(s);

			context.WriteClientContext(writer);
			writer.Flush();

			string result = DataContext.GetStreamContent(s);
			SimpleDataObject rawData = (SimpleDataObject)ctl.Value;
			Assert.IsTrue(result.IndexOf(rawData.MyString) > -1, "Data not found in client context");
			Assert.AreNotEqual(rawData.MyString, result, "Client context did not wrap");
		}


		[RowTest]
		[Row("${my.love}", "my")]
		[Row("${Foo.bar}", "Foo")]
		[Row("${Dog[0].parts}", "Dog")]
		[Row("${one}", "one")]
		[Row("${a}", "a")]
		[Row("${[0].parts}", "")]
		public void GetVariableRootTest(string variable, string expectedRoot)
		{
			Assert.AreEqual(expectedRoot, DataContext.GetVariableExpressionRootKey(variable), "Failure to recover key from " + variable);
		}

		[RowTest]
		[Row("${my.love}", "my.love")]
		[Row("${Foo.bar}", "Foo.bar")]
		[Row("${Dog[0].parts}", "Dog[0].parts")]
		[Row("${one}", "one")]
		[Row("${a}", "a")]
		public void GetVariableExpressionTest(string variable, string expected)
		{
			Assert.AreEqual(expected, DataContext.GetVariableExpression(variable), "Failure getting expression " + variable);
		}
	}
}

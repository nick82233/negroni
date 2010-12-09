using System;
using System.Collections;
using System.Collections.Generic;
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
	[TestsOn(typeof(DataContext))]
	public class BulkVariableResolverTestFixture
	{

		private string testData_Template = @"
My string is ${xdata.MyString}
I say ${Msg.Hello}
";
		
		private SimpleExpressionResolverDataObject testData_data =
	new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);


#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void ResolveVariablesAll()
		{
			DataContext dc = BuildTestDataContext(testData_data);

			Assert.IsTrue(testData_Template.Contains("${Msg"));
			Assert.IsTrue(testData_Template.Contains("${xdata"));

			string result = dc.ResolveVariables(testData_Template);

			Assert.IsFalse(result.Contains("${Msg"));
			Assert.IsFalse(result.Contains("${xdata"));

			Assert.IsTrue(result.Contains(testData_data.MyString));
		}

#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void ResolveMessageBundlesOnly()
		{
			DataContext dc = BuildTestDataContext(testData_data);

			Assert.IsTrue(testData_Template.Contains("${Msg"));
			Assert.IsTrue(testData_Template.Contains("${xdata"));

			string result = dc.ResolveMessageBundleVariables(testData_Template, null);

			Assert.IsFalse(result.Contains("${Msg"));
			Assert.IsTrue(result.Contains("${xdata"));

			Assert.IsFalse(result.Contains(testData_data.MyString));
		}


#if XUNIT
		[Fact]
#else
		[Test]
#endif
		public void CultureResetsAfterResolveMessageBundlesOnly()
		{
			DataContext dc = BuildTestDataContext(testData_data);

			Assert.IsTrue(testData_Template.Contains("${Msg"));

			string result = dc.ResolveMessageBundleVariables(testData_Template, "es");

			string es_Hello = "hola mundo";
			string def_Hello = "hello world";

			Assert.IsFalse(result.Contains("${Msg"));
			Assert.IsTrue(result.Contains(es_Hello));

			string current = dc.CalculateVariableValue("${Msg.Hello}");

			Assert.AreEqual(def_Hello, current, "Culture not reset after resolve");
		}


		/// <summary>
		/// Builds a sample data context with test data
		/// </summary>
		/// <returns></returns>
		DataContext BuildTestDataContext(SimpleExpressionResolverDataObject data)
		{
			DataContext dc = new DataContext();

			IResourceBundle resource = new GenericResourceBundle();
			resource.AddString("Hello", "hello world");

			dc.AddResourceBundle(resource);

			resource = new GenericResourceBundle("es");
			resource.AddString("Hello", "hola mundo");

			dc.AddResourceBundle(resource);

			dc.RegisterDataItem("xdata", data);

			return dc;
		}

	}
}

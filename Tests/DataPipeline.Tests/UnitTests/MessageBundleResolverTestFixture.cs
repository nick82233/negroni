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
	public class MessageBundleResolverTestFixture
	{


		const string EXPECTED_Hello = "hello world";
		const string EXPECTED_embedded = "I can say " + EXPECTED_Hello;
		const string EXPECTED_triple = "Some people think they need to say: " + EXPECTED_embedded;


		/// <summary>
		/// Builds a sample data context with test data
		/// </summary>
		/// <returns></returns>
		DataContext BuildTestDataContext(SimpleExpressionResolverDataObject data)
		{
			DataContext dc = new DataContext();

			IResourceBundle resource = new GenericResourceBundle();
			resource.AddString("Hello", "hello world");
			resource.AddString("embedded", "I can say ${Msg.Hello}");
			resource.AddString("triple", "Some people think they need to say: ${Msg.embedded}");

			dc.AddResourceBundle(resource);

			resource = new GenericResourceBundle("es");
			resource.AddString("Hello", "hola mundo");

			dc.AddResourceBundle(resource);

			dc.RegisterDataItem("xdata", data);

			return dc;
		}

		private string testData_Template = @"
My string is ${xdata.MyString}
I say ${Msg.Hello}

${Msg.embedded}

${Msg.triple}
";
		
		private SimpleExpressionResolverDataObject testData_data =
	new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);

		[Test]
		public void ResolveFlatMsgVariable()
		{
			DataContext dc = BuildTestDataContext(testData_data);

			Assert.IsFalse(testData_Template.Contains(EXPECTED_Hello));

			string result = dc.ResolveVariables(testData_Template);

			Assert.IsTrue(result.Contains(EXPECTED_Hello));
		}

		[Test]
		public void ResolveSingleNestedMsgVariable()
		{
			DataContext dc = BuildTestDataContext(testData_data);

			Assert.IsFalse(testData_Template.Contains(EXPECTED_embedded));

			string result = dc.ResolveVariables(testData_Template);

			Assert.IsTrue(result.Contains(EXPECTED_embedded));
		}

		[Test]
		public void ResolveDoubleNestedMsgVariable()
		{
			DataContext dc = BuildTestDataContext(testData_data);

			Assert.IsFalse(testData_Template.Contains(EXPECTED_triple));

			string result = dc.ResolveVariables(testData_Template);

			Assert.IsTrue(result.Contains(EXPECTED_triple));
		}


		[Test]
		public void ResolveCircularPointersDoesntExplode()
		{
			DataContext dc = BuildTestDataContext(testData_data);
			string key1 = "key1";
			string key2 = "key2";

			dc.ResourceStringCatalog.InvariantCultureBundle.AddString(
				key1, "This has ${Msg.key2} circular");

			dc.ResourceStringCatalog.InvariantCultureBundle.AddString(
				key2, "Circle ${Msg.key1} circular");

			string msgvar = "${Msg." + key1 + "}";

			string result = dc.CalculateVariableValue(msgvar);
			//We just need to check that an exception ins not throw.
			// 
			Assert.IsTrue(result.Contains("Circle ${Msg.key1} circular") || result.Contains("${Msg.key2}"));
		}

		[Test]
		public void CircularResolverErrorsLogged()
		{
			DataContext dc = BuildTestDataContext(testData_data);
			string key1 = "key1";
			string key2 = "key2";

			dc.ResourceStringCatalog.InvariantCultureBundle.AddString(
				key1, "This has ${Msg.key2} circular");

			dc.ResourceStringCatalog.InvariantCultureBundle.AddString(
				key2, "Circle ${Msg.key1} circular");

			string msgvar = "${Msg." + key1 + "}";

			string sampleContent = "I am a content with " + msgvar + "and other";

			int beginErrorCount = dc.VariableCalculateErrors.Count;

			string result = dc.ResolveVariables(sampleContent);

			Assert.Greater(dc.VariableCalculateErrors.Count, beginErrorCount, "Error not incremented");
			List<string> errors = dc.VariableCalculateErrors;
			bool foundErrKey = false;
			for (int i = 0; i < errors.Count; i++)
			{
				if (errors[i].Contains(key1) || errors[i].Contains(key2))
				{
					foundErrKey = true;
				}
			}
			Assert.IsTrue(foundErrKey, "Error key not found after resolution");
			
		}


		[Test]
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

		[Test]
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


		[Test]
		public void CultureResetsAfterResolveMessageBundlesOnly()
		{
			DataContext dc = BuildTestDataContext(testData_data);

			string es_Hello = "hola mundo";
			string def_Hello = "hello world";

			Assert.IsFalse(testData_Template.Contains(es_Hello));

			string result = dc.ResolveMessageBundleVariables(testData_Template, "es");

			Assert.IsTrue(result.Contains(es_Hello));

			string current = dc.CalculateVariableValue("${Msg.Hello}");

			Assert.AreEqual(def_Hello, current, "Culture not reset after resolve");
		}



	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Negroni.DataPipeline;
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
	[TestsOn(typeof(WrapperUtility))]
	public class WrapperUtilityTestFixture
	{

		[Test]
		public void DataContractInvokersLoad()
		{
			GenericExpressionEvalWrapper.ClearRegisteredTypes();
			string valExpr = "myString"; 
			DataContext context = new DataContext();

			string testString = "Test String";
			SimpleDataContract data = new SimpleDataContract(1, testString, 2.2);

			GenericExpressionEvalWrapper wrapper = new GenericExpressionEvalWrapper(data, true);

			object rslt = wrapper.ResolveExpressionValue(valExpr);
			Assert.IsNotNull(rslt, "Result is null");
			string result = rslt.ToString();
			Assert.AreEqual(testString, result, "Direct ResolveExpressionValue call failed");
			Assert.AreEqual(result, WrapperUtility.ResolveExpressionValue(valExpr, data, GenericExpressionEvalWrapper.GetDataFieldEvaluator(data.GetType())));

			DataContext dc = new DataContext();
			dc.RegisterDataItem("foo", wrapper);

			string dcResult = dc.CalculateVariableValue("${foo." + valExpr + "}");
			Assert.AreEqual(testString, dcResult, "DataContext gave incorrect result");
		}



		[Test]
		public void PropertyObjectInvokersLoad()
		{
			GenericExpressionEvalWrapper.ClearRegisteredTypes();
			string valExpr = "MyString";
			DataContext context = new DataContext();

			string testString = "Test String";
			SimpleDataObject data = new SimpleDataObject(1, testString, 2.2);

			GenericExpressionEvalWrapper wrapper = new GenericExpressionEvalWrapper(data, false);

			object rslt = wrapper.ResolveExpressionValue(valExpr);
			Assert.IsNotNull(rslt, "Result is null");
			string result = rslt.ToString();
			Assert.AreEqual(testString, result, "Direct ResolveExpressionValue call failed");
			Assert.AreEqual(result, WrapperUtility.ResolveExpressionValue(valExpr, data, GenericExpressionEvalWrapper.GetDataPropertyEvaluator(data.GetType())));

			DataContext dc = new DataContext();
			dc.RegisterDataItem("foo", wrapper);

			string dcResult = dc.CalculateVariableValue("${foo." + valExpr + "}");
			Assert.AreEqual(testString, dcResult, "DataContext gave incorrect result");
		}
	}
}

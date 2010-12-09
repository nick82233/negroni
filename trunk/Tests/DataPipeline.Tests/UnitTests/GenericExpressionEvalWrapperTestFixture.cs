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
	[TestsOn(typeof(GenericExpressionEvalWrapper))]
	public class GenericExpressionEvalWrapperTestFixture
	{

		[Test]
		public void DataContractRecognized()
		{
			GenericExpressionEvalWrapper.ClearRegisteredTypes();
			string testString = "Test String";
			SimpleDataContract data = new SimpleDataContract(1, testString, 2.2);

			GenericExpressionEvalWrapper wrapper = new GenericExpressionEvalWrapper(data);

			int invokerCount = GenericExpressionEvalWrapper.GetDataContractEvaluator(data.GetType()).Count;
			
			Assert.Greater(invokerCount, 0);

			Assert.AreEqual(0, GenericExpressionEvalWrapper.GetDataPropertyEvaluator(data.GetType()).Count, "Property invokers incorrectly added");

		}

		[Test]
		public void PropertyDataObjectRecognized()
		{
			GenericExpressionEvalWrapper.ClearRegisteredTypes();
			string testString = "Test String";
			SimpleDataObject data = new SimpleDataObject(1, testString, 2.2);

			GenericExpressionEvalWrapper wrapper = new GenericExpressionEvalWrapper(data);
			
			int invokerCount = GenericExpressionEvalWrapper.GetDataContractEvaluator(data.GetType()).Count;

			Assert.AreEqual(invokerCount, 0);

			Assert.Less(0, GenericExpressionEvalWrapper.GetDataPropertyEvaluator(data.GetType()).Count, "Property invokers not loaded");

		}


		[Test]
		public void FieldAndPropertyContractWorks()
		{
			GenericExpressionEvalWrapper.ClearRegisteredTypes();
			string testString = "Test String";
			string propNameValue = "Steve";
			string fieldZodiacValue = "Aquarius";
			Person data = new Person();
			data.DisplayName = propNameValue;
			data.ZodiacSign = fieldZodiacValue; //field value
			

			GenericExpressionEvalWrapper wrapper = new GenericExpressionEvalWrapper(data);

			Assert.AreEqual(fieldZodiacValue, wrapper.ResolveExpressionValue("msZodiacSign"), "Field failed evaluation");
			Assert.AreEqual(propNameValue, wrapper.ResolveExpressionValue("displayName"), "Property failed evaluation");

		}


	}
}

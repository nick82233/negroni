using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline.Tests.TestData;
using Negroni.OpenSocial.EL;
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
	public class VariableResolutionTestFixture
	{
		/// <summary>
		/// Formats a variable as a key
		/// </summary>
		/// <param name="variable"></param>
		/// <returns></returns>
		string AsKey(string variable){
			return "${" + variable + "}";
		}

		[Test]
		public void ResolveStringValue()
		{
			string key = "foo";
			string val = "MyValue";
			DataContext dc = new DataContext();

			dc.RegisterDataItem(key, val);

			string result = dc.GetVariableValue(AsKey(key));
			Assert.AreEqual(val, result);
		}

		[Test]
		public void ResolvePersonVariableInDataTag()
		{
			DataContext dc = new DataContext();
			string key = "vwr";
			string expected = AccountTestData.viewerName;

			dc.RegisterDataItem(key, new OsViewerRequest());
			//AccountTestData.ResolveDataControlValues(dc, AccountTestData.Viewer, AccountTestData.Viewer, null);
			AccountTestData.ResolveDataControlValues(dc, AccountTestData.Viewer, AccountTestData.Viewer, null);

			string result = dc.GetVariableValue(AsKey(key + ".displayName"));
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ResolveSimpleDictionaryVariable()
		{
			DataContext dc = new DataContext();
			Dictionary<string, string> dict = new Dictionary<string, string>();

			string expected = "I am a test";
			string dictKey = "foo";
			string key = "mydict";

			dict.Add(dictKey, expected);

			dc.RegisterDataItem(key, dict);

			string expr = AsKey(String.Format("{0}.{1}", new object[]{key, dictKey}));

			string result = dc.GetVariableValue(expr);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ResolveObjectDictionaryVariable()
		{
			DataContext dc = new DataContext();
			Person myViewer = AccountTestData.Viewer;
			string expected = AccountTestData.viewerName;

			Dictionary<string, object> dict = new Dictionary<string, object>();

			string dictKey = "foo";
			string key = "mydict";

			dict.Add(dictKey, new GenericExpressionEvalWrapper(myViewer));

			dc.RegisterDataItem(key, dict);

			string expr = AsKey(String.Format("{0}.{1}.displayName", new object[] { key, dictKey }));

			string result = dc.GetVariableValue(expr);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ResolveNestedDictionarySimpleVariable()
		{
			DataContext dc = new DataContext();
			string expected = "This rocks";

			Dictionary<string, string> dict = new Dictionary<string, string>();

			string dictKey = "foo";
			string key = "mydict";
			string masterKey = "TopO";

			dict.Add(dictKey, expected);
			Hashtable ht = new Hashtable();
			ht.Add(key, dict);
			//alternate way to register
			dc.RegisterLocalValue(masterKey, ht);

			string expr = AsKey(String.Format("{0}.{1}.{2}", new object[] { masterKey, key, dictKey }));

			string result = dc.GetVariableValue(expr);
			Assert.AreEqual(expected, result);
		}


		[Test]
		public void LocalRegistedSimpleDictionaryVariable()
		{
			DataContext dc = new DataContext();

			Dictionary<string, string> dict = new Dictionary<string, string>();

			string localKey = "My";

			string[] rootKeys = { "color1", "color2", "color3" };
			string[] rootVals = { "red", "green", "blue" };

			for (int i = 0; i < rootKeys.Length; i++)
			{
				dc.RegisterDataItem(rootKeys[i], rootVals[i]);
			}

			for (int i = 0; i < rootKeys.Length; i++)
			{
				Assert.AreEqual(rootVals[i], dc.GetVariableValue(rootKeys[i]), "Bad global registration: " + rootKeys[i]);
			}

			string dictInternalKey = "privateColor";
			string expectedValue = rootVals[1];

			dict.Add("test", "something");
			dict.Add(dictInternalKey, AsKey(rootKeys[1]));

			dc.RegisterLocalValue(localKey, dict, true);

			string expr = AsKey(String.Format("{0}.{1}", localKey, dictInternalKey));

			string result = dc.GetVariableValue(expr);
			Assert.AreEqual(expectedValue, result);
		}

		[Test]
		public void LocalRegistedDataKeyDictionaryVariable()
		{
			DataContext dc = new DataContext();
			string expected = AccountTestData.viewerName;

			
			GenericExpressionEvalWrapper wrappedPerson = new GenericExpressionEvalWrapper(AccountTestData.Viewer);

			string val = wrappedPerson.ResolveExpressionValue("displayName") as string;
			Dictionary<string, string> dict = new Dictionary<string, string>();

			string dictKey = "foo";
			string localKey = "mydict";
			string globalKey = "vwr";


			dict.Add(dictKey, AsKey(globalKey));

			dc.RegisterDataItem(globalKey, wrappedPerson);

			dc.RegisterLocalValue(localKey, dict, true);

			string expr = AsKey(String.Format("{0}.{1}.displayName", new object[] { localKey, dictKey }));

			string result = dc.GetVariableValue(expr);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void CalculateInvalidVariable()
		{
			DataContext dc = new DataContext();

			string missingKey = "foo";
			string localKey = "mydict";
			string localVal = "Something";

			dc.RegisterDataItem(localKey, localVal);

			string result = dc.CalculateVariableValue(localKey);
			Assert.AreEqual(localVal, result);

			result = dc.CalculateVariableValue(missingKey);
			Assert.IsNull(result, "Missing key not null");
		}


		[Test]
		public void BadCallRegisteredWithError()
		{
			DataContext dc = new DataContext();

			string missingKey = "foo";
			string localKey = "mydict";
			string localVal = "Something";

			dc.RegisterDataItem(localKey, localVal);

			string result = dc.CalculateVariableValue(localKey);
			Assert.AreEqual(localVal, result);

			result = dc.CalculateVariableValue(missingKey);
			Assert.IsNull(result, "Missing key not null");
			Assert.IsTrue(dc.HasVariableCalculateErrors());
		}

		[Test]
		public void RequestMissingKey()
		{
			DataContext dc = new DataContext();

			string key = "vwr";
			//string expected = AccountTestData.viewerName;

			dc.RegisterDataItem(key, new OsViewerRequest());
			AccountTestData.ResolveDataControlValues(dc, AccountTestData.Viewer, AccountTestData.Viewer, null);

			string expr = "vwrXX.IDontExist";

			string result = dc.CalculateVariableValue(expr);
			Assert.IsNull(result);
			Assert.IsTrue(dc.HasVariableCalculateErrors());
		}



		[Test]
		public void InferredScopeVariableCallsOnAdding()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject globalObject = new SimpleExpressionResolverDataObject(1, "Top Object", 2.2);
			SimpleExpressionResolverDataObject templateMyScopeObject = new SimpleExpressionResolverDataObject(2, "My Template", 3.2);
			SimpleExpressionResolverDataObject curLoopScopeObject = new SimpleExpressionResolverDataObject(3, "Current to loop", 5.2);

			string globalKey = "foo";
			string templateKey = "My";
			string loopKey = "Cur";

			string expression = "${MyString}";
			string globalExpression = "${foo.MyString}";

			SampleSimpleDataControl ctlGlobal = new SampleSimpleDataControl(globalObject);


			dc.RegisterDataItem(globalKey, ctlGlobal);
			

			string result = dc.CalculateVariableValue(globalExpression);
			Assert.AreEqual(globalObject.MyString, result, "Global object wrong");

			dc.RegisterLocalValue(templateKey, templateMyScopeObject);
			result = dc.CalculateVariableValue(expression);
			Assert.IsFalse(string.IsNullOrEmpty(result), "Scope value is null");
			Assert.AreEqual(templateMyScopeObject.MyString, result, "Template scoped item wrong");

			dc.RegisterLocalValue(loopKey, curLoopScopeObject);
			result = dc.CalculateVariableValue(expression);
			Assert.IsFalse(string.IsNullOrEmpty(result), "Scope value is null in loop");
			Assert.AreEqual(curLoopScopeObject.MyString, result, "Loop scoped item wrong");

		}


		[Test]
		public void InferredScopeVariableCallsOnRemoving()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject globalObject = new SimpleExpressionResolverDataObject(1, "Top Object", 2.2);
			SimpleExpressionResolverDataObject templateMyScopeObject = new SimpleExpressionResolverDataObject(2, "My Template", 3.2);
			SimpleExpressionResolverDataObject curLoopScopeObject = new SimpleExpressionResolverDataObject(3, "Current to loop", 5.2);

			string globalKey = "foo";
			string templateKey = "My";
			string loopKey = "Cur";

			string expression = AsKey("MyString");
			string globalExpression = AsKey("foo.MyString");

			SampleSimpleDataControl ctlGlobal = new SampleSimpleDataControl(globalObject);


			dc.RegisterDataItem(globalKey, ctlGlobal);


			string result = dc.CalculateVariableValue(globalExpression);
			Assert.AreEqual(globalObject.MyString, result, "Global object wrong");

			dc.RegisterLocalValue(templateKey, templateMyScopeObject);

			dc.RegisterLocalValue(loopKey, curLoopScopeObject);
			result = dc.CalculateVariableValue(expression);
			Assert.IsFalse(string.IsNullOrEmpty(result), "Scope value is null in loop");
			Assert.AreEqual(curLoopScopeObject.MyString, result, "Loop scoped item wrong");


			//now pop and check
			dc.RemoveLocalValue(loopKey);
			result = dc.CalculateVariableValue(expression);
			Assert.IsFalse(string.IsNullOrEmpty(result), "Scope value is null");
			Assert.AreEqual(templateMyScopeObject.MyString, result, "Template scoped item wrong");
		}



		[Test]
		public void ListDataIndexExpression()
		{
			DataContext dc = new DataContext();
			List<SimpleExpressionResolverDataObject> listData = new List<SimpleExpressionResolverDataObject>();

			SimpleExpressionResolverDataObject expectedData = new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);

			listData.Add(expectedData);

			//SampleSimpleDataControl ctlList = new SampleSimpleDataControl(listData);

			string key = "foo";
			string expression = "${foo[0].MyString}";
			dc.RegisterDataItem(key, listData);

			string result = dc.CalculateVariableValue(expression);
			Assert.IsNotNull(result, "List result is null");
			Assert.IsFalse(string.IsNullOrEmpty(result));
			Assert.AreEqual(expectedData.MyString, result, "Calculated value does not match");

			string result2 = dc.GetVariableValue(expression);
			Assert.AreEqual(result, result2, "Two entry points are not resolving the same");

		}



		[RowTest]
		[Row("2 > 1", true)]
		[Row("2 < 1", false)]
		[Row("1 < 5", true)]
		[Row("2.6 < 2.7", true)]
		public void GreaterThanLessThanLiteralEvaluates(string expression, bool expected)
		{
			DataContext dc = new DataContext();

			string result = dc.CalculateVariableValue(expression);
			bool answer;
			Assert.IsTrue(Boolean.TryParse(result, out answer), "Boolean failed to parse");

			Assert.AreEqual(expected, answer);
		}

		[RowTest]
		[Row("2 + 1 == 3", true)]
		[Row("2+1==3", true)]
		[Row("1 +1==2", true)]
		[Row("1 +1== 3", false)]
		[Row("3 - 1 == 2", true)]
		[Row("3 - 1 == 3", false)]
		[Row("3 - 1 == gobbldygook", false)]
		[Row("4 % 2 == 0", true)]
		[Row("3 % 2 == 1", true)]
		public void EquivalencyLiteralEvaluates(string expression, bool expected)
		{
			DataContext dc = new DataContext();

			string result = dc.CalculateVariableValue(expression);
			bool answer;
			Assert.IsTrue(Boolean.TryParse(result, out answer), "Boolean failed to parse");

			Assert.AreEqual(expected, answer);
		}


		[RowTest]
		[Row(1, 1.0, "${foo.MyInt + 1 == 2}", true)]
		[Row(1, 1.0, "${foo.MyInt + 1 == 3}", false)]
		[Row(1, 1.0, "${foo.MyInt %2 == 1}", true)]
		[Row(3, 1.0, "${foo.MyInt %2 == 1}", true)]
		[Row(4, 1.0, "${foo.MyInt %2 == 0}", true)]
		public void MixedDataLiteralEquivilencyEvaluations(int dataIntVal, double dataDoubleVal, string expression, bool expectedResult)
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject dataObject
				= new SimpleExpressionResolverDataObject(dataIntVal, "Top Object", dataDoubleVal);

			string key = "foo";

			dc.RegisterDataItem(key, dataObject);
			ResolvedExpression result = Engine.ResolveExpression(expression, dc);
			
			Assert.AreEqual(ResolvedExpression.booleanType, result.ResolvedType);
			Assert.AreEqual(expectedResult, result.ResolvedValue, "Expression did not evalute: " + expression);
		}


		/// <summary>
		/// Evaluation test helper method
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="expectedValue"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		bool TestExpressionEvaluation(DataContext dc, int expectedValue, string expression)
		{
			string resultStr = dc.CalculateVariableValue(expression);

			int result;
			Assert.IsTrue(Int32.TryParse(resultStr, out result), "Value failed to parse as integer");
			Assert.AreEqual(expectedValue, result, "Calculated value does not match");
			return true;
		}

		/// <summary>
		/// Evaluation test helper method
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="expectedValue"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		bool TestExpressionEvaluation(DataContext dc, double expectedValue, string expression)
		{
			string resultStr = dc.CalculateVariableValue(expression);

			double result;
			Assert.IsTrue(Double.TryParse(resultStr, out result), "Value failed to parse as integer");
			Assert.AreEqual(expectedValue, result, "Calculated value does not match");
			return true;
		}

		[Test]
		public void ExpressionAddsValue()
		{
			DataContext dc = new DataContext();
			SimpleExpressionResolverDataObject data = new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);
			int expectedValue = data.MyInt + 1;

			string key = "foo";
			string expression = "${foo.MyInt + 1}";
			dc.RegisterDataItem(key, data);

			Assert.IsTrue(TestExpressionEvaluation(dc, expectedValue, expression));
		}

		[Test]
		public void ExpressionSubtractsValue()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject data = new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);
			int expectedValue = data.MyInt - 1;

			string key = "foo";
			string expression = "${foo.MyInt - 1}";
			dc.RegisterDataItem(key, data);

			Assert.IsTrue(TestExpressionEvaluation(dc, expectedValue, expression));

		}

		[Test]
		public void ExpressionMultipliesValue()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject data = new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);
			int expectedValue = data.MyInt * 2;

			string key = "foo";
			string expression = "${foo.MyInt * 2}";
			dc.RegisterDataItem(key, data);

			Assert.IsTrue(TestExpressionEvaluation(dc, expectedValue, expression));
		}
		[Test]
		public void ExpressionDividesValue()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject data = new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);
			int expectedValue = data.MyInt / 2;

			string key = "foo";
			string expression = "${foo.MyInt / 2}";
			dc.RegisterDataItem(key, data);

			Assert.IsTrue(TestExpressionEvaluation(dc, expectedValue, expression));
		}

		[Test]
		public void DuplicateKeyScopeMatchingTest()
		{
			DataContext dc = new DataContext();

			int val1 = 1, val2 = 2;
			double d1 = 1.1, d2 = 2.2;
			string s1 = "Top Object", s2 = "Double Tope";

			SampleSimpleDataControl dataControl1 = new SampleSimpleDataControl(
				new SimpleExpressionResolverDataObject(val1, s1, d1));
			SampleSimpleDataControl dataControl2 = new SampleSimpleDataControl(
				new SimpleExpressionResolverDataObject(val2, s2, d2));

			
			string key = "foo";
			dataControl1.Key = key;
			dataControl2.Key = key;

			dc.RegisterDataItem(dataControl1, "view1");
			dc.RegisterDataItem(dataControl2, "secondview");

			dc.ActiveViewScope = "view1";
			string result;
			string var = "${foo.MyString}";
			result = dc.GetVariableValue(var);
			Assert.IsNotNull(result);
			Assert.AreEqual(s1, result, "view1 scope incorrect value");

			dc.ActiveViewScope = "secondview";
			result = dc.GetVariableValue(var);
			Assert.IsNotNull(result);
			Assert.AreEqual(s2, result, "secondview scope incorrect value");

		}
		[Test]
		public void ExpressionDividesOddValue()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject data = new SimpleExpressionResolverDataObject(13, "I love milk", 12.2);
			double expectedValue = (double)data.MyInt / 2;

			string key = "foo";
			string expression = "${foo.MyInt / 2}";
			dc.RegisterDataItem(key, data);

			Assert.IsTrue(TestExpressionEvaluation(dc, expectedValue, expression));

		}

		[Test]
		public void ExpressionModuloValue()
		{
			DataContext dc = new DataContext();
			SimpleExpressionResolverDataObject data = new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);
			int expectedValue = data.MyInt % 2;

			string key = "foo";
			string expression = "${foo.MyInt % 2}";
			dc.RegisterDataItem(key, data);

			Assert.IsTrue(TestExpressionEvaluation(dc, expectedValue, expression));
		}


		[Test]
		public void GlobalRegisteredSimpleValueResolution()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject globalData =
				new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);

			string globalKey = "foo";
			string localKey = "babar";
			string expGlobalStr = "${foo.MyString}";
			string expGlobalInt = "${foo.MyInt}";
			string expGlobalFloat = "${foo.MyFloat}";


			Assert.IsNull(dc.CalculateVariableValue(expGlobalStr));
			Assert.IsNull(dc.CalculateVariableValue(expGlobalInt));
			Assert.IsNull(dc.CalculateVariableValue(expGlobalFloat));

			dc.RegisterDataItem(globalKey, globalData);

			Assert.IsNotEmpty(dc.CalculateVariableValue(expGlobalStr));
			Assert.IsNotEmpty(dc.CalculateVariableValue(expGlobalInt));
			Assert.IsNotEmpty(dc.CalculateVariableValue(expGlobalFloat));

			Assert.AreEqual(globalData.MyString, dc.CalculateVariableValue(expGlobalStr));
			Assert.AreEqual(globalData.MyInt.ToString(), dc.CalculateVariableValue(expGlobalInt));
			Assert.AreEqual(globalData.MyFloat.ToString(), dc.CalculateVariableValue(expGlobalFloat));
		}


		[Test]
		public void LocalRegisteredSimpleValueResolution()
		{
			DataContext dc = new DataContext();
			SimpleExpressionResolverDataObject localData = new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);

			SimpleExpressionResolverDataObject globalData =
				new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);

			string globalKey = "foo";
			string localKey = "babar";
			string expGlobalStr = "${foo.MyString}";
			//string expGlobalInt = "${foo.MyInt}";
			//string expGlobalFloat = "${foo.MyFloat}";

			string expLocalStr = "${babar.MyString}";
			string expLocalInt = "${babar.MyInt}";
			string expLocalFloat = "${babar.MyFloat}";


			Assert.IsNull(dc.CalculateVariableValue(expGlobalStr));

			dc.RegisterDataItem(globalKey, globalData);

			Assert.IsNotEmpty(dc.CalculateVariableValue(expGlobalStr));

			Assert.IsNull(dc.CalculateVariableValue(expLocalStr));

			dc.RegisterLocalValue(localKey, localData);

			Assert.IsNotEmpty(dc.CalculateVariableValue(expLocalStr));

			Assert.AreEqual(localData.MyString, dc.CalculateVariableValue(expLocalStr));
			Assert.AreEqual(localData.MyInt.ToString(), dc.CalculateVariableValue(expLocalInt));
			Assert.AreEqual(localData.MyFloat.ToString(), dc.CalculateVariableValue(expLocalFloat));

			//and remove
			dc.RemoveLocalValue(localKey);

			Assert.IsNull(dc.CalculateVariableValue(expLocalStr));
		}

		[Test]
		public void LocalRegisteredVariableParameterDictionaryResolution()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject fooData =
				new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);

			SimpleExpressionResolverDataObject barData =
				new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);


			string fooKey = "foo";
			string barKey = "babar";
			string expFooStr = "${foo.MyString}";
			string expFooInt = "${foo.MyInt}";
			string expFooFloat = "${foo.MyFloat}";

			string expBarStr = "${babar.MyString}";
			string expBarInt = "${babar.MyInt}";
			string expBarFloat = "${babar.MyFloat}";

			string dictFooKey = "dfoo";
			string dictBarKey = "dbar";

			string localKey = "loco";

			string expLocalFoo = string.Format("{0}{1}.{2}.##prop##{3}",
				new string[]{
					DataContext.VARIABLE_START,
					localKey, dictFooKey,
					DataContext.VARIABLE_END});

			string expLocalFooString = expLocalFoo.Replace("##prop##", "MyString");
			string expLocalFooInt = expLocalFoo.Replace("##prop##", "MyInt");

			string expLocalBarString = expLocalFooString.Replace(dictFooKey, dictBarKey);
			string expLocalBarInt = expLocalFooInt.Replace(dictFooKey, dictBarKey);


			Assert.IsNull(dc.CalculateVariableValue(expFooStr));
			Assert.IsNull(dc.CalculateVariableValue(expLocalFooString));

			dc.RegisterDataItem(fooKey, fooData);

			Assert.IsNotNull(dc.CalculateVariableValue(expFooStr));
			Assert.IsNotEmpty(dc.CalculateVariableValue(expFooStr));

			dc.RegisterDataItem(barKey, barData);

			Assert.IsNotNull(dc.CalculateVariableValue(expBarStr));
			Assert.IsNotNull(dc.CalculateVariableValue(expBarInt));
			Assert.IsNull(dc.CalculateVariableValue(expLocalFooString));

			//now register the parameters
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			parameters.Add(dictFooKey, "${" + fooKey + "}");
			parameters.Add(dictBarKey, "${" + barKey + "}");

			dc.RegisterLocalValue(localKey, parameters);

			Assert.IsNotEmpty(dc.CalculateVariableValue(expLocalFooString));
			Assert.IsNotEmpty(dc.CalculateVariableValue(expLocalBarString));

			Assert.IsNotNull(dc.CalculateVariableValue(expLocalFooString));
			Assert.IsNotNull(dc.CalculateVariableValue(expLocalBarString));

			Assert.AreEqual(fooData.MyString, dc.CalculateVariableValue(expLocalFooString));
			Assert.AreEqual(fooData.MyInt.ToString(), dc.CalculateVariableValue(expLocalFooInt));

			Assert.AreEqual(barData.MyString, dc.CalculateVariableValue(expLocalBarString));
			Assert.AreEqual(barData.MyInt.ToString(), dc.CalculateVariableValue(expLocalBarInt));

			//and remove
			dc.RemoveLocalValue(localKey);

			Assert.IsNull(dc.CalculateVariableValue(expLocalFooString));
		}

		[Test]
		public void LocalRegisteredSimpleParameterDictionaryResolution()
		{
			DataContext dc = new DataContext();

			string dictFooKey = "dfoo";
			string dictBarKey = "dbar";

			string localKey = "loco";

			string expLocalFoo = string.Format("{0}{1}.{2}{3}",
				new string[]{
					DataContext.VARIABLE_START,
					localKey, dictFooKey,
					DataContext.VARIABLE_END});

			string expLocalBar = expLocalFoo.Replace(dictFooKey, dictBarKey);

			string fooValue = "Blue";
			string barValue = "1";

			//now register the parameters
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			parameters.Add(dictFooKey, fooValue);
			parameters.Add(dictBarKey, barValue);


			Assert.IsNull(dc.CalculateVariableValue(expLocalFoo));
			Assert.IsNull(dc.CalculateVariableValue(expLocalBar));

			dc.RegisterLocalValue(localKey, parameters);

			Assert.IsNotNull(dc.CalculateVariableValue(expLocalFoo));
			Assert.IsNotNull(dc.CalculateVariableValue(expLocalBar));

			Assert.AreEqual(fooValue, dc.CalculateVariableValue(expLocalFoo));
			Assert.AreEqual(barValue, dc.CalculateVariableValue(expLocalBar));

		}


		[Test]
		public void CompareTwoVariableValues()
		{
			DataContext dc = new DataContext();

			int expectedInt = 9;

			SimpleExpressionResolverDataObject fooData =
				new SimpleExpressionResolverDataObject(expectedInt, "I am a beast", 2.2);

			SimpleExpressionResolverDataObject barData =
				new SimpleExpressionResolverDataObject(expectedInt, "I love milk", 12.2);


			string fooKey = "foo";
			string barKey = "babar";
			string expFooInt = "${foo.MyInt}";

			string expBarInt = "${babar.MyInt}";

			string comparisonExpression = "${foo.MyInt == babar.MyInt}";

			Assert.IsNull(dc.CalculateVariableValue(expFooInt));
			Assert.IsNull(dc.CalculateVariableValue(expBarInt));

			dc.RegisterDataItem(fooKey, fooData);

			Assert.IsNotNull(dc.CalculateVariableValue(expFooInt));

			dc.RegisterDataItem(barKey, barData);

			Assert.IsNotNull(dc.CalculateVariableValue(expBarInt));


			Assert.AreEqual(fooData.MyInt.ToString(), dc.CalculateVariableValue(expFooInt));
			Assert.AreEqual(expectedInt.ToString(), dc.CalculateVariableValue(expFooInt));

			Assert.AreEqual(barData.MyInt.ToString(), dc.CalculateVariableValue(expBarInt));
			Assert.AreEqual(expectedInt.ToString(), dc.CalculateVariableValue(expBarInt));

			string bCalculated = dc.CalculateVariableValue(comparisonExpression);

			bool result;
			Assert.IsTrue(Boolean.TryParse(bCalculated, out result));
			Assert.IsTrue(result, "Parsed boolean return value was not true");


		}



		[Test]
		public void PrivateParameterListDeepReferenceResolves()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject fooData =
				new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);

			SimpleExpressionResolverDataObject barData =
				new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);


			List<SimpleExpressionResolverDataObject> items = new List<SimpleExpressionResolverDataObject>();
			items.Add(fooData);
			items.Add(barData);

			string expression = "${My.list[0].MyString}";
			string expressionInt = "${My.list[0].MyInt}";
			string expressionInferred = "${list[0].MyString}";


			Assert.IsNull(dc.CalculateVariableValue(expression));
			Assert.IsNull(dc.CalculateVariableValue(expressionInt));
			Assert.IsNull(dc.CalculateVariableValue(expressionInferred));
			Assert.IsNull(dc.CalculateVariableValue("${My}"));

			string privateKey = "My";

			Dictionary<string, object> paramDict = new Dictionary<string, object>();
			paramDict.Add("list", items);

			dc.RegisterLocalValue(privateKey, paramDict);

			Assert.IsNotNull(dc.CalculateVariableValue("${My}"));
			Assert.IsNotNull(dc.CalculateVariableValue(expression), "Deep expression is null: " + expression);
			Assert.IsNotEmpty(dc.CalculateVariableValue(expression));
			Assert.IsNotNull(dc.CalculateVariableValue(expressionInferred), "Inferred variable failed to resolve: " + expressionInferred);

			Assert.AreEqual(fooData.MyString, dc.CalculateVariableValue(expression));
			Assert.AreEqual(fooData.MyString, dc.CalculateVariableValue(expressionInferred));
			Assert.AreEqual(fooData.MyInt.ToString(), dc.CalculateVariableValue(expressionInt));

			//and remove
			dc.RemoveLocalValue(privateKey);

			Assert.IsNull(dc.CalculateVariableValue(expression));
			Assert.IsNull(dc.CalculateVariableValue(expressionInferred));
		}



		[Test]
		public void RecursiveVariableResolutionFails()
		{
			DataContext dc = new DataContext();


			string fooKey = "foo";
			string expFooRef = "${foo}";


			Assert.IsNull(dc.CalculateVariableValue(expFooRef));

			dc.RegisterDataItem(fooKey, expFooRef);

			Assert.IsNull(dc.CalculateVariableValue(expFooRef));

		}


		[Test]
		public void RecursiveLocalVariableResolutionFails()
		{
			DataContext dc = new DataContext();


			string dictFooKey = "dfoo";
			string dictBarKey = "dbar";

			string localKey = "loco";

			string expLocalFoo = string.Format("{0}{1}.{2}{3}",
				new string[]{
					DataContext.VARIABLE_START,
					localKey, dictFooKey,
					DataContext.VARIABLE_END});

			string expLocalBar = expLocalFoo.Replace(dictFooKey, dictBarKey);

			string fooValue = expLocalFoo;
			string barValue = "1";

			//now register the parameters
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			parameters.Add(dictFooKey, fooValue);
			parameters.Add(dictBarKey, barValue);

			string evaluationResponse = dc.CalculateVariableValue(expLocalFoo);
			Assert.IsNull(evaluationResponse);
			evaluationResponse = dc.CalculateVariableValue(expLocalBar);
			Assert.IsNull(evaluationResponse);

			dc.RegisterLocalValue(localKey, parameters);

			evaluationResponse = dc.CalculateVariableValue(expLocalFoo);
			Assert.AreEqual(expLocalFoo, evaluationResponse);
			evaluationResponse = dc.CalculateVariableValue(expLocalBar);
			Assert.IsNotNull(evaluationResponse);

			evaluationResponse = dc.CalculateVariableValue(expLocalBar);
			Assert.AreEqual(barValue, evaluationResponse);

		}


		[Test]
		public void ReferencedVariableResolution()
		{
			DataContext dc = new DataContext();


			string fooKey = "foo";
			string expFooRef = "${foo}";

			string barKey = "bar";
			string barValue = "I am bar";
			string expBarRef = "${bar}";

			Assert.IsNull(dc.CalculateVariableValue(expFooRef));
			Assert.IsNull(dc.CalculateVariableValue(expBarRef));

			dc.RegisterDataItem(fooKey, expBarRef);
			dc.RegisterDataItem(barKey, barValue);

			Assert.IsNotNull(dc.CalculateVariableValue(expFooRef));
			Assert.IsNotNull(dc.CalculateVariableValue(expBarRef));

			Assert.AreEqual(barValue, dc.CalculateVariableValue(expBarRef), "Bar direct resolves incorrectly");
			Assert.AreEqual(barValue, dc.CalculateVariableValue(expFooRef), "Foo referencing bar fails to resolve");


		}

		[Test]
		public void LocalRegisteredListResolution()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject fooData =
				new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);

			SimpleExpressionResolverDataObject barData =
				new SimpleExpressionResolverDataObject(12, "I love milk", 12.2);


			string fooKey = "foo";
			string barKey = "babar";
			string expFooStr = "${foo.MyString}";
			string expFooInt = "${foo.MyInt}";
			string expFooFloat = "${foo.MyFloat}";

			string expBarStr = "${babar.MyString}";
			string expBarInt = "${babar.MyInt}";
			string expBarFloat = "${babar.MyFloat}";

			string localKey = "loco";

			string expressionBase = AsKey(localKey + "[##index##].##prop##");
			string expLocalFoo = expressionBase.Replace("##index##", "0");
			string expLocalBar = expressionBase.Replace("##index##", "1");

			string expLocalFooString = expLocalFoo.Replace("##prop##", "MyString");
			string expLocalFooInt = expLocalFoo.Replace("##prop##", "MyInt");

			string expLocalBarString = expLocalBar.Replace("##prop##", "MyString");
			string expLocalBarInt = expLocalBar.Replace("##prop##", "MyInt");

			Assert.IsNull(dc.CalculateVariableValue(expFooStr));
			Assert.IsNull(dc.CalculateVariableValue(expLocalFooString));

			dc.RegisterDataItem(fooKey, fooData);

			Assert.IsNotNull(dc.CalculateVariableValue(expFooStr));
			Assert.IsNotEmpty(dc.CalculateVariableValue(expFooStr));

			dc.RegisterDataItem(barKey, barData);

			Assert.IsNotNull(dc.CalculateVariableValue(expBarStr));
			Assert.IsNotNull(dc.CalculateVariableValue(expBarInt));
			Assert.IsNull(dc.CalculateVariableValue(expLocalFooString));

			//now register the parameters
			ArrayList localList = new ArrayList();

			localList.Add(fooData);
			localList.Add(barData);

			dc.RegisterLocalValue(localKey, localList);

			Assert.IsNotEmpty(dc.CalculateVariableValue(expLocalFooString));
			Assert.IsNotEmpty(dc.CalculateVariableValue(expLocalBarString));

			Assert.IsNotNull(dc.CalculateVariableValue(expLocalFooString));
			Assert.IsNotNull(dc.CalculateVariableValue(expLocalBarString));

			Assert.AreEqual(fooData.MyString, dc.CalculateVariableValue(expLocalFooString));
			Assert.AreEqual(fooData.MyInt.ToString(), dc.CalculateVariableValue(expLocalFooInt));

			Assert.AreEqual(barData.MyString, dc.CalculateVariableValue(expLocalBarString));
			Assert.AreEqual(barData.MyInt.ToString(), dc.CalculateVariableValue(expLocalBarInt));

			//and remove
			dc.RemoveLocalValue(localKey);

			Assert.IsNull(dc.CalculateVariableValue(expLocalFooString));
		}


		[Test]
		public void ListWithSelectorsResolution()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject fooData =
				new SimpleExpressionResolverDataObject(9, "bob", 2.2);

			SimpleExpressionResolverDataObject barData =
				new SimpleExpressionResolverDataObject(12, "steve", 12.2);

			SimpleExpressionResolverDataObject zooData =
				new SimpleExpressionResolverDataObject(10, "kim", 2.2);

			List<IExpressionEvaluator> dataList = new List<IExpressionEvaluator>();
			dataList.Add(fooData);
			dataList.Add(barData);
			dataList.Add(zooData);


			string listKey = "fooList";
			string expIndexFooStr = "${fooList[0].MyString}";
			string expSelectorBarInt = "${fooList[@MyString='steve'].MyInt}";
			string expSelectorBarStr = "${fooList[@MyString='kim'].MyString}";


			Assert.IsNull(dc.CalculateVariableValue(listKey));
			Assert.IsNull(dc.CalculateVariableValue(expIndexFooStr));
			Assert.IsNull(dc.CalculateVariableValue(expSelectorBarStr));

			dc.RegisterDataItem(listKey, dataList);

			Assert.IsNotNull(dc.CalculateVariableObjectValue(listKey));

			string result = dc.CalculateVariableValue(expIndexFooStr);
			Assert.AreEqual("bob", result, "Index get from list failed");

			result = dc.CalculateVariableValue(expSelectorBarInt);
			Assert.AreEqual("12", result, "Selector get from list failed");
			result = dc.CalculateVariableValue(expSelectorBarStr);
			Assert.AreEqual("kim", result, "Selector get from list failed");

		}



		[Test]
		public void NestedExpresionEvaluatorObjects()
		{
			DataContext dc = new DataContext();

			SimpleExpressionResolverDataObject innerData =
				new SimpleExpressionResolverDataObject(10, "I am a beast", 2.2);

			ComplexExpressionResolverDataObject outerObject =
				new ComplexExpressionResolverDataObject("Hello Outer", innerData);

			
			string key = "xdata";
			string expOuterStr = AsKey(key + ".MyString");
			string expNestedStr = AsKey(key + ".nested.MyString");
			string expNestedObject = AsKey(key + ".nested");

			Assert.IsNull(dc.CalculateVariableValue(expOuterStr));
			Assert.IsNull(dc.CalculateVariableValue(expNestedStr));

			dc.RegisterDataItem(key, outerObject);

			Assert.IsNotNull(dc.CalculateVariableValue(expOuterStr));
			Assert.IsNotNull(dc.CalculateVariableValue(expNestedStr));

			Assert.AreEqual(outerObject.MyString, dc.CalculateVariableValue(expOuterStr));
			Assert.AreEqual(innerData.MyString, dc.CalculateVariableValue(expNestedStr));

			Assert.IsNull(dc.CalculateVariableValue(expNestedObject), "Object reference is incorrectly not null for string eval");
	
		}


		class AnObject
		{
			public string PropVal
			{
				get;
				set;
			}

			public string FieldVal = null;
		}

		[Test]
		public void AutoWrappedObjectTests()
		{
			DataContext dc = new DataContext();

			AnObject unwrapped = new AnObject
			{
				PropVal = "fooProp",
				FieldVal = "fooField"
			};

			GenericExpressionEvalWrapper wrapped = new GenericExpressionEvalWrapper(unwrapped);

			dc.RegisterDataItem("wrapped", wrapped);
			dc.RegisterDataItem("unwrapped", unwrapped);

			Assert.IsFalse(string.IsNullOrEmpty(dc.CalculateVariableValue("${wrapped.PropVal}")));
			Assert.IsFalse(string.IsNullOrEmpty(dc.CalculateVariableValue("${wrapped.FieldVal}")));

			Assert.AreEqual(unwrapped.PropVal, dc.CalculateVariableValue("${wrapped.PropVal}"));
			Assert.AreEqual(unwrapped.PropVal, dc.CalculateVariableValue("${unwrapped.PropVal}"));
			string val = dc.CalculateVariableValue("${unwrapped.PropVal}");
			Assert.AreEqual(unwrapped.FieldVal, dc.CalculateVariableValue("${wrapped.FieldVal}"));
			Assert.AreEqual(unwrapped.FieldVal, dc.CalculateVariableValue("${unwrapped.FieldVal}"));

		}
	
	}
}

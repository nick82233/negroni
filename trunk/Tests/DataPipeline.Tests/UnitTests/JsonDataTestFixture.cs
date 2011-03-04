using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline.Tests.TestData;
using Negroni.DataPipeline.Serialization;

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
#if XUNIT
#else
	[TestFixture]
#endif
	[TestsOn(typeof(JsonData))]
	public class JsonDataTestFixture
	{

		const string JSON_COMPLEX_NESTED_1 = @"{'one': [
	{'id':'m1', 'name': 'Joe'},
	{'id':'m2', 'name': 'Kim', 'kids':
		[{'name':'jimmy', 'age': '2'}]
	}
	]}";



		const string JSON_OBJECT_ARRAY_1 = 
@"[
	{
		color: ""red"",
		value: ""#f00""
	},
	{
		color: ""green"",
		value: ""#0f0""
	},
	{
		color: ""blue"",
		value: ""#00f""
	},
	{
		color: ""cyan"",
		value: ""#0ff""
	},
	{
		color: ""magenta"",
		value: ""#f0f""
	},
	{
		color: ""yellow"",
		value: ""#ff0""
	},
	{
		color: ""black"",
		value: ""#000""
	}
]";



		[Test]
		public void LoadObjectFoundInRaw()
		{
			string json = "{'one': 1, 'two': 2}";

			JsonData target = new JsonData(json);

			Assert.IsNotNull(target.RawJSON);
		}

		[Test]
		public void RecognizeObjectHasData()
		{
			string json = "{'one': 1, 'two': 2}";

			JsonData target = new JsonData(json);
			Assert.IsTrue(target.IsObjectHash, "Not marked as object hash");
			Assert.IsFalse(target.IsArray, "Incorrectly marked as an array");
		}

		[Test]
		public void RecognizeArrayData()
		{
			string json = "['one', 'two']";

			JsonData target = new JsonData(json);
			Assert.IsTrue(target.IsArray, "Not marked as array");
			Assert.IsFalse(target.IsObjectHash, "Incorrectly marked as an object hash");
		}

		[Test]
		public void ResetTypeRecognize()
		{
			string json = "['one', 'two']";

			JsonData target = new JsonData(json);
			Assert.IsTrue(target.IsArray, "Not marked as array");

			json = "{'one': 1, 'two': 2}";
			target.LoadJsonString(json);
			Assert.IsFalse(target.IsArray, "Reload didn't reset type");
			Assert.IsTrue(target.IsObjectHash);
		}

		#region Malformed JSON tests
#if XUNIT
#else

		[ExpectedException(typeof(ArgumentException))]
		[RowTest]
		[Row("['half an array'")]
		[Row("'back array']")]
		[Row("'back object'}")]
		[Row("{'half an object'")]
		public void MalformedJSONThrows(string json)
		{
			JsonData target = new JsonData(json);
			Assert.Fail("Failed to throw on bad JSON: " + json);
		}
#endif

		#endregion

		[Test]
		public void ArrayLengthCorrect()
		{
			string json = "['one', 'two']";

			JsonData target = new JsonData(json);
			Assert.AreEqual(2, target.Length, "Array Length incorrect");
		}
		[Test]
		public void ObjectLengthCorrect()
		{
			string json = "{'one': 1, 'two': 2}";

			JsonData target = new JsonData(json);
			Assert.AreEqual(2, target.Length, "Object Length incorrect");
		}

		[RowTest]
		[Row("{'one': \"1\", 'two': 2}", "one", "1")]
		[Row("{'one': 1, 'two': 2}", "one", 1)]
		[Row("{'one': 1, 'two': 2}", "two", 2)]
		[Row("{'one': 1, 'two': 2, 'three': '3', 'four': '4'}", "two", 2)]
		[Row("{'one': 1, 'two': 2, 'three': '3', 'four': '4'}", "three", 3)]
		public void SimpleObjectFind(string json, string key, object expected)
		{
			JsonData target = new JsonData(json);

			object result = target.ResolveExpressionValue(key);
			if (expected == null || result == null)
			{
				Assert.AreEqual(expected, result);
			}
			else
			{
				Assert.AreEqual(expected.ToString(), result.ToString());
			}
		}

		[RowTest]
		[Row("['one',\"2\"]", "0", "one")]
		public void SimpleListFind(string json, string key, object expected)
		{
			JsonData target = new JsonData(json);

			object result = target.ResolveExpressionValue(key);
			Assert.AreEqual(expected, result);
		}


		[Test]
		public void StringValueContainingCommas()
		{
			string val = "one, two, buckle";
			string key = "one";
			string json = "{\"" + key + "\":\"" + val + "\"}";
			JsonData target = new JsonData(json);

			object result = target.ResolveExpressionValue(key);
			if (val == null || result == null)
			{
				Assert.AreEqual(val, result);
			}
			else
			{
				Assert.AreEqual(val.ToString(), result.ToString());
			}
		}

		[RowTest]
		[Row("{'one': \"two\"}", "one", "two")]
		[Row("{'o\"ne': \"two\"}", "o\"ne", "two")]
		[Row("{\"o'ne\": \"two\"}", "o'ne", "two")]
		[Row("{'one': \"tw'o\"}", "one", "tw'o")]
		public void EmbeddedQuoteTests(string json, string key, string expected)
		{
			JsonData target = new JsonData(json);

			object result = target.ResolveExpressionValue(key);
			Assert.AreEqual(expected, result);
		}

		[RowTest]
		[Row("{'one': \"two\"}", "one", "two")]
		[Row("{'o,ne': \"two\"}", "o,ne", "two")]
		[Row("{'o,ne': \"two\", 'three': 3}", "three", 3)]
		[Row("{'o,ne': \"two\", 'three': 3, 'fou,r': '4'}", "fou,r", 4)]
		[Row("['o,ne', \"two\", 'three', 3, 'fou,r', '4']", "2", "three")]
		public void EmbeddedCommaTests(string json, string key, object expected)
		{
			JsonData target = new JsonData(json);

			object result = target.ResolveExpressionValue(key);
			Assert.AreEqual(expected, result);
		}

		[RowTest]
		[Row("{'one': {'one': \"two\"}}", "one.one", "two")]
		[Row("{'one': {'one': \"two\"}, 'two': {'odd':'ball'}}", "one.one", "two")]
		[Row("{'one': {'two': {'two': \"two\"}}}", "one.two.two", "two")]
		[Row("{'x': {'two': [1, 2,3,'four']}}", "x.two[0]", 1)]
		[Row("{'x': {'two': [1, 2,3,{'boo':'yah'}]}}", "x.two[3].boo", "yah")]
		[Row(@"{'one': [{'id':'m1', 'name': 'Joe'},
{'id':'m2', 'name': 'Kim'}]}", "one[0].name", "Joe")]
		[Row(@"{'one': [{'id':'m1', 'name': 'Joe'},
{'id':'m2', 'name': 'Kim'}]}", "one[0].id", "m1")]
		[Row(@"{'one': [{'id':'m1', 'name': 'Joe'},
{'id':'m2', 'name': 'Kim'}]}", "one[1].name", "Kim")]
		public void ComplexObjectTests(string json, string key, object expected)
		{
			JsonData target = new JsonData(json);

			object result = target.ResolveExpressionValue(key);
			Assert.AreEqual(expected, result);
		}




		[RowTest]
		[Row(@"{'one': [{'id':'m1', 'name': 'Joe'},
{'id':'m2', 'name': 'Kim'}]}", "one[@id='m1'].name", "Joe")]
		[Row(JSON_COMPLEX_NESTED_1, "one[@id='m2'].kids[0].name", "jimmy")]
		[Row(JSON_COMPLEX_NESTED_1, "one[@id='m2'].kids[@name='jimmy'].age", 2)]
		public void AttributeSelectTests(string json, string expr, object expected)
		{
			JsonData target = new JsonData(json);

			object result = target.ResolveExpressionValue(expr);
			Assert.AreEqual(expected, result);
		}

		[RowTest]
		[Row(@"{'one': [{'id':'m1', 'name': 'Joe'},
{'id':'m2', 'name': 'Kim'}]}", "one[@id='m1'].name", "Joe")]
		[Row(JSON_COMPLEX_NESTED_1, "one[0].name", "Joe")]
		[Row(JSON_COMPLEX_NESTED_1, "one[@id='m2'].kids[0].name", "jimmy")]
		[Row(JSON_COMPLEX_NESTED_1, "one[@id='m2'].kids[@name='jimmy'].age", "2")]
		public void AttributeSelectWithELTests(string json, string expr, object expected)
		{
			JsonData jsObj = new JsonData(json);
			DataContext dc = new DataContext();
			string key = "foo";
			string el = "${" + key + "." + expr + "}";

			dc.RegisterDataItem(key, jsObj);
			Assert.IsNotNull(dc.CalculateVariableObjectValue(key), "Object not registered");

			string result = dc.CalculateVariableValue(el);
			Assert.AreEqual(expected, result, "EL statement failed");
		}


		[Test]
		public void LongJsonDataTest()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 100; i++)
			{
				sb.Append("hello").Append(i);
			}
			string expected = sb.ToString();
			string json = "{'message': \"" + expected + "\"}";

			JsonData target = new JsonData(json);
			Assert.AreEqual(target.ResolveExpressionValue("message"), expected);

		}

		[Test]
		public void LongJsonArrayTest()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 100; i++)
			{
				sb.Append("hello").Append(i);
			}
			string expected = sb.ToString();
			string json = "['message', \"" + expected + "\"]";

			JsonData target = new JsonData(json);
			Assert.AreEqual(target.ResolveExpressionValue("[1]"), expected);

		}

		[RowTest]
		[Row("{\n\tcolor: 'red',\n\tvalue: '#f00'\n}", "color", "red")]
		[Row("{color: 'red',\n\tvalue: '#f00'\n}", "color", "red")]
		[Row("{color: 'red',\n\tvalue: '#f00'\n}\n\t", "color", "red")]
		public void JsonObjectPrettyFormatted(string json, string key, string expectedValue)
		{
			JsonData target = new JsonData(json);
			string val = target.ResolveExpressionValue(key) as string;
			Assert.AreEqual(expectedValue, val);
		}


		[RowTest]
		[Row("{color: 'red', value: '#f00'}", "color", "red")]
		public void UnquotedObjectKeyTest(string json, string key, string expectedValue)
		{
			JsonData target = new JsonData(json);
			string val = target.ResolveExpressionValue(key) as string;
			Assert.AreEqual(expectedValue, val);
		}

		[RowTest]
		[Row("['color', 'red', \"number\", '#f00']", "[0]", "color")]
		[Row("['color', 'red', \"number\", '#f00']", "[3]", "#f00")]
		[Row("['color', 'red', \"number\", '#f00']", "[4]", null)]
		[Row("[100,200,300]", "[0]", "100")]
		[Row("[100, 200, 300]", "[1]", "200")]
		[Row("[100,	200,	300]", "[1]", "200")]
		public void SimpleArrayLoad(string json, string key, string expectedValue)
		{
			JsonData target = new JsonData(json);
			string val = null;
			object x = target.ResolveExpressionValue(key);
			if (x != null)
			{
				val = x.ToString();
			}
			Assert.AreEqual(expectedValue, val);
		}


		[RowTest]
		[Row(@"['he', 'she\'s']", "[1]", "she's")]
		[Row(@"{'red': ""blue \""bear\""""}", "red", "blue \"bear\"")]
		[Row(@"['h\""e', 'she\'s']", "[0]", "h\"e")]
		[Row(@"{'r\'ed': ""blue \""bear\""""}", "r'ed", "blue \"bear\"")]
		[Row(@"{'red': {'one': ""blue \""bear\""""}}", "red.one", "blue \"bear\"")]
		public void EscapedStringsDontExplode(string json, string key, string expectedValue)
		{
			JsonData target = new JsonData(json);
			string val = null;
			object x = target.ResolveExpressionValue(key);
			if (x != null)
			{
				val = x.ToString();
			}
			Assert.AreEqual(expectedValue, val);
		}




		[Test]
		public void JsonObjectArrayFormatted()
		{

			JsonData target = new JsonData(JSON_OBJECT_ARRAY_1);
			Assert.AreEqual(7, target.Length, "Not proper array length");

			Assert.IsTrue(target.IsArray);
			string color = target.ResolveExpressionValue("[0].color") as string;
			Assert.AreEqual("red", color);
			string hex = target.ResolveExpressionValue("[0].value") as string;
			Assert.AreEqual("#f00", hex);
		}
		

	}
}

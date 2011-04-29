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
	public class JsonDataEscapingTestFixture
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


		[RowTest]
		[Row(@"{'key': 'line 1\nline2'}", "key", "line 1\nline2")]
		[Row(@"['key', 'line 1\nline2']", "[1]", "line 1\nline2")]
		[Row(@"{'key': 'return\rline2'}", "key", "return\rline2")]
		public void EscapedNewlinesTest(string json, string key, string expectedValue)
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
		[Row(@"{'key': 'http:\/\/example.org\/somewhere'}", "key", "http://example.org/somewhere")]
		public void EscapedSlashesTest(string json, string key, string expectedValue)
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
		[Row(@"{'key': 'item1\titem2'}", "key", "item1	item2")]
		[Row(@"['item1\titem2']", "[0]", "item1\titem2")]
		public void EscapedTabsTest(string json, string key, string expectedValue)
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
		[Row(@"{'key': 'line1 \
crosses 2'}", "key", "line1 crosses 2")]
		[Row(@"['line1 \
crosses 2']", "[0]", "line1 crosses 2")]
		public void EscapeRealLineBreaks(string json, string key, string expectedValue)
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



	}
}

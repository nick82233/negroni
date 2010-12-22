using System;
using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.Test.Controls;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Test.OSML
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OsmlControl"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(VariableTag))]
	public class VariableTagTests
    {

		[RowTest]
		[Row("<os:Var key='foo' value='foo' />", "foo")]
		[Row("<os:Var key='foo' >foo</os:Var>", "foo")]
		[Row("<os:Var key='foo' >9</os:Var>", 9)]
		public void SimpleVariableValues(string tag, object expectedValue)
		{
			OsVar target = new OsVar(tag);
			Assert.AreEqual(expectedValue, target.GetVariableValue());
		}


		[Test]
		public void JSonDataValues()
		{
			string tag = "<os:Var key='foo' >{'displayName':'John'}</os:Var>";
			OsVar target = new OsVar(tag);
			JsonData json = target.GetVariableValue() as JsonData;
			Assert.IsNotNull(json, "Didn't parse to JSON");
			Assert.AreEqual("John", json.ResolveExpressionValue("displayName"));
		}

		[Test]
		public void JSonDataValuesWithSpace()
		{
			string tag = "\n<os:Var key='foo' >\n\n{'displayName':'John'}\n\n</os:Var>";
			OsVar target = new OsVar(tag);
			JsonData json = target.GetVariableValue() as JsonData;
			Assert.IsNotNull(json, "Didn't parse to JSON");
			Assert.AreEqual("John", json.ResolveExpressionValue("displayName"));
		}

	}
}
	
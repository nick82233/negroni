using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

namespace Negroni.OpenSocial.Tests.OSML
{
	[TestFixture]
	[TestsOn(typeof(BaseGadgetControl))]
	public class OsmlControlAttributeTests : OsmlControlTestBase
	{
		const string simpleName = "<os:Name person=\"${Viewer}\" />";
		const string simpleNameNoAttrs = "<os:Name  />";
		const string nameAttrWithSpaceInValueDblQuote = "<os:Name value=\"foo and bar\" />";
		const string nameAttrWithSpaceInValueSingleQuote = "<os:Name value='foo and bar' />";

		const string nameFreeformAttrs = "<os:Name person=\"${Viewer}\" key=\"foo\" value=\"bar\" />";
		const string nameSingleQuoteAttrs = "<os:Name userid='12' person='${Viewer}' key='foo' value='bar' />";
		const string nameDoubleQuoteAttrsSpaced = @"<os:Name userid = ""12"" person = ""${Viewer}"" key= ""foo"" value =""bar"" />";
		const string nameSingleQuoteAttrsSpaced = "<os:Name userid = '12' person='${Viewer}' key = 'foo' value    ='bar' />";
		const string nameCamelCaseAttributes = @"<os:Name userId=""12"" person='${Viewer}' kEy='foo' VALUE='bar' />";
		const string nameUpperCaseAttributes = @"<os:Name USERID=""12"" PERSON='${Viewer}' KEY='foo' VALUE='bar' />";
		const string nameAttrsTightLastSpace = @"<os:Name userid = ""12"" person = ""${Viewer}"" key= ""foo"" value =""bar""/>";
		const string nameAttrsDoubleSpace = @"<os:Name userid =  ""12"" person = ""${Viewer}"" key= ""foo"" value =""bar""/>";
		const string nameNewlineInTag = @"<os:Name 
userid = ""12"" 
person=""${Viewer}"" key= ""foo"" value =""bar""
/>";

		const string nameNewlineAndTabsInTag = @"<os:Name 
	userid = ""12"" 
person=""${Viewer}""	key= ""foo"" 
value	=	""bar""
/>";
		const string nameNewlineBetweenAttrValue = @"<os:Name 
userid =
""12"" 
person
=""${Viewer}"" key= ""foo"" value =    ""bar""
/>";


		[Test]
		public void TestNoAttributes()
		{
			OsmlName target = new OsmlName(simpleNameNoAttrs);
			Assert.IsFalse(target.HasAttributes());
		}
		[Test]
		public void TestMissingAttribute()
		{
			OsmlName target = new OsmlName(simpleNameNoAttrs);
			Assert.AreEqual(string.Empty, target.GetAttribute("xx"));
		}

		[Test]
		public void TestAttributeWithSpaceInValueDblQuote()
		{
			OsmlName target = new OsmlName(nameAttrWithSpaceInValueDblQuote);
			Assert.AreEqual("foo and bar", target.GetAttribute("value"));
		}

		[Test]
		public void TestAttributeWithSpaceInValueSingleQuote()
		{
			OsmlName target = new OsmlName(nameAttrWithSpaceInValueSingleQuote);
			Assert.AreEqual("foo and bar", target.GetAttribute("value"));
		}

		[Test]
		public void TestDoubleQuoteAttributes()
		{
			OsmlName target = new OsmlName(simpleName);
			Assert.IsTrue(target.HasAttributes());
			Assert.AreEqual("${Viewer}", target.GetAttribute("person"));
		}


		[Test]
		public void EqualInQuoteValue()
		{
			string value = "Cat!=Dog";
			string markup = "<os:Name userid ='12' person =\"${Viewer}\" key='foo' otherval=\"" + value + "\" />";

			OsmlName target = new OsmlName(markup);
			Assert.IsTrue(target.HasAttributes());
			Assert.AreEqual(value, target.GetAttribute("otherval"));
		}


		[RowTest]
		[Row(nameSingleQuoteAttrs)]
		[Row(nameDoubleQuoteAttrsSpaced)]
		[Row(nameSingleQuoteAttrsSpaced)]
		[Row(nameCamelCaseAttributes)]
		[Row(nameUpperCaseAttributes)]
		[Row(nameAttrsTightLastSpace)]
		[Row(nameAttrsDoubleSpace)]
		[Row(nameNewlineInTag)]
		[Row(nameNewlineAndTabsInTag)]
		[Row(nameNewlineBetweenAttrValue)]
		public void TestAttrQuoteVarients(string markup)
		{
			OsmlName target = new OsmlName(markup);
			Assert.IsTrue(target.HasAttributes());
			Assert.AreEqual("12", target.GetAttribute("userid"));
			Assert.AreEqual("12", target.GetAttribute("userId"));
			Assert.AreEqual("12", target.GetAttribute("USERID"));
			Assert.AreEqual("${Viewer}", target.GetAttribute("person"));
			Assert.AreEqual("foo", target.GetAttribute("key"));
			Assert.AreEqual("bar", target.GetAttribute("value"));
		}
	}
}

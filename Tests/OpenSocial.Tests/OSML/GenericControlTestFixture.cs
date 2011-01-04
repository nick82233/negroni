using System;
using System.IO;

using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Gadget.Controls;

namespace Negroni.OpenSocial.Tests.OSML
{
	[TestFixture]
	[TestsOn(typeof(BaseGadgetControl))]
	public class GenericControlTestFixture
	{

		[RowTest]
		[Row(new object[]{"foo", "manchu"})]
		[Row(new object[]{"Big", "small"})]
		[Row(new object[]{"foo", ""})]
		[Row(new object[] { "", "" })]
		public void TestSingleAttributes(string name, string value)
		{
			BaseGadgetControl item = new GadgetLiteral();
			item.SetAttribute(name, value);
			Assert.AreEqual(value, item.GetAttribute(name));
		}

		[Test]
		public void TestCaseInsensitiveAttributes()
		{
			string value = "small";
			BaseGadgetControl item = new GadgetLiteral();
			item.SetAttribute("Big", value);
			Assert.AreEqual(value, item.GetAttribute("big"));

			string newval = "newval";
			item.SetAttribute("big", newval);
			Assert.AreEqual(newval, item.GetAttribute("big"));
		}


		[Test]
		public void TestManyAttributes()
		{
			string valueBase = "small_{0}";
			string keyBase = "foo{0}";
			BaseGadgetControl item = new GadgetLiteral();

			int max = 200;
			for (int i = 0; i < max; i++)
			{
				item.SetAttribute(string.Format(keyBase, i), string.Format(valueBase, i));
			}

			int[] keys = new int[] { 0, 5, 55, 99, 121 };
			for (int i = 0; i < keys.Length; i++)
			{
				Assert.AreEqual(string.Format(valueBase, i), item.GetAttribute(string.Format(keyBase, i)));
			}
			
			Assert.AreEqual(string.Empty, item.GetAttribute("DOESNOTEXIST"));

		}

		[Test]
		public void TestParseSingleAttribute()
		{
			string testTag = "<foo bar=\"1\" >hello</foo>";

			GadgetLiteral ctl = new GadgetLiteral(testTag);

			Assert.IsTrue(ctl.AttributeCount > 0);
			Assert.AreEqual("1", ctl.GetAttribute("bar"));
			
		}

		[Test]
		public void TestParseMultipleAttributes()
		{
			string testTag = "<foo bar=\"1\" tag=\"foo:Bar\" >hello</foo>";

			GadgetLiteral ctl = new GadgetLiteral(testTag);

			Assert.IsTrue(ctl.AttributeCount == 2);
			Assert.AreEqual("1", ctl.GetAttribute("bar"));
			Assert.AreEqual("foo:Bar", ctl.GetAttribute("tag"));

		}

		[Test]
		public void TestParseAttributeQuoteMarks()
		{
			string[] keys = { "bar", "tag", "z", "Bra", "new" };
			string[] values = { "1", "foo:Bar", "bra", "boo'loon", @"z\""ztop" };

			string testTag = "<foo ";
			for (int i = 0; i < keys.Length; i++)
			{
				testTag += " " + keys[i] + " = ";
				if (i % 2 == 0)
				{
					testTag += "'" + values[i] + "'";
				}
				else
				{
					testTag += "\"" + values[i] + "\"";
				}

			 
			}

			testTag += " >hello</foo>";

			BaseGadgetControl ctl = new GadgetLiteral(testTag); 

			Assert.IsTrue(ctl.AttributeCount == keys.Length);

			for (int i = 0; i < keys.Length; i++)
			{
				Assert.AreEqual(values[i], ctl.GetAttribute(keys[i]));
			}

		}

	}
}

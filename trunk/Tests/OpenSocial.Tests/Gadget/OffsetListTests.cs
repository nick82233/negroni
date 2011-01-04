using System;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML.Controls;


namespace Negroni.OpenSocial.Tests.Gadget
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="OffsetList"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(OffsetList))]
    public class OffsetListTest
    {

		[Test]
		public void TestAddCount()
		{
			OffsetList list = new OffsetList();
			Assert.IsTrue(0 == list.Count);
			list.AddOffset(0, new OsmlNav().OffsetKey);
			Assert.IsTrue(1 == list.Count);
			list.AddOffset(3, 55, ControlFactory.RESERVEDKEY_LITERAL);
			Assert.IsTrue(2 == list.Count);

		}
		
		[Test]
		public void TestListSerialize()
		{
			OffsetList list = new OffsetList();
			list.AddOffset(3, new OsmlName().OffsetKey);
			list.AddOffset(15, 44, ControlFactory.RESERVEDKEY_LITERAL);
			list.AddOffset(45, new OsmlNav().OffsetKey);

			string expected = "3:os_Name|15-44:Literal|45:os_Nav";

			string val = list.ToString();
			Assert.AreEqual(expected, val, "List serialized incorrectly");
		}


		[Test]
		public void TestListDeserialize()
		{
			string str = "3:os_Name|15-44:Literal|45:os_Nav";
			OffsetList list = new OffsetList();

			Assert.IsTrue(0 == list.Count);
			list.DeserializeString(str);
			Assert.IsTrue(3 == list.Count, "Loaded count wrong");
			Assert.AreEqual(new OsmlName().OffsetKey, list[0].OffsetKey, "First item of wrong type");
			Assert.AreEqual(new OsmlNav().OffsetKey, list[list.Count - 1].OffsetKey, "Last item of wrong type");
		}


		[Test]
		public void TestSingleNestedDeserialize()
		{
			string str = "33:ContentBlock{12:TemplateScript}";

			OffsetList list = new OffsetList(str);
			Assert.AreEqual(1, list.Count, "Main list count wrong");
			Assert.AreEqual(new ContentBlock().OffsetKey, list[0].OffsetKey, "Incorrect first item - not content block");
			Assert.AreEqual(1, list[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(new OsTemplate().OffsetKey, list[0].ChildOffsets[0].OffsetKey, "Not template at first item");

		}

		[Test]
		public void TestTwoNestedDeserialize()
		{
			string str = "33:ContentBlock{12:TemplateScript{0:Literal|55:OsName|70:Literal}}";

			OffsetList list = new OffsetList(str);
			Assert.AreEqual(1, list.Count, "Main list count wrong");
			Assert.AreEqual(1, list[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(3, list[0].ChildOffsets[0].ChildOffsets.Count, "Child list count wrong");


			Assert.AreEqual(new OsTemplate().OffsetKey, list[0].ChildOffsets[0].OffsetKey, "Not template at first item");

		}

		[Test]
		public void TestTwoNestedComplextDeserialize()
		{
			string str = "33:ContentBlock{12:TemplateScript{0:Literal|55:OsName|70:Literal}}|150:ContentBlock{12:TemplateScript{0:OsName}}";

			OffsetList list = new OffsetList(str);
			Assert.AreEqual(2, list.Count, "Main list count wrong");
			Assert.AreEqual(1, list[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(3, list[0].ChildOffsets[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(1, list[1].ChildOffsets[0].ChildOffsets.Count, "Second content list count wrong");


			Assert.AreEqual(new OsTemplate().OffsetKey, list[0].ChildOffsets[0].OffsetKey, "Not template at first item");
			Assert.AreEqual(new OsTemplate().OffsetKey, list[1].ChildOffsets[0].OffsetKey, "Not template at first item");
			Assert.AreEqual(new ContentBlock().OffsetKey, list[1].OffsetKey, "Not Content at second item");

		}


		[Test]
		public void TestNestedContentDeserialize()
		{
			OffsetItem offsets = new OffsetItem(GadgetTestData.GadgetOffsetListString);
			OffsetList list = offsets.ChildOffsets;
			Assert.AreEqual(2, offsets.ChildOffsets.Count, "Main list count wrong");
			Assert.AreEqual("ModulePrefs", list[0].OffsetKey, "Incorrect first item - not module prefs");
			Assert.AreEqual("ContentBlock", list[1].OffsetKey, "Incorrect second item - not content block");

			Assert.AreEqual(3, list[1].ChildOffsets[1].ChildOffsets.Count, "Child list not correct length");

		}

    }
}
	
using System;
using System.Text;
using System.Collections.Generic;


using Negroni.TemplateFramework.Tests.ExampleControls;

#if XUNIT
using Xunit;
#elif NUNIT
using NUnit;
using NUnit.Framework;
using NUnitExtension.RowTest;
#else
using MbUnit.Framework;
#endif


namespace Negroni.TemplateFramework.Tests
{
	/// <summary>
	/// Summary description for OffsetItemTests
	/// </summary>
	[TestFixture]
	public class OffsetItemTests
	{
		public OffsetItemTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}



		[Test]
		public void TestSimpleDeSerializes()
		{
			TestSimpleDeSerialize(3, ControlFactory.RESERVEDKEY_LITERAL);
			TestSimpleDeSerialize(0, ControlFactory.RESERVEDKEY_LITERAL);
			TestSimpleDeSerialize(6, "os_Nav");
			TestSimpleDeSerialize(0, "os_TabSet");
			TestSimpleDeSerialize(99, "os_TabSet");
		}


		//[RowTest]
		//[Row(new object[] { 3, ControlFactory.RESERVEDKEY_LITERAL })]
		//[Row(new object[] { 0, ControlFactory.RESERVEDKEY_LITERAL })]
		//[Row(new object[] { 6, "os_Nav" })]
		//[Row(new object[] { 0, "os_TabSet" })]
		//[Row(new object[] { 99, "os_TabSet" })]
		void TestSimpleDeSerialize(int position, string offsetKey)
		{
			OffsetItem item = new OffsetItem(position, offsetKey);
			OffsetItem itemOut = new OffsetItem();

			if (position != 0)
			{
				Assert.AreNotEqual(item.Position, itemOut.Position);
			}

			itemOut.DeserializeString(item.ToString());

			Assert.AreEqual(itemOut.Position, item.Position, "Position did not deserialize");
			Assert.IsTrue(position == itemOut.Position);
			Assert.AreEqual(itemOut.OffsetKey, item.OffsetKey, "OffsetKey did not deserialize");
			Assert.AreEqual(offsetKey, itemOut.OffsetKey, "OffsetKey changed value on deserialize.  Now: " + itemOut.OffsetKey);
		}


		[Test]
		public void TestDeserializeChildlist()
		{
			string offsetKey = new SampleContainerControl().OffsetKey;
			string offsetKeyChild = new SampleHeading().OffsetKey;
			BaseContainerControl osContainer = new SampleContainerControl();
			BaseGadgetControl childControl = new SampleHeading();
			OffsetItem item = new OffsetItem(0, osContainer.OffsetKey);
			item.ChildOffsets.AddOffset(0, ControlFactory.RESERVEDKEY_LITERAL);
			item.ChildOffsets.AddOffset(5, childControl.OffsetKey);
			item.ChildOffsets.AddOffset(15, childControl.OffsetKey);

			string deserial = item.ToString();
			string expected = "0:mytest_SampleContainer{0:Literal|5:SampleHeading|15:SampleHeading}";

			Assert.AreEqual(expected, deserial, "Unexpected deserialization string: " + deserial);

			OffsetItem result = new OffsetItem(deserial);
			Assert.IsTrue(result.HasChildList(), "Not marked as having child list");
			Assert.AreEqual(3, result.ChildOffsets.Count, "Incorrect count of children");
			Assert.AreEqual(offsetKeyChild, result.ChildOffsets[1].OffsetKey, "Child type not correct");
		}



		[Test]
		public void TestSimpleSerialize()
		{
			OffsetItem item = new OffsetItem(3, ControlFactory.RESERVEDKEY_LITERAL);
			string expected = "3:Literal";

			string val = item.ToString();
			Assert.AreEqual(expected, val, "Item serialized incorrectly");
		}

		[Test]
		public void TestSimpleRangeSerialize()
		{
			OffsetItem item = new OffsetItem(3, 99, ControlFactory.RESERVEDKEY_LITERAL);
			string expected = "3-99:Literal";

			string val = item.ToString();
			Assert.AreEqual(expected, val, "Item serialized incorrectly");
		}


		[Test]
		public void TestRangeDeSerializeValues()
		{
			//OffsetItem item = new OffsetItem(3, 99, OffsetItemType.Literal);
			string expected = "3-99:Literal";
			OffsetItem item = new OffsetItem(expected);

			string val = item.ToString();
			Assert.AreEqual(expected, val, "Item deserialized incorrectly");

			Assert.AreEqual(3, item.Position);
			Assert.AreEqual(99, item.EndPosition);
		}



		[Test]
		public void TestChildrenSerialize()
		{
			OffsetItem item = new OffsetItem(3, new SampleContainerControl().OffsetKey);
			item.ChildOffsets.AddOffset(3, new SampleHeading().OffsetKey);
			item.ChildOffsets.AddOffset(15, 44, ControlFactory.RESERVEDKEY_LITERAL);
			item.ChildOffsets.AddOffset(45, new SampleHeading().OffsetKey);

			string expected = "3:mytest_SampleContainer{3:SampleHeading|15-44:Literal|45:SampleHeading}";

			string val = item.ToString();
			Assert.AreEqual(expected, val, "Item serialized incorrectly");
		}

		[Test]
		public void TestAbsolutePosition()
		{
			string testString = "38:GadgetRoot{8:ModulePrefs|55:ContentBlock{36-148:DataScript{28-101:ViewerRequest}|148-293:TemplateScript{32:Literal|100:OsName|130-136:Literal}}}";

			OffsetItem item = new OffsetItem(testString);

			//ModulePrefs
			Assert.AreEqual(46, item.ChildOffsets[0].GetAbsolutePosition(), "First item position incorrect");
			Assert.AreEqual(2, item.ChildOffsets.Count);
			Assert.AreEqual((38 + 55), item.ChildOffsets[1].GetAbsolutePosition());
			Assert.AreEqual(2, item.ChildOffsets[1].ChildOffsets.Count, "Content block has incorrect items");
			Assert.AreEqual((38 + 55 + 36), item.ChildOffsets[1].ChildOffsets[0].GetAbsolutePosition());

		}
	}
}

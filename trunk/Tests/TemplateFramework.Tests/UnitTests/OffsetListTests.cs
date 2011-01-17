using Negroni.TemplateFramework;

using System.Text;
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
    ///This is a test class for OffsetListTest and is intended
    ///to contain all OffsetListTest Unit Tests
    ///</summary>
	[TestFixture]
	public class OffsetListTests
	{
		

		[Test]
		public void TestAddCount()
		{
			OffsetList list = new OffsetList();
			Assert.IsTrue(0 == list.Count);
			list.AddOffset(0, new SampleHeading().OffsetKey);
			Assert.IsTrue(1 == list.Count);
			list.AddOffset(3, 55, ControlFactory.RESERVEDKEY_LITERAL);
			Assert.IsTrue(2 == list.Count);

		}

		[Test]
		public void TestListSerialize()
		{
			OffsetList list = new OffsetList();
			list.AddOffset(3, new SampleHeading().OffsetKey);
			list.AddOffset(15, 44, ControlFactory.RESERVEDKEY_LITERAL);
			list.AddOffset(45, new SampleHeading().OffsetKey);

			string expected = "3:SampleHeading|15-44:Literal|45:SampleHeading";

			string val = list.ToString();
			Assert.AreEqual(expected, val, "List serialized incorrectly");
		}


		[Test]
		public void TestListDeserialize()
		{
			string str = "3:SampleHeading|15-44:Literal|45:mytest_SampleContainer";
			OffsetList list = new OffsetList();

			Assert.IsTrue(0 == list.Count);
			list.DeserializeString(str);
			Assert.IsTrue(3 == list.Count, "Loaded count wrong");
			Assert.AreEqual(new SampleHeading().OffsetKey, list[0].OffsetKey, "First item of wrong type");
			Assert.AreEqual(new SampleContainerControl().OffsetKey, list[list.Count - 1].OffsetKey, "Last item of wrong type");
		}


		[Test]
		public void TestSingleNestedDeserialize()
		{
			string str = "33:mytest_SampleContainer{12:mytest_SampleContainer}";

			OffsetList list = new OffsetList(str);
			Assert.AreEqual(1, list.Count, "Main list count wrong");
			Assert.AreEqual(new SampleContainerControl().OffsetKey, list[0].OffsetKey, "Incorrect first item - not content block");
			Assert.AreEqual(1, list[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(new SampleContainerControl().OffsetKey, list[0].ChildOffsets[0].OffsetKey, "Not template at first item");

		}

		[Test]
		public void TestTwoNestedDeserialize()
		{
			string str = "33:mytest_SampleContainer{12:mytest_SampleContainer{0:Literal|55:SampleHeading|70:Literal}}";

			OffsetList list = new OffsetList(str);
			Assert.AreEqual(1, list.Count, "Main list count wrong");
			Assert.AreEqual(1, list[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(3, list[0].ChildOffsets[0].ChildOffsets.Count, "Child list count wrong");


			Assert.AreEqual(new SampleContainerControl().OffsetKey, list[0].ChildOffsets[0].OffsetKey, "Not container at first item");

		}

		[Test]
		public void TestTwoNestedComplextDeserialize()
		{
			string str = "33:mytest_SampleContainer{12:mytest_SampleContainer{0:Literal|55:SampleHeading|70:Literal}}|150:mytest_SampleContainer{12:ASpecialContainer{0:SampleHeading}}";

			OffsetList list = new OffsetList(str);
			Assert.AreEqual(2, list.Count, "Main list count wrong");
			Assert.AreEqual(1, list[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(3, list[0].ChildOffsets[0].ChildOffsets.Count, "Child list count wrong");
			Assert.AreEqual(1, list[1].ChildOffsets[0].ChildOffsets.Count, "Second content list count wrong");


			Assert.AreEqual(new SampleContainerControl().OffsetKey, list[0].ChildOffsets[0].OffsetKey, "Not template at first item");
			Assert.AreEqual(new ASpecialContainer().OffsetKey, list[1].ChildOffsets[0].OffsetKey, "Not template at first item");
			Assert.AreEqual(new SampleContainerControl().OffsetKey, list[1].OffsetKey, "Not Content at second item");

		}


		[Test]
		public void TestNestedContentDeserialize()
		{
			string str = "33:mytest_SampleContainer{12:mytest_SampleContainer{0:Literal|55:ASpecialContainer{0:Literal|55:SampleHeading|70:Literal}|70:Literal}|150:ASpecialContainer{12:ASpecialContainer{0:SampleHeading}}}";
			OffsetItem offsets = new OffsetItem(str);
			OffsetList list = offsets.ChildOffsets;
			Assert.AreEqual(2, offsets.ChildOffsets.Count, "Main list count wrong");
			Assert.AreEqual("mytest_SampleContainer", list[0].OffsetKey, "Incorrect first item");
			Assert.AreEqual("ASpecialContainer", list[1].OffsetKey, "Incorrect second item");

			Assert.AreEqual(3, list[0].ChildOffsets[1].ChildOffsets.Count, "Child list not correct length");

		}


	}
}

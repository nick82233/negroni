using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;

namespace Negroni.OpenSocial.Tests.Gadget
{
	
    /// <summary>
    /// A <see cref="TestFixture"/> for the <see cref=" "/> class.
    /// </summary>
	[TestFixture]
	[TestsOn(typeof(ViewSize))]
	public class ViewSizeTestFixture
	{

		[Test]
		public void ViewSizeEmpty()
		{
			ViewSize v = new ViewSize();
			Assert.AreEqual(0, v.Height);
			Assert.AreEqual(0, v.Width);
		}

		[RowTest]
		[Row(new object[]{"200,300", 200, 300})]
		[Row(new object[] { "0", 0, 0})]
		[Row(new object[] { "2000,300", 2000, 300 })]
		[Row(new object[] { "300", 0, 300 })]
		public void HeightWidthViewSize(string source, int expectedWidth, int expectedHeight)
		{
			ViewSize v = new ViewSize(source);
			Assert.AreEqual(expectedWidth, v.Width);
			Assert.AreEqual(expectedHeight, v.Height);
		}



	}
}

using System;
using MbUnit.Framework;
using MySpace.OpenSocial.OSML;
using MySpace.OpenSocial.OSML.Controls;
using MySpace.OpenSocial.Gadget.Controls;


namespace MySpace.OpenSocial.Test.OSML
{
    /// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="OsControlFactory"/> class.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(ControlFactory))]
    public class OsControlFactoryTests
    {
        #region Test cases
        [RowTest]
		[Row("os:Name", typeof(OsmlName))]
		[Row("os:Form", typeof(OsForm))]
		[Row("os:Badge", typeof(OsmlBadge))]
		[Row("div", typeof(GadgetLiteral))]
		[Row("os:Nav", typeof(OsmlNav))]
		public void TestGetInstanceFromMarkup(string markupTag, Type controlType)
        {
			BaseGadgetControl gotten = ControlFactory.CreateControl(markupTag, string.Empty);
			Assert.AreEqual(controlType, gotten.GetType());
        }
        #endregion    
    }
}
	
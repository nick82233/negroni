using System;
using System.Text;
using System.Collections.Generic;

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
	/// Summary description for BaseContainerControlTests
	/// </summary>
	[TestFixture]
	public class BaseContainerControlTests
	{
		public BaseContainerControlTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		
		[Test]
		public void SimpleChildCount()
		{
			BaseContainerControl target = new BaseContainerControl();

			Assert.AreEqual(0, target.CountInternalControls(), "Did not start at zero");
			target.AddControl(new GadgetLiteral());

			Assert.AreEqual(1, target.CountInternalControls(), "Not one after single addition");

			target.AddControl(new GadgetLiteral());
			Assert.AreEqual(2, target.CountInternalControls(), "Not two after second");

		}

		[Test]
		public void NestedSingleChildCount()
		{
			BaseContainerControl target = new BaseContainerControl();

			Assert.AreEqual(0, target.CountInternalControls(), "Did not start at zero");
			target.AddControl(new GadgetLiteral());

			Assert.AreEqual(1, target.CountInternalControls(), "Not one after single addition");

			BaseContainerControl sub = (BaseContainerControl)target.AddControl(new BaseContainerControl());
			Assert.AreEqual(2, target.CountInternalControls(), "Not two after second");

			sub.AddControl(new GadgetLiteral());
			Assert.AreEqual(3, target.CountInternalControls());

			sub.AddControl(new GadgetLiteral());
			Assert.AreEqual(4, target.CountInternalControls());

		}

		[Test]
		public void NestedDoubleChildCount()
		{
			BaseContainerControl target = new BaseContainerControl();

			Assert.AreEqual(0, target.CountInternalControls(), "Did not start at zero");
			target.AddControl(new GadgetLiteral());

			Assert.AreEqual(1, target.CountInternalControls(), "Not one after single addition");

			BaseContainerControl sub = (BaseContainerControl)target.AddControl(new BaseContainerControl());
			Assert.AreEqual(2, target.CountInternalControls(), "Not two after second");

			sub.AddControl(new GadgetLiteral());
			Assert.AreEqual(3, target.CountInternalControls());

			BaseContainerControl subSub = (BaseContainerControl)sub.AddControl(new BaseContainerControl());
			Assert.AreEqual(4, target.CountInternalControls());

			subSub.AddControl(new GadgetLiteral());
			Assert.AreEqual(5, target.CountInternalControls());

		}

	}
}

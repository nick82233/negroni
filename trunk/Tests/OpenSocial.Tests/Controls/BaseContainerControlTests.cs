using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Test.TestData;


namespace Negroni.OpenSocial.Test.Controls
{
	/// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="BaseContainerControl"/> class.
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(BaseContainerControl))]
	public class BaseContainerControlTests
	{

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

		[Test]
		public void ParentChildReferences()
		{
			BaseContainerControl target = new BaseContainerControl();

			Assert.AreEqual(0, target.CountInternalControls(), "Did not start at zero");
			target.AddControl(new GadgetLiteral());

			Assert.AreEqual(1, target.Controls.Count, "Not one after single addition");

			Assert.IsNotNull(target.Controls[0].Parent, "Parent reference is null");
			Assert.AreEqual(target, target.Controls[0].Parent, "Parent reference not set correctly");

		}


		[Test]
		public void DoubleNestedParentChildReferences()
		{
			BaseContainerControl target = new BaseContainerControl();

			Assert.AreEqual(0, target.CountInternalControls(), "Did not start at zero");
			target.AddControl(new BaseContainerControl());

			Assert.AreEqual(1, target.Controls.Count, "Not one after single addition");

			GadgetLiteral second = new GadgetLiteral("Foo is bar");

			((BaseContainerControl)target.Controls[0]).AddControl(second);

			Assert.AreEqual(target, second.Parent.Parent, "Double nest does not match");
		}


	}
}

using System;
using System.Text;
using System.Collections.Generic;

using Negroni.TemplateFramework.Parsing;
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
	/// Summary description for UnitTest1
	/// </summary>
	[TestFixture]
	public class ControlFactoryTests
	{

		string key = "cftest"; //control factory key

		public ControlFactoryTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void CleanupTest()
		{
			ControlFactory.RemoveControlFactory(key);
		}

		[Test]
		public void CreateEmptyControlFactory()
		{
			Assert.IsFalse(ControlFactory.IsFactoryDefined(key), "Factory already defined");
			ControlFactory tmp = ControlFactory.CreateControlFactory(key, System.Reflection.Assembly.GetExecutingAssembly());
			Assert.IsNotNull(tmp);
		}

		[Test]
		public void CurrentAssemblyRegistersControls()
		{
			CleanupTest();

			Assert.IsFalse(ControlFactory.IsFactoryDefined(key), "Factory already defined");

			ControlFactory.CreateControlFactory(key, System.Reflection.Assembly.GetExecutingAssembly());

			Assert.IsTrue(ControlFactory.IsFactoryDefined(key), "Factory failed to register");
			ControlFactory fact = ControlFactory.GetControlFactory(key);

			Assert.IsNotNull(fact, "Factory null");
			Assert.IsTrue(0 < fact.GetControlCount(Parsing.ParseContext.DefaultContext), "No controls defined");
		}
		[Test]
		public void UnregisteredControlFactory()
		{
			CleanupTest();

			string key = "fooUnregistered";
			Assert.IsFalse(ControlFactory.IsFactoryDefined(key));
		}

		[Test]
		public void MissingContextEmpty()
		{
			CleanupTest();

			//Assert.AreEqual(-1, ControlFactory.Instance.GetControlCount(new ParseContext("Unknown")), "Unknown context has controls");
		}

		[Test]
		public void MarkupTagLookupFindsContext()
		{
			CleanupTest();
			ControlFactory fact = ControlFactory.CreateControlFactory(key, System.Reflection.Assembly.GetExecutingAssembly());

			ControlMap map = null;
			ParseContext context = null;

			Assert.IsTrue(fact.TryGetControlMap("nest:Child", out map, out context));

			Assert.AreEqual(typeof(NestedContext), context.ContainerControlType);
			//ParseContext context = fact.GetControlContextGroup(
		}
		[Test]
		public void MarkupTagBuildTagStructure()
		{
			CleanupTest();
			ControlFactory fact = ControlFactory.CreateControlFactory(key, System.Reflection.Assembly.GetExecutingAssembly());

			string fromTag = "nest:Child";
			List<string> tagNest = fact.GetTagNesting(fromTag);
			Assert.AreEqual(tagNest.Count, 3);

			Assert.IsTrue("SecondRoot".Equals(tagNest[0], StringComparison.InvariantCultureIgnoreCase), "Incorrect Root");
			Assert.IsTrue("nest:nested".Equals(tagNest[1], StringComparison.InvariantCultureIgnoreCase), "Incorrect Second");
			Assert.IsTrue(fromTag.Equals(tagNest[2], StringComparison.InvariantCultureIgnoreCase), "Final tag not included");

		}


		[Test]
		public void BuildRootMasterTree()
		{
			CleanupTest();
			ControlFactory fact = ControlFactory.CreateControlFactory(key, System.Reflection.Assembly.GetExecutingAssembly());

			ControlMap map = null;
			ParseContext context = null;
			
			Assert.IsTrue(fact.TryGetControlMap("nest:Child", out map, out context));

			Assert.AreEqual(typeof(NestedContext), context.ContainerControlType);
			//ParseContext context = fact.GetControlContextGroup(
		}

		[Test]
		public void CreateRemoveControlFactory()
		{
			CleanupTest();

			string newKey = "foobiedoobie";
			Assert.IsFalse(ControlFactory.IsFactoryDefined(newKey));
			ControlFactory cf = ControlFactory.CreateControlFactory(newKey);
			Assert.IsNotNull(cf);
			Assert.AreEqual(typeof(ControlFactory), cf.GetType());

			cf = null;
			cf = ControlFactory.GetControlFactory(newKey);
			Assert.IsNotNull(cf, "Registered ControlFactory not found with GetControlFactory call");
			Assert.IsTrue(ControlFactory.IsFactoryDefined(newKey));

			ControlFactory.RemoveControlFactory(newKey);
			Assert.IsFalse(ControlFactory.IsFactoryDefined(newKey));
			try
			{
				cf = ControlFactory.GetControlFactory(newKey);
				Assert.Fail("Should throw above");
			}
			catch (ControlFactoryNotDefinedException)
			{

			}
		}
		[Test]
		public void UndefinedFactoryGetThrows()
		{
			CleanupTest();

			string newKey = "foobiedoobie";
			try
			{
				ControlFactory cf = ControlFactory.GetControlFactory(newKey);
				Assert.Fail("Should throw above");
			}
			catch (ControlFactoryNotDefinedException)
			{

			}
		}

	}
}

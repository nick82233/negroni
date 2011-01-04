using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using MbUnit.Framework;

using Negroni.TemplateFramework;
using Negroni.DataPipeline;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.Tests.Controls;

using Negroni.OpenSocial.Tests.TestData;

namespace Negroni.OpenSocial.Tests.OSML
{
	public class OsmlControlTestBase
	{
		/// <summary>
		/// Tests the rendered results against expected output.
		/// Trims to remove leading and trailing whitespace
		/// </summary>
		/// <param name="control"></param>
		/// <param name="expectedResult"></param>
		/// <returns></returns>
		public bool AssertRenderResultsEqual(BaseGadgetControl control, string expectedResult)
		{
			string result = ControlTestHelper.GetRenderedContents(control);
			result = ControlTestHelper.NormalizeRenderResult(result);
			expectedResult = ControlTestHelper.NormalizeRenderResult(expectedResult);
			return (result == expectedResult);
		}

		public const string TEST_FACTORY_KEY = "mytestfactory";

		protected ControlFactory testFactory = null;

		public static void InitGadgetControlFactory()
		{
			ControlFactory.RemoveControlFactory(TEST_FACTORY_KEY);
			ControlFactory fact = ControlFactory.CreateControlFactory(TEST_FACTORY_KEY);
			Assembly openSocialAsm = System.Reflection.Assembly.GetAssembly(typeof(ContentBlock));
			Assembly sandboxControlAsm = System.Reflection.Assembly.GetAssembly(typeof(OsPeopleRequest));

			fact.LoadGadgetControls(openSocialAsm);
			fact.LoadGadgetControls(sandboxControlAsm);
		}

		public static void ResolveDataControlValues(DataContext dataContext, Person viewer, Person owner, List<Person> viewerFriends)
		{
			AccountTestData.ResolveDataControlValues(dataContext, viewer, owner, viewerFriends);
		}


		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			InitGadgetControlFactory();
			testFactory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);
		}
		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			testFactory = null;
			ControlFactory.RemoveControlFactory(TEST_FACTORY_KEY);
		}


	}
}

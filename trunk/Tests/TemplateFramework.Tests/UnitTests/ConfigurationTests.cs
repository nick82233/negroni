using System;
using System.Text;
using System.Collections.Generic;

using Negroni.TemplateFramework.Parsing;
using Negroni.TemplateFramework.Configuration;

#if XUNIT
using Xunit;
#elif NUNIT
using NUnit;
using NUnit.Framework;
using NUnitExtension.RowTest;
#else
using MbUnit.Framework;
#endif

namespace Negroni.TemplateFramework.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestFixture]
	public class ConfigurationTests
	{

		public ConfigurationTests()
		{
			ControlFactory.ClearControlFactories();
		}

		
		public void CleanupTest()
		{
			ControlFactory.ClearControlFactories();
		}

		[Test]
		public void AutoLoadNegroniFrameworkConfig()
		{
			CleanupTest();

			string key = "testFactory";
			Assert.IsFalse(ControlFactory.IsFactoryDefined(key), "Factory already defined");

			ControlFactory cf = ControlFactory.GetControlFactory(key);

			Assert.IsNotNull(cf);
			Assert.Greater(cf.GetControlCount(ParseContext.DefaultContext), 5);

			Assert.IsFalse(cf.LoadErrors.Count > 0, "Load errors encountered");
			ControlMap map = cf.GetControlMap("ASpecialContainer");
			Assert.IsNotNull(map);
			Assert.IsFalse(map.ControlType == typeof(GadgetLiteral));

		}

		[Test]
		public void VerifyNegroniConfigParser()
		{
			CleanupTest();

			Assert.IsFalse(ControlFactory.IsFactoryDefined(NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY), "Factory already defined");

			ControlFactory cf = ControlFactory.GetControlFactory("testFactory");

			Assert.IsNotNull(cf);

			ControlFactory cfConfig = ControlFactory.GetControlFactory(NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY);

			Assert.IsNotNull(cfConfig);
			string offsetKey = cfConfig.GetOffsetKey("assembly", ParseContext.DefaultContext);

			Assert.IsNotNull(offsetKey);
			Assert.AreEqual("assembly", offsetKey);

		}

	}
}

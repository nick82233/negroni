﻿using System;
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

namespace Negroni.TemplateFramework.Tests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestFixture]
	[TestsOn(typeof(NegroniFrameworkConfig))]
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
		public void RootElementCheckTest()
		{
			ControlFactory cf = ControlFactory.GetControlFactory(NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY);

			Assert.IsNotNull(cf);

			Negroni.TemplateFramework.Configuration.ParsingControls.NegroniControlFactories root = new Configuration.ParsingControls.NegroniControlFactories();

			Assert.IsTrue(cf.IsRootElement(root), "Config root control not recognized");

			Configuration.ParsingControls.NegroniControlFactory ctl = new Configuration.ParsingControls.NegroniControlFactory();

			Assert.IsFalse(cf.IsRootElement(ctl), "Non-root control incorrectly marked as root");

		}


		[Test]
		public void ConfigLoadDoesNotExplode()
		{
			CleanupTest();

			string key = "testFactory";

			try
			{
				NegroniFrameworkConfig.ReloadConfiguration();
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
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

			ControlFactory.ClearControlFactories();

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

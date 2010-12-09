using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline.Tests.TestData;
using Negroni.OpenSocial.EL;
using Negroni.DataPipeline;
using Negroni.DataPipeline.Security;

#if XUNIT
using Xunit;
#elif NUNIT
using NUnit;
using NUnit.Framework;
using NUnitExtension.RowTest;
#else
using MbUnit.Framework;
#endif

namespace Negroni.DataPipeline.Tests
{
	[TestFixture]
	[TestsOn(typeof(SecurityPolicy))]
	public class SecurityPolicyTestFixture
	{
		string key = "foo";
		string simpleMarkup = "<h1>Hello World</h1>";

		[Test]
		public void NoEscapingAllowsMarkup()
		{
			DataContext dc = new DataContext();

			dc.RegisterDataItem(key, simpleMarkup);

			Assert.AreEqual(EL_Escaping.None, dc.Settings.SecurityPolicy.EL_Escaping);

			string result = dc.CalculateVariableValue(key);
			Assert.AreEqual(simpleMarkup, result);
		}

		[Test]
		public void EL_EscapingAllowsMarkup()
		{
			DataContext dc = new DataContext();

			dc.RegisterDataItem(key, simpleMarkup);
			dc.Settings.SecurityPolicy.EL_Escaping = EL_Escaping.Html;
			Assert.AreEqual(EL_Escaping.Html, dc.Settings.SecurityPolicy.EL_Escaping);

			string result = dc.CalculateVariableValue(key);
			Assert.AreNotEqual(simpleMarkup, result);
			Assert.IsFalse(result.Contains("<"), "Markup left in result");
		}
	}
}

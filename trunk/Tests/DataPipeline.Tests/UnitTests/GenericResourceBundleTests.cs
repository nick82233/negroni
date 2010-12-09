using System;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline;

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
	[TestsOn(typeof(GenericResourceBundle))]
	public class GenericResourceBundleTests
	{
		[Test]
		public void InvariantCultureSet()
		{
			GenericResourceBundle target = new GenericResourceBundle();
			Assert.IsTrue(target.IsInvariantCulture);
		}
		[Test]
		public void InvariantCultureNotSet()
		{
			GenericResourceBundle target = new GenericResourceBundle("en-US");
			Assert.IsFalse(target.IsInvariantCulture);
		}

		[Test]
		public void InvariantLanguageSet()
		{
			GenericResourceBundle target = new GenericResourceBundle("en");
			Assert.IsTrue(target.IsInvariantLanguage);
			Assert.IsFalse(target.IsInvariantCulture);
		}
		[Test]
		public void InvariantLanguageNotSet()
		{
			GenericResourceBundle target = new GenericResourceBundle("en-US");
			Assert.IsFalse(target.IsInvariantLanguage);
		}

		[Test]
		public void ForceInvariantCulture()
		{
			GenericResourceBundle target = new GenericResourceBundle("en-US");
			Assert.IsFalse(target.IsInvariantCulture);
			target.SetAsInvariantCulture(true);
		}

		[Test]
		public void SetGetMessage()
		{
			string msgkey = "foo";
			string msg = "Hello world";
			GenericResourceBundle target = new GenericResourceBundle("en-US");
			target.AddString(msgkey, msg);
			Assert.IsNotNull(target.GetString(msgkey));
			Assert.AreEqual(msg, target.GetString(msgkey));
		}

	}
}

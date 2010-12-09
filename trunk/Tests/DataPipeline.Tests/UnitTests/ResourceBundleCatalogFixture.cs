using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Negroni.DataPipeline.Tests.TestData;

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
	[TestsOn(typeof(ResourceBundleCatalog))]
	public class ResourceBundleCatalogFixture
	{


		const string EN_HELLO = "Hello world";
		const string EN_GOODBYE = "goodnight, and good luck";
		const string EN_WELCOME = "welcome to my house";

		const string ES_HELLO = "Hola mundo";
		const string ES_GOODBYE = "via con dios";
		const string ES_WELCOME = "bienvenidos";

		const string DE_HELLO = "Halo";
		const string DE_GOODBYE = "Caio";
		const string DE_WELCOME = "Willkommen";

		const string KEY_HELLO = "hello";
		const string KEY_GOODBYE = "goodbye";
		const string KEY_WELCOME = "welcome";

		const string CULTURE_EN = "en";
		const string CULTURE_ES = "es";
		const string CULTURE_DE = "de";


		DataContext dc = null;


		public ResourceBundleCatalogFixture()
		{
			InitTestDataContext();
		}


		/// <summary>
		/// Builds a sample data context with test data
		/// </summary>
		/// <returns></returns>
		void InitTestDataContext()
		{
			dc = new DataContext();
			IResourceBundle resource;

			resource = new GenericResourceBundle(CULTURE_EN);
			resource.AddString(KEY_HELLO, EN_HELLO);
			resource.AddString(KEY_GOODBYE, EN_GOODBYE);
			resource.AddString(KEY_WELCOME, EN_WELCOME);
			dc.AddResourceBundle(resource);

			resource = new GenericResourceBundle(CULTURE_ES);
			resource.AddString(KEY_HELLO, ES_HELLO);
			resource.AddString(KEY_GOODBYE, ES_GOODBYE);
			resource.AddString(KEY_WELCOME, ES_WELCOME);
			dc.AddResourceBundle(resource);

			resource = new GenericResourceBundle(CULTURE_DE);
			resource.AddString(KEY_HELLO, DE_HELLO);
			resource.AddString(KEY_GOODBYE, DE_GOODBYE);
			resource.AddString(KEY_WELCOME, DE_WELCOME);
			dc.AddResourceBundle(resource);

		}


		[Test]
		public void DefinedCulturesCorrect()
		{
			string[] cultures = dc.ResourceStringCatalog.GetDefinedCultures();
			Assert.Greater(cultures.Length, 0, "Cultures empty");
			Assert.AreEqual(3, cultures.Length);
		}

		[Test]
		public void InvariantCultureExists()
		{
			Assert.IsNotNull(dc.ResourceStringCatalog.InvariantCultureBundle);
			Assert.IsTrue(dc.ResourceStringCatalog.InvariantCultureBundle.HasValues());
		}

		[Test]
		public void CultureHasCorrectMessageKeys()
		{
			//en
			string[] dekeys = dc.ResourceStringCatalog.GetKeysDefinedForCulture(CULTURE_DE);
			string[] enkeys = dc.ResourceStringCatalog.GetKeysDefinedForCulture(CULTURE_EN);

			Assert.Greater(dekeys.Length, 0);
			Assert.AreEqual(dekeys.Length, enkeys.Length);
			Assert.AreEqual(3, enkeys.Length);

		}

		[Test]
		public void CultureHasCorrectMessageKeysCaseIrrespective()
		{
			string[] dekeysLower = dc.ResourceStringCatalog.GetKeysDefinedForCulture(CULTURE_DE.ToLower());
			string[] dekeysUpper = dc.ResourceStringCatalog.GetKeysDefinedForCulture(CULTURE_DE.ToUpper());

			Assert.Greater(dekeysLower.Length, 0, "Lower case is empty");
			Assert.Greater(dekeysUpper.Length, 0, "Upper case is empty");
			Assert.AreEqual(3, dekeysLower.Length);
		}

		[Test]
		public void UndefinedCultureHasNoMessageKeys()
		{
			string[] undefinedKeys = dc.ResourceStringCatalog.GetKeysDefinedForCulture("Idontexist");
			Assert.AreEqual(0, undefinedKeys.Length);
		}

		[Test]
		public void GetKeysThenResolveFindsValues()
		{
			string[] msgKeys = dc.ResourceStringCatalog.GetKeysDefinedForCulture(CULTURE_EN);

			string testString = "";
			for (int i = 0; i < msgKeys.Length; i++)
			{
				string var = "${Msg." + msgKeys[i] + "}";
				testString += dc.ResolveMessageBundleVariables(var, CULTURE_EN);
			}
			Assert.IsTrue(testString.Contains(EN_GOODBYE));
			Assert.IsTrue(testString.Contains(EN_HELLO));
			Assert.IsTrue(testString.Contains(EN_WELCOME));

		}



		[RowTest]
		[Row(CULTURE_EN, KEY_GOODBYE)]
		[Row(CULTURE_ES, KEY_GOODBYE)]
		[Row(CULTURE_DE, KEY_GOODBYE)]
		[Row(ResourceBundleCatalog.INVARIANT_CULTURE_KEY, KEY_GOODBYE)]
		public void MessageSetForCulture(string culture, string key)
		{
			string src = "${Msg." + key + "}";
			string result = dc.ResolveMessageBundleVariables(src, culture);
			string result2 = dc.ResolveMessageBundleVariables(src, culture.ToLower());
			string result3 = dc.ResolveMessageBundleVariables(src, culture.ToUpper());
			Assert.IsFalse(string.IsNullOrEmpty(result));
			Assert.IsFalse(string.IsNullOrEmpty(result2), "Lower case culture fails");
			Assert.IsFalse(string.IsNullOrEmpty(result3), "Upper case culture fails");

			Assert.AreEqual(result, result2);
			Assert.AreEqual(result2, result3);
		}

	}
}

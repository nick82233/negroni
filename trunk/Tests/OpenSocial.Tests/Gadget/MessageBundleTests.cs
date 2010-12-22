using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.TestData.Partials;
using Negroni.OpenSocial.Test.OSML;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Test.Gadget
{
	[TestFixture]
	[TestsOn(typeof(MessageBundle))]
	public class MessageBundleTests : OsmlControlTestBase
	{


		[Test]
		public void SimpleBundleHasMessages()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			Locale locale = new Locale(null, master);
			locale.Lang = "en";
			locale.LoadMessageBundle(MessageBundleData.Source_EN);
			Assert.Greater(locale.MyMessageBundle.Messages.GetDefinedKeys().Length, 1, "No messages in bundle");
		}

		[RowTest]
		[Row(MessageBundleData.Source_DE)]
		[Row(MessageBundleData.Source_EN)]
		[Row(MessageBundleData.Source_EN_wComment)]
		[Row(MessageBundleData.Source_EN_wXML)]
		public void LoadExternalBundles(string source)
		{
			string key = "color";
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			Locale locale = new Locale(null, master);
			locale.LoadMessageBundle(source);
			Assert.Greater(locale.MyMessageBundle.Messages.GetDefinedKeys().Length, 1, "No messages in bundle");
			Assert.IsTrue(locale.MyMessageBundle.Messages.HasString(key));
			string val;

			Assert.IsTrue(locale.MyMessageBundle.Messages.TryGetString(key, out val));
			Assert.IsFalse(string.IsNullOrEmpty(val));
		}

		[RowTest]
		[Row(null)]
		[Row("")]
		public void ExternalBundleLoadRobustness(string source)
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			Locale locale = new Locale(null, master);
			locale.LoadMessageBundle(source);
			Assert.IsNotNull(locale.MyMessageBundle.Messages);
		}

		[Test]
		public void ExternalBundlesLoadedIntoDataContext()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			Locale locale = new Locale(null, master);
			locale.LoadMessageBundle(MessageBundleData.Source_EN);
			
			string testMsg = "${Msg.color}";

			string result = master.MasterDataContext.ResolveMessageBundleVariables(testMsg, "en");

			Assert.IsFalse(string.IsNullOrEmpty(result));
			Assert.AreEqual("Color", result);
		}

		[Test]
		public void SerializeExternalMessages()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Locale locale;
			locale = new Locale(null, master);
			locale.Lang = "en";
			locale.MessageSrc = "http://foo.com/en.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_EN);

			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "de";
			locale.MessageSrc = "http://foo.com/de.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_DE);

			master.ModulePrefs.AddControl(locale);

			string[] cultures = master.MyDataContext.ResourceStringCatalog.GetDefinedCultures();
			string cult = "";
			for (int i = 0; i < cultures.Length; i++)
			{
				cult += "|" + cultures[i];			 
			}
			Assert.IsTrue(cult.Contains("en"));
			Assert.IsTrue(cult.Contains("de"));

			string allMessages = master.GetConsolidatedMessageBundles();
			Assert.IsNotNull(allMessages, "Serialized messages is null");
			Assert.IsTrue(allMessages.Contains("lang=\"en\""), "English culture flag not found in response");
			Assert.IsTrue(allMessages.Contains("lang=\"de\""));

			Assert.IsTrue(allMessages.Contains(@"<msg name=""blue"">Blau</msg>"), "missing german test string");
			Assert.IsTrue(allMessages.Contains(@"<msg name=""blue"">blue</msg>"), "Missing English test string");

		}


		[Test]
		public void ReloadSerializedMessages()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Locale locale;
			locale = new Locale(null, master);
			locale.Lang = "en";
			locale.MessageSrc = "http://foo.com/en.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_EN);

			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "de";
			locale.MessageSrc = "http://foo.com/de.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_DE);

			master.ModulePrefs.AddControl(locale);

			string allMessages = master.GetConsolidatedMessageBundles();

			Assert.IsFalse(allMessages.Contains(">>"), "Consolidated contains illegal >> chars");
			Assert.IsFalse(allMessages.Contains("<<"), "Consolidated contains illegal << chars");

			GadgetMaster master2 = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			master2.LoadConsolidatedMessageBundles(allMessages);

			Assert.IsTrue(master2.ModulePrefs.Locales.Count > 1, "Locales not created after messagebundle load");

			string testString = "${Msg.blue}";

			string result;
			result = master2.MasterDataContext.ResolveMessageBundleVariables(testString, "de");
			Assert.AreEqual("Blau", result, "DE string failed");
			result = master2.MasterDataContext.ResolveMessageBundleVariables(testString, "en");
			Assert.AreEqual("blue", result, "EN string failed");
		}

		[Test]
		public void RoundTripConsolidatedMessages()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Locale locale;
			locale = new Locale(null, master);
			locale.Lang = "en";
			locale.MessageSrc = "http://foo.com/en.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_EN);

			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "de";
			locale.MessageSrc = "http://foo.com/de.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_DE);

			master.ModulePrefs.AddControl(locale);

			string allMessages = master.GetConsolidatedMessageBundles();

			string testString = @"<msg name=""blue"">Blau</msg>";

			Assert.IsTrue(allMessages.Contains(testString), "First pass missing message");

			Assert.IsFalse(allMessages.Contains(">>"), "Consolidated contains illegal >> chars");
			Assert.IsFalse(allMessages.Contains("<<"), "Consolidated contains illegal << chars");

			GadgetMaster master2 = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			master2.LoadConsolidatedMessageBundles(allMessages);

			string roundTripMessages = master2.GetConsolidatedMessageBundles(true);

			Assert.IsTrue(roundTripMessages.Contains(testString), "Second pass missing message");

			GadgetMaster master3 = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			master3.LoadConsolidatedMessageBundles(roundTripMessages);

			Assert.IsTrue(master3.GetConsolidatedMessageBundles(true).Contains(testString), "Third pass missing message");

		}

/*
		  <msg name=""hello_world"">
    Hello world
  </msg>
  <msg name=""color"">Color</msg> 
  <msg name=""red"">red</msg> 
  <msg name=""green"">green</msg> 
  <msg name=""blue"">blue</msg> 
  <msg name=""gray"">gray</msg> 
  <msg name=""purple"">Purple</msg> 
  <msg name=""black"">Black</msg>
 * */
		[RowTest]
		[Row("__MSG_color__", "en", "Color")]
		[Row("My favorite __MSG_color__ is __MSG_green__", "en", "My favorite Color is green")]
		[Row("__MSG_hello_world__", "en", "Hello world")]
		[Row("__MSG_color__ is ${Msg.color}", "en", "Color is Color")]
		[Row("${Msg.color} is __MSG_color__", "en", "Color is Color")]
		[Row("is __MSG_color__ is ${Msg.color} is", "en", "is Color is Color is")]
		public void LegacyMessageVariableResolves(string source, string culture, string expected)
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Locale locale;
			locale = new Locale(null, master);
			locale.Lang = "en";
			locale.MessageSrc = "http://foo.com/en.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_EN);

			master.ModulePrefs.AddControl(locale);

			locale = new Locale(null, master);
			locale.Lang = "de";
			locale.MessageSrc = "http://foo.com/de.xml";
			locale.LoadMessageBundle(MessageBundleData.Source_DE);

			master.ModulePrefs.AddControl(locale); 
			
			string result = master.MasterDataContext.ResolveMessageBundleVariables(source, culture).Trim();
			Assert.AreEqual(expected, result);
		}


	}
}

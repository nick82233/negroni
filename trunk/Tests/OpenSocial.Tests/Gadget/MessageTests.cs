using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls;

using Negroni.OpenSocial.Tests.TestData;
using Negroni.OpenSocial.Tests.TestData.Partials;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Tests.Gadget
{
	[TestFixture]
	[TestsOn(typeof(Message))]
	public class MessageTests : OsmlControlTestBase
	{

		/// <summary>
		/// Creates the test object
		/// </summary>
		/// <returns></returns>
		ModulePrefs GetModulePrefsObject()
		{
			GadgetMaster master = new GadgetMaster(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));
			ModulePrefs target = new ModulePrefs(ModulePrefsInlineMessagesData.Source, master);
			return target;
		}


		[Test]
		public void MessagePresent()
		{
			ModulePrefs target = GetModulePrefsObject();

			Assert.GreaterThan(target.CountInternalControls(), 0, "No controls Parsed");
		}

		[Test]
		public void MessageContents_BadFormat()
		{
			ModulePrefs target = GetModulePrefsObject();

			BaseGadgetControl control = target.Controls[0];
			Assert.GreaterThan(target.CountInternalControls(), 0, "No controls Parsed");
		}


		[Test]
		public void MessageLocaleFound()
		{
			ModulePrefs target = GetModulePrefsObject();
			Assert.GreaterThan(target.Locales.Count, 0);
		}


		[Test]
		public void MessageControlHasText()
		{
			ModulePrefs target = GetModulePrefsObject();

			Locale l = target.Locales[0];
			Assert.IsTrue(l.MyMessageBundle.Messages.HasValues(), "No messages registered");
			Assert.GreaterThan(l.Controls.Count, 0, "Message not also registered in Controls collection");


			Message msg = l.MyMessageBundle.MessageControls[0];  //.Messages.GetDefinedMessages().Values[0];
			Assert.IsNotEmpty(msg.Value);
			Assert.IsNotNull(msg.Value);
			Assert.AreEqual(ModulePrefsInlineMessagesData.testmsg1_string, msg.Value, "Message contents incorrect");

		}


		[Test]
		public void MessageInDataContext()
		{
			ModulePrefs target = GetModulePrefsObject();

			string msgExpression = "${Msg." + ModulePrefsInlineMessagesData.testmsg1_key + "}";
			string result = target.MyDataContext.GetVariableValue(msgExpression);
			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result);
			Assert.AreEqual(ModulePrefsInlineMessagesData.testmsg1_string, result, "Gotten message invalid");

		}

	}
}

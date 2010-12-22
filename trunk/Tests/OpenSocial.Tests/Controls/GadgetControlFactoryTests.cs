using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.TestData;


namespace Negroni.OpenSocial.Test.Controls
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="testFactory"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(ControlFactory))]
	public class GadgetControlFactoryTests
    {
		const string TEST_FACTORY_KEY = "mytestfactory";

		ControlFactory testFactory = null;

		static void InitGadgetControlFactory()
		{
			ControlFactory fact = ControlFactory.CreateControlFactory(TEST_FACTORY_KEY);
			Assembly openSocialAsm = System.Reflection.Assembly.GetAssembly(typeof(ContentBlock));
			Assembly sandboxControlAsm = System.Reflection.Assembly.GetAssembly(typeof(OsPeopleRequest));

			fact.LoadGadgetControls(openSocialAsm);
			fact.LoadGadgetControls(sandboxControlAsm);
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


		[RowTest]
		[Row(typeof(GadgetMaster))]
		[Row(typeof(ModulePrefs))]
		[Row(typeof(DataScript))]
		[Row(typeof(OsTemplate))]
		[Row(typeof(ContentBlock))]
		[Row(typeof(object))]
		public void GadgetControlCatalogNotEmpty(Type contextKey)
		{
			InitGadgetControlFactory();
			ParseContext context = new ParseContext(contextKey);
			Assert.Greater(testFactory.GetControlCount(context), 0, "Context empty: " + context.ToString());
		}


		[RowTest]
		[Row(typeof(ModulePrefs), "GadgetRoot")]
		[Row(typeof(ContentBlock), "GadgetRoot")]
		[Row(typeof(OsTemplate), "ContentBlock")]
		[Row(typeof(DataScript), "ContentBlock")]
		[Row(typeof(OsViewerRequest), "DataScript")]
		[Row(typeof(OsPeopleRequest), "DataScript")]
		public void RegisteredContextOffset(Type controlType, string expectedOffsetKey)
		{
			List<ParseContext> contexts = testFactory.GetControlContextGroups(controlType);

			Assert.IsNotNull(contexts, "Returned context is null");

			Assert.IsTrue(contexts.Count > 0, "No contexts found");
			
			OffsetItem expectedRootOffset = new OffsetItem(0, expectedOffsetKey);

			bool foundExpected = false;

			OffsetItem gottenRoot = null;
			for (int i = 0; i < contexts.Count; i++)
			{
				gottenRoot = testFactory.CreateRootOffset(contexts[i]);
				if (gottenRoot.Equals(expectedRootOffset))
				{
					foundExpected = true;
					break;
				}
			}
			
			Assert.IsTrue(foundExpected, "Incorrect root context: " + gottenRoot.OffsetKey);
		}

		[Test]
		public void ContextGroupsRegistered()
		{
			int ExpectedGadgetContextGroups = 13;// 14;
			Assert.Greater(testFactory.GetAvailableContextGroups().Count, 0, "No ContextGroups found in ControlFactory");
			Assert.AreEqual(ExpectedGadgetContextGroups, testFactory.GetAvailableContextGroups().Count);
		}


		[RowTest]
		[Row(typeof(ModulePrefs), "ModulePrefs")]
		[Row(typeof(ContentBlock), "ContentBlock")]
		public void CreateContextualRootOffset(Type contextGroupType, string expectedOffsetKey)
		{
			OffsetItem expectedRootOffset = new OffsetItem(0, expectedOffsetKey);

			ParseContext context = new ParseContext(contextGroupType);
			OffsetItem gottenRoot = testFactory.CreateRootOffset(context);
			Assert.AreEqual(expectedRootOffset, gottenRoot, "Incorrect root created: " + gottenRoot.OffsetKey);
		}

		[RowTest]
		[Row("<ModulePrefs>", typeof(GadgetMaster), "ModulePrefs")]
		[Row("ModulePrefs",  typeof(GadgetMaster), "ModulePrefs")]
//		[Row("<Module>",  typeof(GadgetMaster), "GadgetRoot")]
//		[Row("<script type='text/os-template' >Foo</script>",  typeof(ContentBlock), "TemplateScript")]
		[Row("Content",  typeof(GadgetMaster), "ContentBlock")]
		public void CreateContextualRootOffsetFromTag(string markupTag, Type contextGroupType, string expectedOffsetKey)
		{
			//force reload
			ControlFactory.RemoveControlFactory(TEST_FACTORY_KEY);
			InitGadgetControlFactory();
			testFactory = ControlFactory.GetControlFactory(TEST_FACTORY_KEY);

			OffsetItem expectedRootOffset = new OffsetItem(0, expectedOffsetKey);

			ParseContext context = new ParseContext(contextGroupType);
			OffsetItem gottenRoot = testFactory.CreateRootOffset(markupTag, context);
			Assert.AreEqual(expectedRootOffset, gottenRoot, "Incorrect root created: " + gottenRoot.OffsetKey);
		}


		[RowTest]
		[Row(new object[] { "os:Name", typeof(OsTemplate), typeof(OsmlName) })]
		[Row(new object[] { "os:Badge", typeof(BaseContainerControl), typeof(OsmlBadge) })]
		[Row(new object[] { "os:ViewerRequest", typeof(DataScript), typeof(OsViewerRequest) })]
		[Row(new object[] { "os:OwnerRequest", typeof(DataScript), typeof(OsOwnerRequest) })]
		[Row(new object[] { "os:PeopleRequest", typeof(DataScript), typeof(OsPeopleRequest) })]
		[Row(new object[] { "os:Repeat", typeof(OsTemplate), typeof(OsmlRepeater) })]
		[Row(new object[] { "Locale", typeof(ModulePrefs), typeof(Locale) })]
		[Row(new object[] { "Require", typeof(ModulePrefs), typeof(ModuleRequire) })]
		[Row(new object[] { "ModulePrefs", typeof(GadgetMaster), typeof(ModulePrefs) })]
		[Row(new object[] { "Content", typeof(GadgetMaster), typeof(ContentBlock) })]
		public void ScopedControlType(string tag, Type contextKey, Type expectedGadgetControlType)
		{
			ParseContext context = new ParseContext(contextKey);

			Type controlType = testFactory.GetControlType(tag, context);
			Assert.AreEqual(expectedGadgetControlType, controlType, String.Format("Bad control in context: {0}", context.ToString()));
		}


		[RowTest]
		[Row(new object[] { "os:Name", typeof(GadgetMaster), typeof(GadgetLiteral) })]
		[Row(new object[] { "os:Name", typeof(DataScript), typeof(GadgetLiteral) })]
		[Row(new object[] { "os:Name", typeof(ModulePrefs), typeof(GadgetLiteral) })]
		[Row(new object[] { "Locale", typeof(DataScript), typeof(GadgetLiteral) })]
		[Row(new object[] { "Locale", typeof(ContentBlock), typeof(GadgetLiteral) })]
		[Row(new object[] { "Locale", typeof(OsTemplate), typeof(GadgetLiteral) })]
		[Row(new object[] { "Locale", typeof(GadgetMaster), typeof(GadgetLiteral) })]
		[Row(new object[] { "os:PeopleRequest", typeof(ModulePrefs), typeof(GadgetLiteral) })]
		[Row(new object[] { "os:PeopleRequest", typeof(Locale), typeof(GadgetLiteral) })]
		[Row(new object[] { "os:PeopleRequest", typeof(GadgetMaster), typeof(GadgetLiteral) })]
		public void OutOfScopeControlType(string tag, Type contextKey, Type expectedGadgetControlType)
		{
			ParseContext context = new ParseContext(contextKey);
			Type controlType = testFactory.GetControlType(tag, context);
			Assert.AreEqual(expectedGadgetControlType, controlType, String.Format("Bad control in context: {0}", context.ToString()));
		}


		[RowTest]
		[Row("os:Name", typeof(OsmlName))]
		[Row("os:Badge", typeof(OsmlBadge))]
		[Row("div", typeof(GadgetLiteral))]
		[Row("os:Nav", typeof(OsmlNav))]
		public void GetInstanceFromTag(string markupTag, Type expectedControlType)
		{
			RootElementMaster master = new RootElementMaster(TEST_FACTORY_KEY);
			BaseGadgetControl gotten = testFactory.CreateControl(markupTag, string.Empty, ParseContext.DefaultContext, master);
			Assert.AreEqual(expectedControlType, gotten.GetType());
		}

		[RowTest]
		[Row("<script>Foo</script>", typeof(GadgetLiteral), typeof(ContentBlock))]
		[Row("<script type='text/os-data'>Foo</script>", typeof(DataScript), typeof(ContentBlock))]
		[Row("<script type='text/os-template' >Foo</script>", typeof(OsTemplate), typeof(ContentBlock))]
		[Row("<script type=\"text/os-template\" >Foo</script>", typeof(OsTemplate), typeof(ContentBlock))]
		[Row("<script type=\"text/os-template\" tag='my:Tag' >Foo</script>", typeof(OsTagTemplate), typeof(ContentBlock))]
		public void DependentAttributeGetInstance(string markup, Type expectedControlType, Type controlContextType)
		{
			RootElementMaster master = new RootElementMaster(TEST_FACTORY_KEY);
			ParseContext context = new ParseContext(controlContextType);
			BaseGadgetControl gotten = testFactory.CreateControl(markup, context, master);
			Assert.AreEqual(expectedControlType, gotten.GetType());
		}

		[RowTest]
		[Row("<script type='text/os-data'>", "DataScript", typeof(ContentBlock))]
		[Row("<script type='text/os-template' >", "TemplateScript", typeof(ContentBlock))]
		[Row("<script type=\"text/os-template\" >", "TemplateScript", typeof(ContentBlock))]
		public void GetOffsetKeyByTag(string markupTag, string expectedOffsetKey, Type contextGroupType)
		{
			ParseContext contextGroup = new ParseContext(contextGroupType);

			Assert.IsFalse(String.IsNullOrEmpty(expectedOffsetKey), "Fatal test error - empty test offsetKey");
			string gottenKey = testFactory.GetOffsetKey(markupTag, contextGroup);

			Assert.AreEqual(expectedOffsetKey, gottenKey);
			
		}

		[RowTest]
		[Row(typeof(GadgetMaster), true)]
		[Row(typeof(OsTemplate), true)]
		[Row(typeof(DataScript), true)]
		[Row(typeof(ModulePrefs), true)]
		[Row(typeof(OsmlRepeater), false)]
		[Row(typeof(GadgetLiteral), false)]
		[Row(typeof(OsmlName), false)]
		[Row(typeof(ModuleRequire), false)]
		[Row(typeof(Locale), true)]
		[Row(typeof(Link), false)]
		public void ControlTypeIsContextGroupControl(Type controlType, bool expectedAnswer)
		{
			Assert.AreEqual(expectedAnswer, testFactory.IsContextGroupContainerControl(controlType), "Type: " + controlType.Name + " gave wrong answer");
		}


		[RowTest]
		[Row("GadgetRoot", true)]
		[Row("TemplateScript", true)]
		[Row("DataScript", true)]
		[Row("ModulePrefs", true)]
		[Row("ContentBlock", true)]
		[Row("os_Repeat", false)]
		[Row("Literal", false)]
		[Row("os_Name", false)]
		[Row("ModuleRequire", false)]
		[Row("Locale", true)]
		[Row("Link", false)]
		public void IsContextGroupControl(string offsetType, bool expectedAnswer)
		{
			Assert.AreEqual(expectedAnswer, testFactory.IsContextGroupContainerControl(offsetType), "Offset: " + offsetType + " gave wrong answer");
		}



		[RowTest]
		[Row(typeof(DataScript), "DataScript", typeof(ContentBlock))]
		[Row(typeof(OsTemplate), "TemplateScript", typeof(ContentBlock))]
		[Row(typeof(OsTemplate), "TemplateScript", typeof(ContentBlock))]
		[Row(typeof(OsmlName), "os_Name", typeof(OsTemplate))]
		[Row(typeof(OsmlRepeater), "os_Repeat", typeof(OsTemplate))]
		public void GetOffsetKeyByType(Type controlType, string expectedOffsetKey, Type contextGroupType)
		{
			ParseContext contextGroup = new ParseContext(contextGroupType);

			string gottenKey = testFactory.GetOffsetKey(controlType, contextGroup);

			Assert.AreEqual(expectedOffsetKey, gottenKey, "Key from ControlFactory.GetOffsetKey incorrect: " + controlType.Name);
			
		}

		[RowTest]
		[Row(typeof(DataScript), "DataScript", typeof(ContentBlock))]
		[Row(typeof(OsTemplate), "TemplateScript", typeof(ContentBlock))]
		[Row(typeof(OsTemplate), "TemplateScript", typeof(ContentBlock))]
		[Row(typeof(OsmlName), "os_Name", typeof(OsTemplate))]
		[Row(typeof(OsmlRepeater), "os_Repeat", typeof(OsTemplate))]
		public void GetOffsetKeyByInstance(Type controlType, string expectedOffsetKey, Type contextGroupType)
		{
			ParseContext contextGroup = new ParseContext(contextGroupType);

			BaseGadgetControl instance = Activator.CreateInstance(controlType) as BaseGadgetControl;
			Assert.AreEqual(expectedOffsetKey, instance.OffsetKey, "Key from control instance incorrect: " + controlType.Name);
			
		}


		[RowTest]
		[Row(typeof(DataScript), typeof(ContentBlock), typeof(DataScript))]
		[Row(typeof(OsTemplate), typeof(ContentBlock), typeof(OsTemplate))]
		[Row(typeof(ContentBlock), typeof(GadgetMaster), typeof(ContentBlock))]
		[Row(typeof(OsmlRepeater), typeof(OsTemplate), typeof(OsTemplate))]
		public void ChildParseContext(Type testControlType, Type parentContextType, Type expectedContextType)
		{
			ParseContext expected = new ParseContext(expectedContextType);
			ParseContext context = testFactory.GetChildControlContextGroup(testControlType, new ParseContext(parentContextType));

			Assert.AreEqual(expected, context, "Incorrect child ParseContext for: " + testControlType.Name);
		}



		[RowTest]
		[Row("<div >", "div")]
		[Row("<h1>", "h1")]
		[Row("< div >", "div")]
		[Row("<hr/>", "hr")]
		public void MarkupTagNameParse(string markup, string expectedTagName)
		{
			string tag = ControlFactory.GetTagName(markup);
			Assert.AreEqual(expectedTagName, tag);
		}


		[RowTest]
		[Row("<div repeat='${ppl}' >foo${Cur.Name}</div>", typeof(OsTemplate), typeof(OsmlRepeater))]
//		[Row("<div if='${ppl.length==1}' >", OffsetItemType.OsIf, typeof(OsmlIf))]
		[Row("<div Repeat='${ppl}' >foo${Cur.Name}</div>", typeof(OsTemplate), typeof(OsmlRepeater))]
		[Row("<div xrepeat='${ppl}' >foo${Cur.Name}</div>", typeof(OsTemplate), typeof(GadgetLiteral))]
		public void ControlAttributeAlternates(string markup, Type parseContextType, Type expectedControlType)
		{
			ParseContext context = new ParseContext(parseContextType);
			string tag = ControlFactory.GetTagName(markup);
			RootElementMaster master = new RootElementMaster(TEST_FACTORY_KEY);

			BaseGadgetControl gotten = testFactory.CreateControl(tag, markup, context, master);
			Assert.IsTrue(gotten is GadgetLiteral, "Initial tag test was not a GadgetLiteral");


			gotten = testFactory.CreateControl(markup, context, master);
			Assert.AreEqual(expectedControlType, gotten.GetType(), "Attribute discovered Type object returned is incorrect");
		}



	}
}
	
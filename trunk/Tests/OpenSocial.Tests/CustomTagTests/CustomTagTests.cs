using System;
using System.IO;
using System.Collections.Generic;

using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.DataPipeline;
using Negroni.OpenSocial.Tests.Controls;
using Negroni.OpenSocial.Tests.OSML;
using Negroni.OpenSocial.DataContracts;


namespace Negroni.OpenSocial.Tests
{
    /// <summary>
    /// A <see cref="TestFixture"/> for the <see cref="GadgetMaster"/> class.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(CustomTag))]
	public class CustomTagTests : OsmlControlTestBase
    {
		CustomTagFactory factory = null;
		CustomTagTemplate template = null;

		string tagWithAttribParams = "<{0} name=\"Foo\" place='Bar' ></{0}>";
		string tagWithElemParams = "<{0}><name>Foo</name><place>Bar</place></{0}>";

		public CustomTagTests()
		{
			tagWithAttribParams = String.Format(tagWithAttribParams, CustomTagFactoryTests.CUSTOM_TAG);
			tagWithElemParams = String.Format(tagWithElemParams, CustomTagFactoryTests.CUSTOM_TAG);
		}


		[SetUp]
		public void SetupCustomTag()
		{
			factory = new CustomTagFactory(ControlFactory.GetControlFactory(TEST_FACTORY_KEY));

			Assert.IsTrue(0 == factory.CustomTags.Count);
			template = factory.RegisterCustomTag(CustomTagFactoryTests.CUSTOM_TAG, CustomTagFactoryTests.CUSTOM_TAG_TEMPLATE);
			Assert.IsTrue(1 == factory.CustomTags.Count);
		}
		[TearDown]
		public void TearDownTest()
		{
			factory = null;
		}


		[Test]
		public void TestRegisterTag()
		{
			string tag = CustomTagFactoryTests.CUSTOM_TAG;

			CustomTag inst = factory.CreateTagInstance(tag);
			Assert.AreEqual(tag, inst.MarkupTag);
			Assert.AreEqual(tag, template.Tag);

		}

		[Test]
		public void TestRenderStaticTag()
		{
			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG);

			string result = ControlTestHelper.GetRenderedContents(inst);
			Assert.AreEqual(CustomTagFactoryTests.CUSTOM_TAG_RESULT, result);
		}


		[Test]
		public void TestCloneTag()
		{
			string tag = "foo:Bar";
			CustomTag target = new CustomTag();
			Assert.IsTrue(string.IsNullOrEmpty(target.MarkupTag), "Empty tag is not null");
			target = new CustomTag(tag);
			Assert.AreEqual(tag, target.MarkupTag, "Markup tag not taken off constructor");
			
			CustomTag target2 = (CustomTag)target.Clone();
			Assert.AreEqual(tag, target2.MarkupTag, "Cloned tag incorrect");
		}


		[Test]
		public void TagInstanceParametersEmpty()
		{
			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG);
			Assert.AreEqual(0, inst.Parameters.Count, "Parameters not empty initially");
		}

		[Test]
		public void ElementParametersExist()
		{
			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tagWithElemParams);

			Assert.Greater(inst.Parameters.Count, 0);
		}
		[Test]
		public void ElementParametersSet()
		{
			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tagWithElemParams);

			Assert.AreEqual("Foo", inst.Parameters["name"]);
			Assert.AreEqual("Bar", inst.Parameters["place"]);
		}

		[Test]
		public void ElementParametersAsArray()
		{

			string tag = String.Format("<{0}>", CustomTagFactoryTests.CUSTOM_TAG);

			string[] colors = {"red", "green", "blue"};

			for (int i = 0; i < colors.Length; i++)
			{
				tag += "<color>" + colors[i] + "</color>";
			}

			tag += String.Format("</{0}>", CustomTagFactoryTests.CUSTOM_TAG);

			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tag);

			List<string> val = inst.Parameters["color"] as List<string>;

			Assert.IsNotNull(val, "Element list is not a string array");

			Assert.AreEqual(3, val.Count, "Count of items incorrect");
		}


		[Test]
		public void ComplexElementParametersSet()
		{
			string nameVal = "Joe";
			string placeVal = "Seattle";
			string markupParamVal = "<h1>You are awesome!!!</h1>";
			string tableParamVal = "<table><tr><td>Who are you\n</td>\n</tr>\n</table>";

			string tag =
@"<{0}>
<name>" + nameVal + @"</name>
<place >" + placeVal + @"</place><markup>" + markupParamVal + @"</markup>
<tableStuff>" + tableParamVal + @"</tableStuff>
</{0}>";

			tag = String.Format(tag, CustomTagFactoryTests.CUSTOM_TAG);

			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tag);

			Assert.AreEqual(nameVal, inst.Parameters["name"]);
			Assert.AreEqual(placeVal, inst.Parameters["place"]);
			Assert.AreEqual(markupParamVal, inst.Parameters["markup"]);
			Assert.AreEqual(tableParamVal, inst.Parameters["tableStuff"]);
		}

		[Test]
		public void ElementEmptyWithAttributeParametersSet()
		{
			string nameVal = "Joe";
			string placeVal = "Seattle";

			string tag =
@"<{0}>
<person name='{1}' place=""{2}""  />
</{0}>";

			tag = String.Format(tag, CustomTagFactoryTests.CUSTOM_TAG, nameVal, placeVal);

			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tag);

			Assert.IsTrue(inst.Parameters.ContainsKey("person"), "Main key 'person' not defined");
		}

		[Test]
		public void ComplexInvalidNestedElementParametersSet()
		{
			string tableParamVal = "<table><tr><td>Who are you\n</td>\n</tr>\n</table>";

			string tag =
@"<{0}>
<table>" + tableParamVal + @"</table>
</{0}>";

			tag = String.Format(tag, CustomTagFactoryTests.CUSTOM_TAG);

			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tag);
			//just make sure we don't crash

			Assert.IsTrue(inst.Parameters.ContainsKey("table"));
		}


		[Test]
		public void AttributeParametersExist()
		{
			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tagWithAttribParams);

			Assert.Greater(inst.Parameters.Count, 0);
		}

		[Test]
		public void AttributeParametersSet()
		{
			CustomTag inst = factory.CreateTagInstance(CustomTagFactoryTests.CUSTOM_TAG, tagWithAttribParams);

			Assert.AreEqual("Foo", inst.Parameters["name"]);
			Assert.AreEqual("Bar", inst.Parameters["place"]);
		}


		[Test]
		public void ParseNotIsEmpty()
		{
			Assert.IsFalse(CustomTag.IsEmptyElement(tagWithElemParams, CustomTagFactoryTests.CUSTOM_TAG), "Tag incorrectly not empty: " + tagWithElemParams);
		}

		[Test]
		public void ParseYesIsEmpty()
		{
			Assert.IsTrue(CustomTag.IsEmptyElement(tagWithAttribParams, CustomTagFactoryTests.CUSTOM_TAG), "Tag incorrectly not empty: " + tagWithAttribParams);
		}

		[Test]
		public void ParseYesIsEmptySingleClose()
		{
			string tag = String.Format("<{0} />", CustomTagFactoryTests.CUSTOM_TAG);
			Assert.IsTrue(CustomTag.IsEmptyElement(tag, CustomTagFactoryTests.CUSTOM_TAG), "Tag incorrectly not empty: " + tag);
		}

		[Test]
		public void ParseYesIsEmptyWithSpace()
		{
			string tag = String.Format("<{0} >    </{0}>", CustomTagFactoryTests.CUSTOM_TAG);
			Assert.IsTrue(CustomTag.IsEmptyElement(tag, CustomTagFactoryTests.CUSTOM_TAG), "Tag incorrectly not empty: " + tag);
		}

		[RowTest]
		[Row("<foo>", true)]
		[Row(null, true)]
		[Row("<<foo>", false)]
		[Row("<foo></foo>", true)]
		[Row("<foo>  <foo>", true)]
		[Row("<foo><sdf /></foo><foo>", true)]
		[Row("<foo>\n<b \nsd\n ddk /> ", true)]
		public void TestIsTagBalancedElement(string markup, bool expected)
		{
			Assert.AreEqual(expected, CustomTag.IsTagBalancedElement(markup), "Unbalanced:  " + markup);
		}



		[Test]
		public void TagSimpleElementParametersRender()
		{

			string content = "My favorite color is {0}";
			string param = "color";
			string tag = "x:Foo";
			string val = "blue";
			string template = "<script type='text/os-template' tag='{0}'>\n{1}</script>";
			template = String.Format(template, tag, string.Format(content, "${My." + param + "}"));

			string elemInstMarkup = "<{0}><{1}>{2}</{1}></{0}>";


			string tagInstanceMarkup = string.Format(elemInstMarkup, new string[] { tag, param, val });
			string expected = string.Format(content, val);

			factory.RegisterCustomTag(tag, template);
			CustomTag inst = factory.CreateTagInstance(tag, tagInstanceMarkup);

			Assert.AreEqual(val, inst.Parameters[param], "Registered instance value incorrect");

			string result = ControlTestHelper.GetRenderedContents(inst);
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.AreEqual(expected, result, "Rendered results incorrect");

		}


		[Test]
		public void TagTopVariableParametersRender()
		{
			string name = "Yohan";
			Person viewer = ControlTestHelper.CreatePerson(909, name, null);

			string content = "My Name is {1} and my favorite color is {0}";
			string param = "color";
			string tag = "x:Foo";
			string val = "blue";
			string paramTop = "vwr.DisplayName";
			//${Top.vwr.DisplayName}
			string template = "<script type='text/os-template' tag='{0}'>\n{1}</script>";
			template = String.Format(template, tag, string.Format(content, "${My." + param + "}", "${Top." + paramTop + "}"));

			string elemInstMarkup = "<{0}><{1}>{2}</{1}></{0}>";


			string tagInstanceMarkup = string.Format(elemInstMarkup, new string[] { tag, param, val });
			string expected = string.Format(content, val, name);

			factory.RegisterCustomTag(tag, template);
			DataContext dc = factory.MyRootMaster.MyDataContext;
			ResolveDataControlValues(dc, viewer, viewer, null);
			OsViewerRequest viewerReq = new OsViewerRequest();
			viewerReq.Key = "vwr";
			
			dc.RegisterDataItem(viewerReq);
			ResolveDataControlValues(dc, viewer, viewer, null);

			CustomTag inst = factory.CreateTagInstance(tag, tagInstanceMarkup);

			Assert.AreEqual(val, inst.Parameters[param], "Registered instance value incorrect");

			string result = ControlTestHelper.GetRenderedContents(inst);
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.AreEqual(expected, result, "Rendered results incorrect");

		}




		[Test]
		public void DynamicDataElementParametersRender()
		{
			string name = "Yohan";
			Person viewer = ControlTestHelper.CreatePerson(909, name, null);

			string content = "My Name is {0}";
			string param = "thisPerson";
			string mainPersonDataKey = "vwr";
			//string instVariable = "${" + param 
			string paramGet = param + ".DisplayName";
			string tag = "x:Foo";
	
			string template = "<script type='text/os-template' tag='{0}'>\n{1}</script>";
			template = String.Format(template, tag, string.Format(content, "${My." + paramGet + "}"));

			string elemInstMarkup = "<{0}><{1}>{2}</{1}></{0}>";


			string tagInstanceMarkup = string.Format(elemInstMarkup, new string[] { tag, param, "${" + mainPersonDataKey + "}"});
			string expected = string.Format(content, name);

			factory.RegisterCustomTag(tag, template);

			DataContext dc = factory.MyRootMaster.MyDataContext;
			OsViewerRequest viewerReq = new OsViewerRequest();
			viewerReq.Key = mainPersonDataKey;

			dc.RegisterDataItem(viewerReq);
			ResolveDataControlValues(dc, viewer, viewer, null);

			CustomTag inst = factory.CreateTagInstance(tag, tagInstanceMarkup);

//			Assert.AreEqual(name, inst.Parameters[param], "Registered instance value incorrect");

			string result = ControlTestHelper.GetRenderedContents(inst);
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.AreEqual(expected, result, "Rendered results incorrect");

		}



		[Test]
		public void TagTemplateHasControls()
		{
			string template = "<script type='text/os-template' tag='my:foo'>Hello world <os:name person='${vwr}' />. I am a walrus</script>";
			string tag = "my:foo";

			//CustomTagTemplate control = new CustomTagTemplate(tag, template, factory.MyRootMaster);
			OsTagTemplate control = new OsTagTemplate(tag, template, factory.MyRootMaster.MyControlFactory);

			Assert.Greater(control.Controls.Count, 1, "Only one control inside");
			Assert.AreNotEqual(typeof(GadgetLiteral), control.Controls[1].GetType(), "Incorrect type of child control");
			Assert.AreEqual(typeof(GadgetLiteral), control.Controls[0].GetType(), "First item not a literal");
		}

		[RowTest]
		[Row("color", "blue")]
		[Row("Color", "blue")]
		[Row("realColor", "green")]
		[Row("realcolor", "green")]
		[Row("RealBigColor", "Red")]
		public void TagSimpleElementParametersRender(string paramKey, string value)
		{

			string content = "My favorite color is {0}";
			string tag = "x:Foo";
			string template = "<script type='text/os-template' tag='{0}'>\n{1}</script>";
			template = String.Format(template, tag, string.Format(content, "${My." + paramKey + "}"));

			string elemInstMarkup = "<{0}><{1}>{2}</{1}></{0}>";


			string tagInstanceMarkup = string.Format(elemInstMarkup, new string[] { tag, paramKey, value });
			string expected = string.Format(content, value);

			factory.RegisterCustomTag(tag, template);
			CustomTag inst = factory.CreateTagInstance(tag, tagInstanceMarkup);

			string result = ControlTestHelper.GetRenderedContents(inst);
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.AreEqual(expected, result, "Rendered results incorrect");

		}

		[RowTest]
		[Row("color", "blue")]
		[Row("Color", "blue")]
		[Row("realColor", "green")]
		[Row("realcolor", "green")]
		[Row("RealBigColor", "Red")]
		public void TagSimpleAttributeParametersRender(string paramKey, string value)
		{

			string content = "My favorite color is {0}";
			string tag = "x:Foo";
			string template = "<script type='text/os-template' tag='{0}'>\n{1}</script>";
			template = String.Format(template, tag, string.Format(content, "${My." + paramKey + "}"));

			string elemInstMarkup = "<{0} {1}=\"{2}\"></{0}>";


			string tagInstanceMarkup = string.Format(elemInstMarkup, new string[] { tag, paramKey, value });
			string expected = string.Format(content, value);

			factory.RegisterCustomTag(tag, template);
			CustomTag inst = factory.CreateTagInstance(tag, tagInstanceMarkup);

			string result = ControlTestHelper.GetRenderedContents(inst);
			result = ControlTestHelper.NormalizeRenderResult(result);
			Assert.AreEqual(expected, result, "Rendered results incorrect");

		}


	}
}

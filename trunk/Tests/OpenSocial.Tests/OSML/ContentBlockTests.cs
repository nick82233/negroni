using System;
using System.IO;
using MbUnit.Framework;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

using Negroni.OpenSocial.Test.TestData;
using Negroni.OpenSocial.Test.TestData.Partials;
using Negroni.OpenSocial.Test.Controls;
using Negroni.DataPipeline;

namespace Negroni.OpenSocial.Test.OSML
{
    /// <summary>
	/// A <see cref="TestFixture"/> for the <see cref="ContentBlock"/> class.
    /// </summary>
    [TestFixture]
	[TestsOn(typeof(ContentBlock))]
	public class ContentBlockTests : OsmlControlTestBase
    {

		string sampleContent =
@"<Content type='html'>
	<script type='text/os-data'>
	<os:ViewerRequest key='Viewer' id='Viewer' />
	</script>

	<script type='text/os-template' name='foo'>
	<div>
		Hello, ${Viewer.DisplayName}. Welcome to our app.
	</div>
<p>
And your badge says:
</p>
<os:Badge person='${Viewer}' />
	</script>
</Content>";


		[Test]
		public void TestParseOffsets()
		{
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock block = new ContentBlock();
			block.MyRootMaster = master;
			block.LoadTag(sampleContent);

			//bool done = block.Parse();
			//Assert.IsTrue(done);
			OffsetList offsets = block.MyOffset.ChildOffsets;
			Assert.AreEqual(2, offsets.Count, "Incorrect offset count");

			Assert.IsTrue("DataScript" == offsets[0].OffsetKey, "Data script not found as first item");
			Assert.IsTrue("TemplateScript" == offsets[1].OffsetKey, "Template script not second item");

			for (int i = 0; i < offsets.Count; i++)
			{
				Assert.IsTrue(offsets[i].Position > 0, String.Format("Offset {0} is negatively positioned", i));
			}
			
		}

		[Test]
		public void TestSkeletalFullGadgetOffsets()
		{
			FullGadgetTestData data = new FullGadgetTestData();
			GadgetMaster root = new GadgetMaster(testFactory, data.Source);

			if (!root.IsParsed)
			{
				root.Parse();
			}
			Assert.IsTrue(root.IsParsed);
			Assert.IsTrue(root.MyOffset.ToString().IndexOf("ContentBlock") > -1, "Content Block not found in offsets");
			Assert.AreEqual(data.ExpectedOffsets, root.MyOffset.ToString(), "Full offset list incorrect");
		}

		[Test]
		public void SkeletalFullWithoutTemplatesHasBlocks()
		{
			GadgetWithoutTemplatesData data = new GadgetWithoutTemplatesData();
			GadgetMaster target = GadgetMaster.CreateGadget(TEST_FACTORY_KEY, data.Source);

			Assert.IsTrue(target.IsParsed);
			Assert.IsTrue(target.MyOffset.ToString().IndexOf("ContentBlock") > -1, "Content Block not found in offsets");

			Assert.Greater(target.ContentBlocks.Count, 0, "No content blocks defined");
		}


		[Test]
		public void TestContentRender()
		{
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock block = master.AddContentBlock(new ContentBlock());
			block.LoadTag(sampleContent);

			//block.ParseContent();

			MemoryStream s = new MemoryStream(1024);
			StreamWriter w = new StreamWriter(s);
			block.Render(w);
			w.Flush();
			s.Seek(0, SeekOrigin.Begin);
			string tagRendered = ControlTestHelper.GetStreamContent(s);

			Assert.IsTrue(tagRendered.Length > 0, "Empty rendered tag from content");
		}


		[Test]
		public void TestContentRenderTrailingScript()
		{
			ContentBlockTrailingClientScript testContent = new ContentBlockTrailingClientScript();
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock block = new ContentBlock();
			master.AddContentBlock(block);
			block.LoadTag(testContent.Source);
			block.IncludeWrappingDivs = false;

			string tagRendered = ControlTestHelper.NormalizeRenderResult(ControlTestHelper.GetRenderedContents(block));

			Assert.IsTrue(tagRendered.Length > 0, "Empty rendered tag from content");
			Assert.AreEqual( ControlTestHelper.NormalizeRenderResult(testContent.ExpectedCanvas), tagRendered);
		}
		

		[Test]
		public void CustomTagTemplateRegistered()
		{

			ContentBlock target;
			TagTemplateContentBlockTestData data = new TagTemplateContentBlockTestData();

			
			GadgetMaster master = new GadgetMaster(testFactory);
//			master.MyDataContext.Viewer = GadgetTestData.Viewer;
			target = new ContentBlock(data.Source, master);

			OffsetItem offset = target.MyOffset;

			Assert.AreEqual(2, offset.ChildOffsets.Count);
			Assert.AreEqual(new OsTagTemplate().OffsetKey, offset.ChildOffsets[0].OffsetKey);

			string written = ControlTestHelper.GetRenderedContents(target);

			Assert.IsTrue(written.Length > 0);

		}


		[RowTest]
		[Row("<Content view='canvas'>x</Content>", "canvas.about", false)]
		[Row("<Content view='canvas.about'>x</Content>", "canvas.about", true)]
		[Row("<Content view='home,canvas.about'>x</Content>", "canvas.about", true)]
		[Row("<Content view='home.about'>x</Content>", "canvas.about", false)]
		public void SubViewsCorrect(string content, string subview, bool expected)
		{
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock target = new ContentBlock(content, master);
			//master.AddContentBlock(target);

			Assert.AreEqual(expected, master.IsSubView(target, subview));
		}


		[RowTest]
		[Row("<Content view='home.about'>x</Content>", false)]
		[Row("<Content view='home,about,Profile,profile'>x</Content>", false)]
		[Row("<Content view='profile.left'>x</Content>", false)]
		[Row("<Content >x</Content>", true)]
		[Row("<Content view='*'>x</Content>", true)]
		[Row("<Content view='canvas,*'>x</Content>", true)]
		public void IsAnonymousViewCorrect(string content, bool expected)
		{
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock target = new ContentBlock(content, master);
			Assert.AreEqual(expected, target.IsAnonymousView());
		}

		[RowTest]
		[Row("<Content view='home.about'>x</Content>", "home", true)]
		[Row("<Content view='home.about'>x</Content>", "about", false)]
		[Row("<Content view='home,about,Profile,profile'>x</Content>", "canvas", false)]
		[Row("<Content view='profile.left'>x</Content>", "profile", true)]
		[Row("<Content >x</Content>", "*",  true)]
		[Row("<Content view='*'>x</Content>", "*", true)]
		[Row("<Content view='canvas,*'>x</Content>", "Canvas", true)]
		[Row("<Content view='canvas, home, profile'>x</Content>", "canvas", true)]
		[Row("<Content view='mobilecanvas'>x</Content>", "canvas", false)]
		public void IsQualifiedViewCorrect(string content, string testview, bool expected)
		{
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock target = new ContentBlock(content, master);
			Assert.AreEqual(expected, target.IsQualifiedView(testview));
		}

		[RowTest]
		[Row("<Content view='home.about'>x</Content>", "home", true)]
		[Row("<Content view='home.'>x</Content>", "home", false)]
		[Row("<Content view='home.about'>x</Content>", "about", false)]
		[Row("<Content view='home,about,Profile,profile'>x</Content>", "canvas", false)]
		[Row("<Content >x</Content>", "*", false)]
		[Row("<Content view='*'>x</Content>", "*", false)]
		[Row("<Content view='canvas'>x</Content>", "Canvas", false)]
		[Row("<Content view='canvas.profile'>x</Content>", "canvas", true)]
		[Row("<Content view='mobilecanvas.home'>x</Content>", "canvas", false)]
		public void IsSubViewCorrect(string content, string testview, bool expected)
		{
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock target = new ContentBlock(content, master);
			Assert.AreEqual(expected, target.IsSubView(testview));
		}

		[RowTest]
		[Row("<Content view='profile.left'>x</Content>", "profile", false)]
		[Row("<Content view='profile.right'>x</Content>", "profile", false)]
		[Row("<Content view='profile.top'>x</Content>", "profile", true)]
		[Row("<Content view='profile'>x</Content>", "profile", false)]
		public void IsProfileSubViewCorrect(string content, string testview, bool expected)
		{
			GadgetMaster master = new GadgetMaster(testFactory);
			ContentBlock target = new ContentBlock(content, master);
			Assert.AreEqual(expected, target.IsSubView(testview));
		}


	}
}
	
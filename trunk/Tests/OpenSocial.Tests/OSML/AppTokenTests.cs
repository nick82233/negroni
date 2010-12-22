using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using MySpace.OpenSocial.Gadget;
using MySpace.Security.Services;
using MbUnit.Framework;
using MySpace.TemplateFramework;

using MySpace.Web.Modules.Content.OsmlControls;

namespace MySpace.OpenSocial.Test.OSML
{
	[TestFixture]
	[TestsOn(typeof(MsxAppTokenControl))]
	public class AppTokenTests : OsmlControlTestBase
	{

		ControlFactory fact = null;

		public AppTokenTests()
		{
//			Assembly asm = typeof(MsxAppTokenControl).Assembly;
	//		Assembly asm2 = typeof(GadgetMaster).Assembly;
		//	fact = new ControlFactory("if", asm);
			//fact.LoadGadgetControls(asm2);
		}


		[Test]
		public void RenderContentsGood()
		{
			string txt = "<msx:AppToken applicationId='123' var='xyz' />";

			MsxAppTokenControl ctl = new MsxAppTokenControl();

			ctl.MyDataContext.RequestContext = new DataPipeline.DataRequestContext();
			ctl.MyDataContext.RequestContext.OwnerId = 6221;
			ctl.MyDataContext.RequestContext.ViewerId = 6221;
			ctl.MyDataContext.RequestContext.ApplicationId = 123;

			ctl.LoadTag(txt);

			MemoryStream stream = new MemoryStream();
			StreamWriter w = new StreamWriter(stream);

			ctl.Render(w);
			w.Flush();
			Assert.AreEqual(ctl.ApplicationId, 123);

			stream.Seek(0, SeekOrigin.Begin);
			StreamReader r = new StreamReader(stream);

			string rslt = r.ReadToEnd();

			Assert.IsTrue(rslt.Contains("xyz"), "Rendered result missing variable");
		}

	}
}

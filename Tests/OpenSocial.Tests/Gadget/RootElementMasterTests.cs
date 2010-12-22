using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

using Negroni.DataPipeline;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.Test.OSML;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;

namespace Negroni.OpenSocial.Test.Gadget
{
	[TestFixture]
	[TestsOn(typeof(RootElementMaster))]
	public class RootElementMasterTests : OsmlControlTestBase
	{

		[RowTest]
		[Row("canvas", "notcanvas", false)]
		[Row("notcanvas", "canvas", false)]
		[Row("*", "canvas", true)]
		[Row("", "canvas", true)]
		[Row("foo", "*", true)]
		[Row("foo, canvas", "canvas", true)]
		[Row("foo, canvas", "foo", true)]
		[Row("foo,canvas", "canvas", true)]
		[Row("d,foo, canvas", "dogmeat", false)]
		[Row(null, null, true)]
		public void ValidViewsTest(string registeredViews, string targetView, bool expectedResult)
		{
			Assert.AreEqual(expectedResult, RootElementMaster.IsValidView(registeredViews, targetView));
		}


		[RowTest]
		[Row("canvas", 1)]
		[Row("canvas,home", 3)]
		[Row("canvas,home", 3)]
		[Row("canvas,home,profile", 7)]
		[Row("canvas, home, canvas.about, profile", 7)]
		[Row("canvas,home,profile,mobilecanvas", 15)]
		public void ViewRegistrationTest(string viewsToRegister, int expectedResult)
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);
			int result = target.RegisterViews(viewsToRegister);
			Assert.AreEqual(expectedResult, result);
		}

		[RowTest]
		[Row("canvas")]
		[Row("canvas,home")]
		[Row("canvas,home,profile")]
		[Row("canvas, home, canvas.about, profile")]
		[Row("canvas,home,profile,mobilecanvas")]
		public void ViewRegistrationGetMaskMatches(string viewsToRegister)
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);
			int result = target.RegisterViews(viewsToRegister);
			Assert.AreEqual(result, target.GetViewMask(viewsToRegister));
		}

		[RowTest]
		[Row("canvas")]
		[Row("canvas,home")]
		[Row("canvas,home,profile")]
		[Row("canvas, home, canvas.about, profile")]
		[Row("canvas,home,profile,mobilecanvas")]
		[Row("*")]
		public void ViewStringGetTest(string viewsToRegister)
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);
			int mask = target.RegisterViews(viewsToRegister);
			string[] parts = viewsToRegister.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			string viewStr = target.GetViewString(mask);
			for (int i = 0; i < parts.Length; i++)
			{
				string x = parts[i].Trim();
				if (x.Contains("."))
				{
					x = x.Substring(0, x.IndexOf("."));
				}
				Assert.IsTrue(viewStr.Contains(x), "Missing view: " + x);
			}
		}


		[RowTest]
		[Row("canvas", "canvas", 1)]
		[Row("canvas,home", "canvas", 1)]
		[Row("canvas,home", "home", 2)]
		[Row("canvas,home,profile", "profile", 4)]
		[Row("canvas,home,profile", "home,profile", 6)]
		[Row("canvas,home,profile", "profile, home,", 6)]
		[Row("canvas, home, canvas.about, profile", ",canvas", 1)]
		public void ViewsGetIntMask(string viewsToRegister, string viewsToTest, int expectedResult)
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);
			target.RegisterViews(viewsToRegister);
			int result = target.GetViewMask(viewsToTest);
			Assert.AreEqual(expectedResult, result);			
		}


		[Test]
		public void ExternalRenderControlsInitiallyEmpty()
		{
			string view = "canvas";
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);
			Assert.IsFalse(target.HasExternalServerRenderControls());
			Assert.IsFalse(target.HasExternalServerRenderControls(view), "Named view incorrect");
		}

		[Test]
		public void ExternalRenderControlsFindControl()
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);

			OsGet ctl = new OsGet();
			ctl.LoadTag("<os:Get src='http://www.lolcats.com' resolver='server' />");

			Assert.IsFalse(string.IsNullOrEmpty(ctl.SourceUri), "Empty src");
			Assert.IsTrue(ctl.ResolveLocation == ResolveAt.Server, "Not marked for server resolution");

			Assert.IsFalse(target.HasExternalServerRenderControls());
			target.Controls.Add(ctl);
			target.RegisterExternalServerRenderControl(ctl);

			Assert.IsTrue(target.HasExternalServerRenderControls(), "Control not registered as External Ref");
		}

		[Test]
		public void ExternalRenderControlsGetControl()
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);

			OsGet ctl = new OsGet();
			ctl.LoadTag("<os:Get src='http://www.lolcats.com' resolver='server' />");

			target.Controls.Add(ctl);
			target.RegisterExternalServerRenderControl(ctl);

			List<IExternalDataSource> sources = target.GetExternalServerRenderControls();

			Assert.IsNotNull(sources);
			Assert.Greater(sources.Count, 0, "No sources defined");
			Assert.AreEqual(sources[0], ctl, "Not a matching control");
		}

		[RowTest]
		[Row("canvas", "notcanvas", false)]
		[Row("notcanvas", "canvas", false)]
		[Row("*", "canvas", true)]
		[Row("", "canvas", true)]
		[Row(null, null, true)]
		public void ScopedExternalRenderControlsFiltered(string registeredView, string scopedView, bool expectedFound)
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);
			target.RegisterViews(registeredView);
			OsGet ctl = new OsGet();
			ctl.LoadTag("<os:Get src='http://www.lolcats.com' resolver='server' />");

			target.Controls.Add(ctl);
			target.RegisterExternalServerRenderControl(ctl, target.GetViewMask(registeredView));

			List<IExternalDataSource> sources = target.GetExternalServerRenderControls(scopedView);

			Assert.IsNotNull(sources);
			Assert.AreEqual(expectedFound, sources.Count > 0, "Sources not filtered");
		}

		[RowTest]
		[Row(1, 1, true)]
		[Row(3, 1, true)]
		[Row(3, 9, false)]
		public void BitTest(int pos, uint mask, bool expectedResult)
		{
			uint result = (UInt32.MaxValue >> (32 - pos));
			uint rslt = result & mask;
			Assert.AreEqual(expectedResult, (mask == rslt));
		}

		[RowTest]
		[Row(null, "canvas", "canvas", false, true)]
		public void ViewRegistrationIdent(string initialViews, string newViews, string testView, 
			bool expectedInitial, bool expectedAfter)
		{
			RootElementMaster target = new RootElementMaster(TEST_FACTORY_KEY);
			target.RegisterViews(initialViews);
			Assert.AreEqual(expectedInitial, target.HasViewDefined(testView), "View initially state failed");
			int rslt = target.RegisterViews(newViews);
			Assert.AreEqual(expectedAfter, target.HasViewDefined(testView), "View after state failed");
		
		}

	}
}

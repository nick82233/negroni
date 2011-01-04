using System;
using System.Collections.Generic;
using System.Text;

using Negroni.TemplateFramework;
using Negroni.OpenSocial.Gadget;
using Negroni.OpenSocial.OSML;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Tests.Helpers;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.OpenSocial.Tests.Controls;

namespace Negroni.OpenSocial.Tests.TestData
{
	public static class GadgetTestData
	{
		public const string viewerName = "Steve";
		public const int viewerId = 99;


		public static Person Viewer
		{
			get
			{
				return new Person
				{
					Id = viewerId.ToString(),
					DisplayName = viewerName
				};
			}
		}

		public static List<Person> GetViewerFriendsList()
		{
			List<Person> list = new List<Person>();

			list.Add(ControlTestHelper.CreatePerson(10, "tom", "http://c2.ac-images.myspacecdn.com/images01/83/m_cf35455bc80e50b7ac64b53f659fbe89.jpg"));
			list.Add(ControlTestHelper.CreatePerson(600, "dick", "http://b8.ac-images.myspacecdn.com/00000/82/80/828_m.jpg"));
			list.Add(ControlTestHelper.CreatePerson(6001, "harry", "http://cms.myspacecdn.com/cms/FR/trax.jpg"));

			return list;
		}


		public static string FullGadget = "<?xml version='1.0' encoding='utf-8'?>\n<Module>\n\n\t<ModulePrefs title='Hello World'>\n\t</ModulePrefs>\n<Content type='html' view='profile'>\n<script type='text/os-data'><os:ViewerRequest key='Viewer' id='VIEWER' profileDetails='ABOUT,BOOKS'/>  \n</script>\n<script type='text/os-template'><div> Hello, ${Viewer.DisplayName}. Welcome to our app.<br/>You are <os:Name person='${Viewer}' /></div>\n</script>\n</Content>\n</Module>";
		public static string ProcessedTemplate = "<div> Hello, " + viewerName + ". Welcome to our app.<br/>You are " + viewerName + "</div>";
		//public static string OffsetListString = "93-396:ContentBlock{0:TemplateScript{0:Literal|212:OsName|242:Literal}}";

		public static string GadgetOffsetListString = "39-379:GadgetRoot{11-60:ModulePrefs|61-369:ContentBlock{37-150:DataScript{28-101:os_ViewerRequest}|151-297:TemplateScript{32-100:Literal|100-130:os_Name|130-137:Literal}}}";

		public static OffsetList ExpectedOffsetList;

		/// <summary>
		/// Template specific test data
		/// </summary>
		public static class Templates
		{
			public const string vname = "Steve";
			public static readonly string RawSimpleMarkup
				= "<script type=\"text/os-template\">\r\n<div>Hello <os:Name person=\"${Viewer}\" /></div>\n<h1>Hello World</h1></script>";

			public static readonly string osNameTag = "<os:Name person=\"${Viewer}\" />";

			public static readonly string ExpectedSimpleMarkup
				= "<div>Hello <a href=\"\">" + vname + "</a></div>\n<h1>Hello World</h1>";

			public static readonly string ExpectedSimpleOffsets = "0-111:TemplateScript{34-45:Literal|45-75:os_Name|75-102:Literal}";



			static BaseGadgetControl[] _simpleControls = null;
			public static BaseGadgetControl[] SimpleControls
			{
				get
				{
					if (null == _simpleControls)
					{
						_simpleControls = new BaseGadgetControl[]
						{
							new GadgetLiteral("<div>Hello "),
							new OsmlName(osNameTag),
							new GadgetLiteral("</div>\n<h1>Hello World</h1>")
						};
					}
					return _simpleControls;
				}
			}
		}

				/// <summary>
		/// Template specific test data
		/// </summary>
		public static class ContentBlock
		{
			public const string vname = "Steve";
			public static readonly string RawSimpleMarkup
				= "<Content type=\"html\">\r\n<script type=\"text/os-template\">\r\n<div>Hello <os:Name person=\"${Viewer}\" /></div>\n<h1>Hello World</h1></script>\r\n</Content>";

			public static readonly string ExpectedSimpleMarkup
				= "<div>Hello " + vname + "</div>\n<h1>Hello World</h1>";

			public static readonly string ExpectedSimpleOffsets = "0-146:ContentBlock{23-134:TemplateScript{34-45:Literal|45-75:os_Name|75-102:Literal}}";
		}

		/// Template specific test data
		/// </summary>
		public static class DataContentBlock
		{
			public static readonly string RawSimpleMarkup
				= "<Content type=\"html\"><script type=\"text/os-data\"><os:ViewerRequest key='foo'/><os:PeopleRequest key='bar'/></script>\r\n</Content>";

			public static readonly string ExpectedSimpleOffsets = "0-128:ContentBlock{21-116:DataScript{28-57:os_ViewerRequest|57-86:os_PeopleRequest}}";
		}


		static GadgetTestData()
		{
			ExpectedOffsetList = new OffsetList();
			ExpectedOffsetList.AddOffset(94, 397, "ContentBlock");
			ExpectedOffsetList[0].ChildOffsets.AddOffset(0, ControlFactory.RESERVEDKEY_LITERAL);
			ExpectedOffsetList[0].ChildOffsets.AddOffset(212, "os_Name");
			ExpectedOffsetList[0].ChildOffsets.AddOffset(242, ControlFactory.RESERVEDKEY_LITERAL);
		}

		public static class GadgetCode
		{


			public static class MultiView
			{
				static MultiView()
				{
				}

				public const string Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile'>
	<script type='text/os-template'>Profile view</script>
	</Content>
	<Content type='html' view='canvas'>
	<script type='text/os-template'>Canvas view</script>
	</Content>
	<Content type='html' view='home'>
	<script type='text/os-template'>Home view</script>
	</Content>
</Module>";

				public const string Expected_Canvas = "Canvas view";
				public const string Expected_Home = "Home view";
			}

			public static class SubView
			{

				public const string Source =
	@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile'>
	<script type='text/os-template'>Profile view</script>
	</Content>
	<Content type='html' view='canvas'>
	<script type='text/os-template'>Canvas view </script>
	</Content>
	<Content type='html' view='canvas.about'>
	<script type='text/os-template'>Canvas about view </script>
	</Content>
	<Content type='html' view='canvas.home'>
	<script type='text/os-template'>Canvas home view </script>
	</Content>
</Module>";

				public const string Expected_Canvas =
@"Canvas view 
Canvas about view 
Canvas home view 
";

				public const string Expected_Profile = "Profile view";

			}
		}


		public static class TagTemplates
		{
			static TagTemplates()
			{
				SimpleTemplate = new TestTagTemplate();
				SimpleTemplate.Tag = "my:Thing";
				SimpleTemplate.Source =
@"<script type=""text/os-template"" tag=""my:Thing"" >
<h1>Hello World</h1>
</script>";
				SimpleTemplate.Expected = "<h1>Hello World</h1>";


			}

			static public TestTagTemplate SimpleTemplate { get; set; }


		}




	}
}

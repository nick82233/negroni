using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

using Negroni.OpenSocial.Test.OSML;
using Negroni.OpenSocial.DataContracts;

using Negroni.OpenSocial.Test.Controls;

namespace Negroni.OpenSocial.Test.TestData
{
	public class IfTestData : TestableMarkupDef
	{
		public IfTestData(string testCondition, bool expectedTrue)
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='canvas'>
  <script type=""text/os-data"">
    <os:ViewerRequest key='vwr' />
    <os:PeopleRequest key='myfriends' userid=""@viewer"" groupid=""@friends"" />
  </script>
<script type='text/os-template'>
<h1>Test Below</h1>
<os:If condition='" + testCondition + @"'>
I am true
</os:If>
</script>
</Content>
</Module>";

			ExpectedCanvas = "<h1>Test Below</h1>";
			if (expectedTrue)
			{
				ExpectedCanvas += "\nI am true";
			}


			this.ExpectedViewer = new Person
			{
				Id = "6221",
				DisplayName = "Tom",
				ThumbnailUrl = "http://b2.ac-images.myspacecdn.com/00000/20/52/2502_m.jpg"
			};

			this.ExpectedFriends = GadgetTestData.GetViewerFriendsList();



		}

		/// <summary>
		/// Sample viewer to use
		/// </summary>
		public Person ExpectedViewer { get; set; }

		/// <summary>
		/// Sample friends of viewer to use
		/// </summary>
		public List<Person> ExpectedFriends { get; set; }

		/// <summary>
		/// Keys used in the data pipeline defs
		/// </summary>
		public string[] ExpectedDataKeys = { "vwr", "myfriends" };

	}
}

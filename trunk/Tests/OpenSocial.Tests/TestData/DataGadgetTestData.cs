using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Tests.OSML;

using Negroni.OpenSocial.Tests.Controls;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class DataGadgetTestData : TestableMarkupDef
	{
		public DataGadgetTestData()
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
<h1>User: ${vwr.displayName}</h1>
<os:Repeat expression=""${Top.myfriends}"">
<div>dude is: ${Cur.displayName}</div>
</os:Repeat>
</script>
</Content>
</Module>";
			ExpectedCanvas =
@"<h1>User: Tom</h1>
<div>dude is: tom</div>
<div>dude is: dick</div>
<div>dude is: harry</div>";


			this.ExpectedViewer = ControlTestHelper.CreatePerson(6221, "Tom", "http://b2.ac-images.myspacecdn.com/00000/20/52/2502_m.jpg");

			List<Person> list = new List<Person>();

			list.Add(ControlTestHelper.CreatePerson(10, "tom", "http://c2.ac-images.myspacecdn.com/images01/83/m_cf35455bc80e50b7ac64b53f659fbe89.jpg"));
			list.Add(ControlTestHelper.CreatePerson(600, "dick", "http://b8.ac-images.myspacecdn.com/00000/82/80/828_m.jpg"));
			list.Add(ControlTestHelper.CreatePerson(6001, "harry", "http://cms.myspacecdn.com/cms/FR/trax.jpg"));

			this.ExpectedFriends = list;



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

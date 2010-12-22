using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

using Negroni.OpenSocial.Test.OSML;

using Negroni.OpenSocial.Test.Controls;

namespace Negroni.OpenSocial.Test.TestData
{
	public class DataGadgetViewScopedTestData : DataGadgetTestData
	{
		public DataGadgetViewScopedTestData()
			: base()
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
<h1>User: ${vwr.Name}</h1>
<div repeat=""${myfriends}"" if=""${Context.index %2 == 0}"">dude is: ${Cur.Name}</div>
</script>
</Content>
	<Content type='html' view='profile'>
<script type='text/os-template'>
I have no available data
</script>
</Content>
	<Content type='html' view='profile,canvas,home'>
  <script type=""text/os-data"">
    <os:ViewerRequest key='globalViewer' />
</script>
</Content>

</Module>";

			this.ExpectedProfile = "I have no available data";
		
			this.ExpectedCanvas =
@"<h1>User: Tom</h1>
<div>dude is: tom</div>
<div>dude is: harry</div>";
		}

		public string GlobalDataItemKey = "globalViewer";
	}
}

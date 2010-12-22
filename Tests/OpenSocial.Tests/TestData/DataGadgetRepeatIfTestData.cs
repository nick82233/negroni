using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

using Negroni.OpenSocial.Test.OSML;

using Negroni.OpenSocial.Test.Controls;

namespace Negroni.OpenSocial.Test.TestData
{
	public class DataGadgetRepeatIfTestData : DataGadgetTestData
	{
		public DataGadgetRepeatIfTestData()
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
<os:Repeat expression=""${Top.myfriends}"">
<os:If condition=""${Context.index %2 == 0}"">
<div>dude is: ${Cur.Name}</div>
</os:If>
</os:Repeat>
</script>
</Content>
</Module>";


		
			this.ExpectedCanvas =
@"<h1>User: Tom</h1>
<div>dude is: tom</div>
<div>dude is: harry</div>";
		}


	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

using Negroni.OpenSocial.Tests.OSML;

using Negroni.OpenSocial.Tests.Controls;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class DataGadgetRepeatInJavaScriptBlock : DataGadgetTestData
	{
		public DataGadgetRepeatInJavaScriptBlock()
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
<script type='text/javascript'>
window.friendIds = "";
<os:Repeat expression='${myfriends}'>
window.friendIds += '${Cur.id},';
</os:Repeat>
// some other text
</script>
And After
</script>
</Content>
</Module>";


		
			this.ExpectedCanvas =
@"<script type='text/javascript'>
window.friendIds = "";
window.friendIds += '10,';
window.friendIds += '600,';
window.friendIds += '6001,';
// some other text
</script>
And After";
		}


	}
}

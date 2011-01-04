using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;
using Negroni.OpenSocial.DataContracts;
using Negroni.OpenSocial.Tests.OSML;

using Negroni.OpenSocial.Tests.Controls;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class DataGadgetEmptyAttrRepeatTestData : DataGadgetTestData
	{
		public DataGadgetEmptyAttrRepeatTestData()
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
<img repeat=""${Top.myfriends}"" src=""${Cur.thumbnailUrl}"" />
</script>
</Content>
</Module>";

			List<Person> friends = ExpectedFriends;

			ExpectedCanvas =
@"<h1>User: Tom</h1>
<img src=""" + friends[0].ThumbnailUrl + @""" />
<img src=""" + friends[1].ThumbnailUrl + @""" />
<img src=""" + friends[2].ThumbnailUrl + @""" />
";


		}

	}
}

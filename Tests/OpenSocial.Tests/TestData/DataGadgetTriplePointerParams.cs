using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

using Negroni.OpenSocial.Test.OSML;

using Negroni.OpenSocial.Test.Controls;

namespace Negroni.OpenSocial.Test.TestData
{
	public class DataGadgetTriplePointerParams : DataGadgetTestData
	{
		public DataGadgetTriplePointerParams()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='canvas'>
  <script type=""text/os-data"">
    <os:ViewerRequest key='vwr' />
    <os:PeopleRequest key='f1' userid=""@viewer"" groupid=""@friends"" count=""${f2.count}"" />
    <os:PeopleRequest key='f2' userid=""@viewer"" groupid=""@friends""  count=""${f3.count}""  />
    <os:PeopleRequest key='f3' userid=""@viewer"" groupid=""@friends""  count=""${f1.count}""  />
  </script>
<script type='text/os-template'>
<h1>User: ${vwr.Name}</h1>
</script>
</Content>
</Module>";

		}
	}
}

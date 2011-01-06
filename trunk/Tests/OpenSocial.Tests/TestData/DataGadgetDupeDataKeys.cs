using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

using Negroni.OpenSocial.Tests.OSML;

using Negroni.OpenSocial.Tests.Controls;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class DataGadgetDupeDataKeys : DataGadgetTestData
	{
		public DataGadgetDupeDataKeys()
			: base()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='canvas'>
  <script type=""text/os-data"">
    <os:ViewerRequest key='vwr' />
  </script>
<script type='text/os-template'>
<h1>User: ${vwr.displayName}</h1>
</script>
</Content>
	<Content type='html' view='home'>
  <script type=""text/os-data"">
    <os:ViewerRequest key='vwr' />
  </script>
<script type='text/os-template'>
<h1>Home User: ${vwr.displayName}</h1>
</script>
</Content>
	<Content type='html' view='profile'>
  <script type=""text/os-data"">
    <os:ViewerRequest key='vwr' />
  </script>
<script type='text/os-template'>
<h1>Profile User: ${vwr.displayName}</h1>
</script>
</Content>
</Module>";


		
			this.ExpectedCanvas =
@"<h1>User: Tom</h1>";
			this.ExpectedHome =
@"<h1>Home User: Tom</h1>";
			this.ExpectedProfile =
@"<h1>Profile User: Tom</h1>";
		}


	}
}

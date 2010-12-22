using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	class ExternalSourceGadgetTestData : TestableMarkupDef
	{
		public ExternalSourceGadgetTestData()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='canvas'>
	<script type='text/os-template'>Canvas view</script>
	</Content>
	<Content type='html' view='home'>
	<script type='text/os-template'>
<osx:Get src=""http://www.lolcats.com"" resolver=""server"" />
</script>
	</Content>
</Module>";

		}

		public int ExpectedExternalControlCount = 1;

	}
}

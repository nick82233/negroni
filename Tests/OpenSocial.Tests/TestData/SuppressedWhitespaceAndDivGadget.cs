using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	class SuppressedWhitespaceAndDivGadget : TestableMarkupDef
	{
		public SuppressedWhitespaceAndDivGadget()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile'>
	<script type='text/os-template'>Profile view</script>
	</Content>
	<Content type='html' view='canvas'>
	<script type='text/os-template'>Canvas view</script>
	</Content>
</Module>";

			this.ExpectedCanvas =
@"Canvas view";

			this.ExpectedProfile =
@"Profile view";

		}


		//public string ExpectedCanvas { get; set; }

		//public string ExpectedProfile { get; set; }


	}
}

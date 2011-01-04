using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class GadgetWithProfileLeftView : TestableMarkupDef
	{
		public GadgetWithProfileLeftView()
		{
			this.Source =
	@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='canvas'>
	Start of everything
	</Content>
	<Content type='html' view='profile.left'>
	<script type='text/os-template'>Profile view on left</script>
	</Content>
</Module>";


			ExpectedCanvas = "Start of everything";
			ExpectedProfile = "Profile view on left";
		}

	}
}

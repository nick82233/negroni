using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	public class GadgetWithViews : TestableMarkupDef
	{
		public GadgetWithViews()
		{
			this.Source =
	@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='canvas.start'>
	Start of everything
	</Content>
	<Content type='html' view='profile'>
	<script type='text/os-template'>Profile view</script>
	</Content>
	<Content type='html' view='Profile, home'>
	Home combo view
	</Content>
	<Content type='html' view='canvas'>
	<script type='text/os-template'>Canvas view </script>
	</Content>
	<Content type='html' view='canvas.about'>
	<script type='text/os-template'>Canvas about view </script>
	</Content>
	<Content type='html' view='canvas.home'>
	<script type='text/os-template'>Canvas home view </script>
	</Content>
</Module>";


			ExpectedCanvas = "Start of everything Canvas view Canvas about view Canvas home view";
			ExpectedMobileCanvas = "";
			ExpectedProfile = "Profile view Home combo view";
			ExpectedHome = "Home combo view";
		}

		public string ExpectedMobileCanvas { get; set; }

	}
}

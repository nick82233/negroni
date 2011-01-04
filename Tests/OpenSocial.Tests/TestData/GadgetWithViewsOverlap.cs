using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class GadgetWithViewsOverlap : TestableMarkupDef
	{
		public GadgetWithViewsOverlap()
		{
			this.Source =
	@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='mobilecanvas'>
	Mobile canvas
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
	<Content type='html' view='profilecanvas'>
	Profile canvas view
	</Content>
	<Content type='html' view='profilecanvas, mobilecanvas, Canvas'>
	Profile mobile canvas combo
	</Content>
</Module>";


			ExpectedCanvas = "Canvas view Profile mobile canvas combo";
			ExpectedMobileCanvas = "Mobile canvas Profile mobile canvas combo";
			ExpectedProfileCanvas = "Profile canvas view Profile mobile canvas combo";
			ExpectedProfile = "Profile view Home combo view";
			ExpectedHome = "Home combo view";
		}

		public string ExpectedMobileCanvas { get; set; }
		public string ExpectedProfileCanvas { get; set; }

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	class FullGadgetTestData : TestableMarkupDef
	{
		public FullGadgetTestData()
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
	<Content type='html' view='canvas.about'>
	<script type='text/os-template'>Canvas about view</script>
	</Content>
	<Content type='html' view='canvas.home'>
	<script type='text/os-template'>Canvas home view</script>
	</Content>
</Module>";

			this.ExpectedCanvas = "Canvas view";

		}

		public string ExpectedOffsets = "40-467:GadgetRoot{11-116:ContentBlock{39-92:TemplateScript{32-44:Literal}}|119-222:ContentBlock{38-90:TemplateScript{32-43:Literal}}|225-340:ContentBlock{44-102:TemplateScript{32-49:Literal}}|343-456:ContentBlock{43-100:TemplateScript{32-48:Literal}}}";

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

namespace Negroni.OpenSocial.Tests.TestData
{
	class TemplateGadgetTestData : TestableMarkupDef
	{

		public const string CUSTOM_TAG = "my:Tag";
		public const string CUSTOM_TAG_CONTENTS = "<h3 style='color:blue'>Hello Custom Tag</h3>";

		public TemplateGadgetTestData()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile,canvas'>
	<script type='text/os-template' tag=""" + CUSTOM_TAG + @""">" +
CUSTOM_TAG_CONTENTS +
@"</script>
	</Content>
<Content type='html' view='canvas'>
<script type='text/os-template'>
Canvas view<br />
<my:Tag></my:Tag>
</script>
</Content>
</Module>";
			ExpectedCanvas =
@"Canvas view<br />
<h3 style='color:blue'>Hello Custom Tag</h3>";
		}

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	class TagTemplateContentBlockTestData : TestableMarkupDef
	{
		public TagTemplateContentBlockTestData()
		{
			this.Source =
@"<Content type='html' view='profile,canvas'>
	<script type='text/os-template' tag=""my:Tag"">
<h3 style='color:blue'>Hello Custom Tag</h3>
</script>
<script type='text/os-template'>
Canvas view<br />
<my:Tag></my:Tag>
</script>
</Content>
";
			ExpectedCanvas =
@"Canvas view<br />
<h3 style='color:blue'>Hello Custom Tag</h3>";
		}

	}
}

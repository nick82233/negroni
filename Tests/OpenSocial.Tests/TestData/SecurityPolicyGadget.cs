using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	class SecurityPolicyGadget : TestableMarkupDef
	{
		public SecurityPolicyGadget()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
<ModulePrefs>
 <Optional feature='security-policy' >
    <Param name='el_escaping'>html</Param>
  </Optional >

</ModulePrefs>
	<Content type='html' view='canvas'>
<script type='text/os-data'>
<os:Var key='markup'>
<h1>Hello World</h1>
</os:Var>
</script>

<script type='text/os-template'>
${markup}
</script>
	</Content>
</Module>";

			this.ExpectedCanvas = "&lt;h1&gt;Hello World&lt;/h1&gt;";

		}

	}
}

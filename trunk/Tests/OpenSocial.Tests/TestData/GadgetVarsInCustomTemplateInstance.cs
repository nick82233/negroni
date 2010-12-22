using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	public class GadgetVarsInCustomTemplateInstance : TestableMarkupDef
	{
		public GadgetVarsInCustomTemplateInstance()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile,canvas'>
<script type='text/os-data'>
<os:ViewerRequest key=""vwr"" />
</script>

<script type='text/os-template' tag='my:Test'>
<h1>I say ${My.message}</h1>
</script>

<script type='text/os-template'>
<my:Test>
<os:Var key=""message"" value=""bar"" />
</my:Test>
</script>
	</Content>
</Module>";
			ExpectedVariableValue = "bar";

			ExpectedCanvas = "<h1>I say " + ExpectedVariableValue + "</h1>";

		}

		public string ExpectedVariableKey { get; set; }
		public string ExpectedVariableValue { get; set; }

	}
}

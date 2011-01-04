using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class GadgetVarsInTemplate : TestableMarkupDef
	{
		public GadgetVarsInTemplate()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile,canvas'>
<script type='text/os-data'>
<os:ViewerRequest key=""vwr"" />
</script>

<script type='text/os-template'>
<os:Var key=""foo"" value=""bar"" />
<h1>I say ${foo}</h1>
</script>

	</Content>
</Module>";
			ExpectedVariableKey = "foo";
			ExpectedVariableValue = "bar";

			ExpectedCanvas = "<h1>I say " + ExpectedVariableValue + "</h1>";

		}

		public string ExpectedVariableKey { get; set; }
		public string ExpectedVariableValue { get; set; }

	}
}

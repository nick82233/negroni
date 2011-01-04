using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class GadgetVarsInDataPipeline : TestableMarkupDef
	{
		public GadgetVarsInDataPipeline()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile,canvas'>
<script type='text/os-data'>
<os:Var key=""foo"" value=""bar"" />
</script>	
<script type='text/os-template'>
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

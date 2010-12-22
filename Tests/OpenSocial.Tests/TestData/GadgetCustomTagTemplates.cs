using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	public class GadgetCustomTagTemplates : TestableMarkupDef
	{
		public string ExpectedGlobalVarValue = "zebra";
		public GadgetCustomTagTemplates()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile,canvas'>
<script type='text/os-data'>
<os:ViewerRequest key=""vwr"" />
<os:Var key='refkey' value='" + ExpectedGlobalVarValue + @"' />
</script>

<script type='text/os-template' tag='my:Test'>
<h1>I say ${My.message}</h1>
${My.content}
</script>

<script type='text/os-template'>
<my:Test>
<os:Var key=""message"" value=""bar"" />
</my:Test>
<my:Test message='" + ExpectedParamAttributeValue + @"' something='Else'>
</my:Test>
<my:Test >
<message>" + ExpectedParamElementValue + @"</message>
<content>${refkey}</content>
<something>Else</something>
</my:Test>
</script>
	</Content>
</Module>";
			ExpectedVariableValue = "bar";

			ExpectedCanvas = "<h1>I say " + ExpectedVariableValue + @"</h1>"
				+ ExpectedParamAttributeValue
				+ ExpectedParamElementValue
				+ ExpectedGlobalVarValue;
				
		}

		public string ExpectedParamElementValue = "from element param";
		public string ExpectedParamAttributeValue = "from attribute param";

		public string ExpectedVariableKey { get; set; }
		public string ExpectedVariableValue { get; set; }

	}
}

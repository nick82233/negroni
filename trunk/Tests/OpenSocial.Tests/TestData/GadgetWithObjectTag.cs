using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Tests.Helpers;

namespace Negroni.OpenSocial.Tests.TestData
{
	public class GadgetWithObjectTag : TestableMarkupDef
	{
		public GadgetWithObjectTag()
		{
			this.Source =
	@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='canvas'>
	Object below:
<OBJECT DATA='mlk.mov' TYPE='video/quicktime' TITLE=""Martin Luther King's &quot;I Have a Dream&quot; speech"" WIDTH='150' HEIGHT='150'>
<PARAM NAME='pluginspage' VALUE='http://quicktime.apple.com/' />
<PARAM NAME='autoplay' VALUE='true' />
<OBJECT DATA='mlk.mp3' TYPE='audio/mpeg' TITLE=""Martin Luther King's &quot;I Have a Dream&quot; speech"">
<PARAM NAME='autostart' VALUE='true' />
<PARAM NAME='hidden' VALUE='true' />
<A HREF='mlk.html'>Full text of Martin Luther King's 'I Have a Dream' speech</A>
</OBJECT>
</OBJECT>
	</Content>
</Module>";


			ExpectedCanvas = @"	Object below:
<OBJECT DATA='mlk.mov' TYPE='video/quicktime' TITLE=""Martin Luther King's &quot;I Have a Dream&quot; speech"" WIDTH='150' HEIGHT='150'>
<PARAM NAME='pluginspage' VALUE='http://quicktime.apple.com/' />
<PARAM NAME='autoplay' VALUE='true' />
<OBJECT DATA='mlk.mp3' TYPE='audio/mpeg' TITLE=""Martin Luther King's &quot;I Have a Dream&quot; speech"">
<PARAM NAME='autostart' VALUE='true' />
<PARAM NAME='hidden' VALUE='true' />
<A HREF='mlk.html'>Full text of Martin Luther King's 'I Have a Dream' speech</A>
</OBJECT>
</OBJECT>
";
		}

	}
}

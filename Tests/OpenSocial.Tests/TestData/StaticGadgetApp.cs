using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData
{
	/// <summary>
	/// Static gadget using CDATA
	/// </summary>
	public class StaticGadgetApp : TestableMarkupDef
	{
		public StaticGadgetApp()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
	<Content type='html' view='profile'>
<![CDATA[
	Profile view
]]>
	</Content>
	<Content type='html' view='canvas'>
<![CDATA[
	Canvas view
]]>
	</Content>
</Module>";

			this.ExpectedCanvas = "Canvas view";
			this.ExpectedProfile = "Profile view";

		}
	}
}

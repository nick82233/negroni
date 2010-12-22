using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

using Negroni.OpenSocial.Test.OSML;

using Negroni.OpenSocial.Test.Controls;
using Negroni.OpenSocial.Test.TestData.TemplateLibraries;

namespace Negroni.OpenSocial.Test.TestData
{
	public class GadgetReferencingTemplateLibrary : DataGadgetTestData
	{
		public GadgetReferencingTemplateLibrary()
			: base()
		{
			this.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<Module>
<ModulePrefs>
<Require feature=""opensocial-templates"">
  <Param name=""requireLibrary"">http://www.example.com/basicLibrary.xml</Param>
</Require>
</ModulePrefs>
	<Content type='html' view='canvas'>
  <script type=""text/os-data"">
    <os:ViewerRequest key='vwr' />
  </script>
<script type='text/os-template'>
<h1>User: ${vwr.Name}</h1>
<foo:bar></foo:bar>
<foo:dog><dog>${vwr.displayName}</dog></foo:dog>
</script>
</Content>
</Module>";
			BasicLibrary testLib = new BasicLibrary();
			ExpectedCustomTags.AddRange(testLib.DefinedTags);


			ExpectedCanvas =
@"<h1>User: " + ExpectedViewer.DisplayName + @"</h1>
<h1>I am a simple template</h1> <div style='border:2px solid lightgrey;'>Woof woof. My name is " + ExpectedViewer.DisplayName + @"</div>
";


		}

		private List<string> _expectedCustomTags = null;

		/// <summary>
		/// Accessor for expectedCustomTags.
		/// Performs lazy load upon first request
		/// </summary>
		public List<string> ExpectedCustomTags
		{
			get
			{
				if (_expectedCustomTags == null)
				{
					_expectedCustomTags = new List<string>();
				}
				return _expectedCustomTags;
			}
		}

		public string ExpectedTemplateLibraryUri = "http://www.example.com/basicLibrary.xml";

	}
}

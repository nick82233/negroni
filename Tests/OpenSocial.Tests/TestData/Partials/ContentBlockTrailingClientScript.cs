using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Test.Helpers;

namespace Negroni.OpenSocial.Test.TestData.Partials
{
	class ContentBlockTrailingClientScript : TestableMarkupDef
	{

		public ContentBlockTrailingClientScript()
		{
this.Source = 
@"<Content type='html' view='canvas'>
    <script type=""text/os-data"">
      <os:PersonRequest key=""Viewer"" id=""VIEWER"" />
    </script>
    <script type=""text/os-template"">
Canvas Content Here
<script>
var x=""foo"";
</script>
    </script>
  </Content>
";
this.ExpectedCanvas =
@"
Canvas Content Here
<script>
var x=""foo"";
</script>
";
		}

	}
}

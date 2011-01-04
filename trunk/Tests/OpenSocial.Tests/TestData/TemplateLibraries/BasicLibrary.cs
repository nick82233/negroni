using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Tests.TestData.TemplateLibraries
{
	/// <summary>
	/// For spec, see 
	/// http://wiki.opensocial.org/index.php?title=OpenSocial_Templates_Developer%27s_Guide
	/// 
	/// </summary>
	public class BasicLibrary
	{
		public const string Source =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Templates xmlns:foo='http://foo.com/'>
  <Namespace prefix=""foo"" url=""http://foo.com/""/>
 
  <!-- Simple declarative tag foo:bar -->
  <Template tag=""foo:bar"">
<h1>I am a simple template</h1>
  </Template>

<Template tag=""foo:dog""><div style='border:2px solid lightgrey;'>Woof woof. My name is ${My.dog}</div>
</Template>
 
</Templates>";

		public List<string> DefinedTags = new List<string>();

		public BasicLibrary()
		{
			DefinedTags.Add("foo:bar");
			DefinedTags.Add("foo:dog");

		}

	}
}

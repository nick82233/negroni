using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Tests.TestData.TemplateLibraries
{
	/// <summary>
	/// Same as BasicLibrary, but without the preceeding XML declaration.
	/// For spec, see 
	/// http://wiki.opensocial.org/index.php?title=OpenSocial_Templates_Developer%27s_Guide
	/// 
	/// </summary>
	public class BasicLibraryNoXml : BasicLibrary
	{
		public static readonly string Source = InternalXml;

	}
}

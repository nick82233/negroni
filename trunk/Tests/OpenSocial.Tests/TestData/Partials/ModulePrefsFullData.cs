using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Test.TestData.Partials
{
	/// <summary>
	/// Gadget module preferences
	/// </summary>
	public class ModulePrefsFullData
	{
		public const string ExpectedIcon = "http://c4.ac-images.myspacecdn.com/images02/78/l_7da98739a935462cae6fcb8773ad2c63.png";
		public const string ExpectedThumbnail = "http://c1.ac-images.myspacecdn.com/images02/111/l_f39574fb5a234a689e538155e9371afc.png";
		public const string ExpectedTitle = "I am great";

		public static readonly string Source = 
@"<ModulePrefs Title='" + ExpectedTitle + @"'
thumbnail='" + ExpectedThumbnail + @"'
>
<Icon>" + ExpectedIcon + @"</Icon>
<Require feature='dynamic-height'/>
	<Require feature='opensocial-data'/>
	<Require feature='opensocial-templates'/>
</ModulePrefs>";

	}
}

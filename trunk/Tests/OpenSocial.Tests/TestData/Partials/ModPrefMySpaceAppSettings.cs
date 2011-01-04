using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Tests.TestData.Partials
{
	/// <summary>
	/// Sample data for gadget having MySpaceAppSettings info
	/// </summary>
	public class ModPrefMySpaceAppSettings
	{
		public const string AppTitle = "MySpace App Settings Test";

		public const string Source =
@"<Module>
<ModulePrefs title=""" + AppTitle +  @""" 
    title_url='http://groups.google.com/group/Google-Gadgets-API'
    author='Jane Smith' 
    author_email='xxx@google.com'>


<Optional feature='MySpace-Settings'>
<Param name=""AppCategory1"">1</Param>
<Param name=""AppCategory2"">18</Param>
<Param name='AgeRestriction'>Over21</Param>
<Param name='DefaultLanguage'>en-US</Param>
</Optional>
    
</ModulePrefs>
</Module>
";

		public const string ExpectedCategory1 = "Animals & Pets";
		public const string ExpectedCategory2 = "Dating & Relationships";
		public const string ExpectedAgeRestriction = "Over21";



	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Negroni.OpenSocial.Gadget;

namespace Negroni.OpenSocial.Test.TestData.Partials
{
	/// <summary>
	/// Sample data for gadget having MySpaceAppSettings info
	/// </summary>
	public class ModPrefMySpaceViewSettings
	{
		public const string AppTitle = "MySpace App Settings Test";

		public const string Source =
@"<Module>
<ModulePrefs title=""" + AppTitle + @""" 
    title_url='http://groups.google.com/group/Google-Gadgets-API'
    author='Jane Smith' 
    author_email='xxx@google.com'>


<Optional feature='MySpace-Views'>
<Param name=""ProfileSize"">290</Param>
<Param name=""CanvasSize"">100,500</Param>
<Param name='profileLocation'>left</Param> <!-- left/right -->
</Optional>
    
</ModulePrefs>
</Module>
";

		public static readonly ViewSize expectedCanvasSize = new ViewSize(100, 500);
		public static readonly ViewSize expectedProfileSize = new ViewSize("290");
		
		public static readonly string expectedProfileLocation = "left";
		public static readonly string expectedProfileMount = "profile.left";

		public static int expectedProfileHeight = 290;



	}
}

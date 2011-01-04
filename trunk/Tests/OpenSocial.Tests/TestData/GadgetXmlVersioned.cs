using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Tests.TestData
{
	public static class GadgetXmlVersioned
	{

		public const string VERSION_NONE_GADGET =
@"<Module>
<ModulePrefs title='Gifting friends' 
descrition='App that demonstrates the use of templates in the profile view to retrieve and display data.'>
  <Require feature='opensocial-data'/>
  <Require feature='opensocial-templates'>
    <Param name='process-on-server'>true</Param>
  </Require>
</ModulePrefs>
</Module>
";

				public const string VERSION_NONE_EMPTY_GADGET =
@"<Module>
<ModulePrefs title='Gifting friends' 
descrition='App that demonstrates the use of templates in the profile view to retrieve and display data.'>
</ModulePrefs>
</Module>
";


		public const string VERSION_08_GADGET =
@"<Module>
<ModulePrefs title='Gifting friends' 
descrition='App that demonstrates the use of templates in the profile view to retrieve and display data.'>
  <Require feature='opensocial-0.8'/>
  <Require feature='opensocial-data'/>
  <Require feature='opensocial-templates'>
    <Param name='process-on-server'>true</Param>
  </Require>
</ModulePrefs>
</Module>
";

		public const string VERSION_09_GADGET =
@"<Module>
<ModulePrefs title='Gifting friends' 
descrition='App that demonstrates the use of templates in the profile view to retrieve and display data.'>
  <Require feature='opensocial-0.9'/>
  <Require feature='opensocial-data'/>
  <Require feature='opensocial-templates'>
    <Param name='process-on-server'>true</Param>
  </Require>
</ModulePrefs>
</Module>
";

		public const string VERSION_10_GADGET =
@"<Module>
<ModulePrefs title='Gifting friends' 
descrition='App that demonstrates the use of templates in the profile view to retrieve and display data.'>
  <Require feature='opensocial-1.0'/>
  <Require feature='opensocial-data'/>
  <Require feature='opensocial-templates'>
    <Param name='process-on-server'>true</Param>
  </Require>
</ModulePrefs>
</Module>
";

	}
}

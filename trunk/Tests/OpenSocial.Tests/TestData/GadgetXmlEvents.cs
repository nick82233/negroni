using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Test.TestData
{
	public static class GadgetXmlEvents
	{
		public const string ADD_APP_EVENT_HREF = "http://something.com/install";

		public const string REMOVE_APP_EVENT_HREF = "http://something.com/remove";


		public const string SOURCE =
@"<Module>
<ModulePrefs title='Gifting friends' 
descrition='App that demonstrates the use of templates in the profile view to retrieve and display data.'>
  <Require feature='opensocial-0.9'/>
  <Require feature='opensocial-data'/>
  <Require feature='opensocial-templates'>
    <Param name='process-on-server'>true</Param>
  </Require>
<link rel='event.addapp' href='" + ADD_APP_EVENT_HREF + @"' />
<Link rel='event.removeapp' href='" + REMOVE_APP_EVENT_HREF + @"' />
</ModulePrefs>
</Module>
";


	}
}

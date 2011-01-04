using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Tests.TestData.Partials
{
	/// <summary>
	/// Gadget module preferences
	/// </summary>
	class ModulePrefsInlineMessagesData
	{
		public const string testmsg1_key = "test1";
		public const string testmsg1_string = "I am a test";
		//<UserPref name='mypref' default_value='whodidthis'/>
		public const string Source = "<ModulePrefs Title='Hello World'><Locale><MessageBundle><msg name=\"" + testmsg1_key + "\">" + testmsg1_string + "</msg></MessageBundle></Locale></ModulePrefs>";
		//below offsets are not correct
		public const string ExpectedOffsets = "0-118:ModulePrefs{33-68:Require|68-104:Require}";

		public const string ExpectedTitle = "Hello World";

	}
}

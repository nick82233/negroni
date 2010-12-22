using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Test.TestData.Partials
{
	/// <summary>
	/// Gadget module preferences
	/// </summary>
	public class ModulePrefsData
	{
		//<UserPref name='mypref' default_value='whodidthis'/>
		public const string Source = "<ModulePrefs Title='Hello World'><Require feature='dynamic-height'/><Require feature='opensocial-data'/></ModulePrefs>";
		public const string ExpectedOffsets = "0-118:ModulePrefs{33-68:Require|68-104:Require}";

		public const string ExpectedTitle = "Hello World";

	}
}

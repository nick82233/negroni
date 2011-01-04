using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Tests.TestData.Partials
{
	/// <summary>
	/// Data Pipeline Data Script sample data
	/// </summary>
	class DataScriptBlockData
	{
		public const string Source = "<script type=\"text/os-data\"><os:ViewerRequest key='foo'/><os:PeopleRequest key='bar'/></script>";
		public const string ExpectedOffsets = "0-95:DataScript{28-57:os_ViewerRequest|57-86:os_PeopleRequest}";


	}
}

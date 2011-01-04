using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.OpenSocial.Tests.Helpers
{
	public abstract class TestableMarkupDef
	{
		public string Source { get; set; }

		public string Expected { get; set; }

		public string ExpectedCanvas { get; set; }
		public string ExpectedHome { get; set; }
		public string ExpectedProfile { get; set; }
	}


	public class TestTagTemplate : TestableMarkupDef
	{
		/// <summary>
		/// Defined tag
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Custom tag usage instance
		/// </summary>
		public string InstanceSource { get; set; }
	}


}

using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.TemplateFramework.Tests.TestData
{
	/// <summary>
	/// Definition of testable markup to be used by unit tests
	/// </summary>
	public abstract class TestableMarkup
	{
		/// <summary>
		/// Raw source of test data
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Expected rendering result
		/// </summary>
		public string ExpectedRender { get; set; }
	}


	public class TestTagTemplate : TestableMarkup
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

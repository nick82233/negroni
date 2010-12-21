#if MBUNIT

#else

using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.TemplateFramework.Tests
{
	/// <summary>
	/// Implementation of "TestsOn" attribute for frameworks
	/// that do not support it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	class TestsOnAttribute : System.Attribute
	{
		public TestsOnAttribute() { }

		public TestsOnAttribute(Type typeUnderTest)
		{
			this.TypeUnderTest = typeUnderTest;
		}

		public Type TypeUnderTest { get; set; }
	}
}
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace Negroni.TemplateFramework.Tests.TestData
{
	class SimpleTestData : TestableMarkup
	{

		public SimpleTestData()
		{
			InitTestData();
		}

		public string ExpectedSomePropValue
		{
			get
			{
				return "foo";
			}
		}

		public string ExpectedHeadingValue
		{
			get
			{
				return "I am a head";
			}
		}

		/// <summary>
		/// Initialize the test data
		/// </summary>
		private void InitTestData()
		{
			base.Source =
@"<?xml version='1.0' encoding='utf-8'?>
<mytest:SampleContainer someprop='" + ExpectedSomePropValue + @"'>
	<mytest:SampleHeading>" + ExpectedHeadingValue + @"
</mytest:SampleContainer>
";
			ExpectedRender = ExpectedHeadingValue;
		}
		


	}
}

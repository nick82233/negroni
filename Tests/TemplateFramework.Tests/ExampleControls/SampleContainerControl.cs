using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework.Tests.ExampleControls
{
	[MarkupTag("mytest:SampleContainer")]
	public class SampleContainerControl : BaseContainerControl
	{

		public SampleContainerControl() { }

		public SampleContainerControl(string markup)
		{
			LoadTag(markup);
		}


		/// <summary>
		/// Value of the attribute "someprop"
		/// </summary>
		public string SomeProp
		{
			get
			{
				return GetAttribute("someprop");
			}
		}


	}
}

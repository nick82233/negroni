using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework.Tests.ExampleControls
{
	[MarkupTag("mytest:SpecialContainer")]
	[OffsetKey("ASpecialContainer")]
	public class ASpecialContainer : BaseContainerControl
	{

		public ASpecialContainer() { }

		public ASpecialContainer(string markup)
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

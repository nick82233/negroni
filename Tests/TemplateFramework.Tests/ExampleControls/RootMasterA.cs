using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework.Tests.ExampleControls
{
	[MarkupTag("RootA")]
	[OffsetKey("RootA")]
	[RootElement]
	public class RootMasterA : RootElementMaster
	{
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

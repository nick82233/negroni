using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework.Tests.ExampleControls
{
	[MarkupTag("SecondRoot")]
	[OffsetKey("SecondRoot")]
	[RootElement(false)]
	public class RootMasterSecond : RootElementMaster
	{
		/// <summary>
		/// Value of the attribute "otherprop"
		/// </summary>
		public string OtherProp
		{
			get
			{
				return GetAttribute("otherprop");
			}
		}


	}
}

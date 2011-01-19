using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework.Tests.ExampleControls
{
	[MarkupTag("nest:Child")]
	[ContextGroup(typeof(NestedContext))]
	public class NestedChild : BaseGadgetControl
	{
		public override void Render(System.IO.TextWriter writer)
		{
			writer.WriteLine("Child");
		}
		
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.DataPipeline;

namespace Negroni.TemplateFramework.Tests.ExampleControls
{
	[MarkupTag("nest:Nested")]
	[ContextGroup(typeof(RootMasterSecond))]
	public class NestedContext : BaseContainerControl
	{

		
	}
}

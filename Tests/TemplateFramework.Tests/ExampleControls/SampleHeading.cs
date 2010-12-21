using System;
using System.Collections.Generic;
using System.Text;
using Negroni.TemplateFramework;


namespace Negroni.TemplateFramework.Tests.ExampleControls
{
	[MarkupTag("mytest:SampleHeading")]
	[OffsetKey("SampleHeading")]
	public class SampleHeading : BaseGadgetControl
	{
		public SampleHeading() { }

		public SampleHeading(string markup)
		{
			LoadTag(markup);
		}

		public override void Render(System.IO.TextWriter writer)
		{
			//throw new NotImplementedException();
			writer.Write("<h1>");
			writer.Write(this.InnerMarkup);
			writer.Write("</h1>");
		}
	}
}

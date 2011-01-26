using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
	[MarkupTag("box")]
	public class BoxControl : BaseContainerControl
	{
		public override void Render(System.IO.TextWriter writer)
		{
			writer.Write("<div style='border:3px solid red;margin:2px;padding:4px;'>\n");
			base.Render(writer);
			writer.Write("</div>\n");
		}
	}
}

using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
	[MarkupTag("rainbow")]
	public class RainbowControl : BaseGadgetControl
	{

		public override void Render(System.IO.TextWriter writer)
		{
			string divTemplate = "<div style='width:50px;float:left;height:30px;background:{0};'>&nbsp;</div>";
			writer.Write(String.Format(divTemplate, "red"));
			writer.Write(String.Format(divTemplate, "orange"));
			writer.Write(String.Format(divTemplate, "yellow"));
			writer.Write(String.Format(divTemplate, "green"));
			writer.Write(String.Format(divTemplate, "blue"));
			writer.Write(String.Format(divTemplate, "violet"));

		}
	}
}
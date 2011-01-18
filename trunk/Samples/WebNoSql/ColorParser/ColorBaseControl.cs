using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
	public class ColorBaseControl : BaseGadgetControl
	{
		protected string color = "white";

		public override void Render(System.IO.TextWriter writer)
		{
			writer.Write("<div style='width:50px;float:left;height:30px;background:");
			writer.Write(color);
			writer.Write(";'>");
			writer.Write(base.InnerMarkup);
			writer.Write("</div>");
		}
	}
}
using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
	public class ColorBaseControl : BaseGadgetControl
	{
		protected string color = "white";

		public string Width { get; set; }

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			Width = GetAttribute("width");
		}

		public override void Render(System.IO.TextWriter writer)
		{
			string wd;
			if (string.IsNullOrEmpty(Width))
			{
				wd = "50px";
			}
			else
			{
				wd = Width;
			}

			writer.Write("<div style='width:");
			writer.Write(wd);
			writer.Write(";float:left;height:30px;background:");
			writer.Write(color);
			writer.Write(";'>");
			writer.Write(base.InnerMarkup);
			writer.Write("</div>");
		}
	}
}
using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
	[MarkupTag("green")]
	public class GreenControl : ColorBaseControl
	{
		public GreenControl()
		{
			this.color = "green";
		}
	}
}
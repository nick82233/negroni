using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
	[MarkupTag("red")]
	public class RedControl : ColorBaseControl
	{
		public RedControl()
		{
			this.color = "red";
		}
	}
}
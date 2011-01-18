using System;
using Negroni.TemplateFramework;

namespace WebNoSql.ColorParser
{
	[MarkupTag("blue")]
	public class BlueControl : ColorBaseControl
	{
		public BlueControl()
		{
			this.color = "blue";
		}
	}
}
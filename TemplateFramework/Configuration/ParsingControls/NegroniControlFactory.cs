using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Negroni.TemplateFramework.Configuration.ParsingControls
{
	[MarkupTag("NegroniControlFactory")]
	internal class NegroniControlFactory : BaseContainerControl, INegroniControlFactory
	{

		public NegroniControlFactory()
		{
			ControlAssemblies = new List<INegroniAssembly>();
		}

		public List<INegroniAssembly> ControlAssemblies { get; set; }

		public string Key { get; set; }

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);

			this.Key = GetAttribute("key");
		}

		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			base.AddControl(control);

			if (control is NegroniAssembly)
			{
				ControlAssemblies.Add(control as INegroniAssembly);
			}

			return control;
		}


	}
}

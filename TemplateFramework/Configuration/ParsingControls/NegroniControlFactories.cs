using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Negroni.TemplateFramework.Configuration.ParsingControls
{
	[MarkupTag("NegroniControlFactories")]
	[RootElement]
	internal class NegroniControlFactories : RootElementMaster, INegroniFactoriesSection
	{

		public NegroniControlFactories()
		{
			ControlFactories = new List<INegroniControlFactory>();
		}

		public List<INegroniControlFactory> ControlFactories { get; private set;}


		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			base.AddControl(control);

			if (control is NegroniControlFactory)
			{
				ControlFactories.Add(control as NegroniControlFactory);
			}

			return control;
		}
	}
}

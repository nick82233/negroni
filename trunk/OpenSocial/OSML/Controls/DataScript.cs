/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Negroni.OpenSocial.Gadget;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget.Controls; 
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// DataPipeline tag holder
	/// </summary>
	[MarkupTag("script")]
	[AttributeTagDependent("type", "text/os-data")]
	[ContextGroup(typeof(ContentBlock))]
	[ContextGroupContainer]
	[OffsetKey("DataScript")]
	public class DataScript : OsTemplate
	{
		public DataScript() 
		{}
		public DataScript(string markup, string controlFactoryKey)
		{
			this.MyControlFactory = ControlFactory.GetControlFactory(controlFactoryKey);
			LoadTag(markup);
		}
		public DataScript(string markup, ControlFactory controlFactory)
		{
			this.MyControlFactory = controlFactory;
			LoadTag(markup);
		}

		public DataScript(string markup, OffsetItem thisOffset, RootElementMaster master)
		{
			MyRootMaster = master;
			MyOffset = thisOffset;
			LoadTag(markup);
		}

		public DataScript(string markup, OffsetItem thisOffset, string controlFactoryKey)
		{
			this.MyControlFactory = ControlFactory.GetControlFactory(controlFactoryKey);
			MyOffset = thisOffset;
			LoadTag(markup);
		}
		public DataScript(string markup, OffsetItem thisOffset, ControlFactory controlFactory)
		{
			this.MyControlFactory = controlFactory;
			MyOffset = thisOffset;
			LoadTag(markup);
		}

		/// <summary>
		/// Validates data items are registered with the DataContext.
		/// Loads them if the given key is not found.
		/// </summary>
		public void ConfirmDataItemsRegistered()
		{
			string viewContext = MyRootMaster.GetViewString(ViewMask);
			for (int i = 0; i < Controls.Count; i++)
			{
				if (Controls[i] is BaseDataControl)
				{
					if (Controls[i] is OsHttpRequest)
					{
						OsHttpRequest ctl = (OsHttpRequest)Controls[i];
						MyDataContext.RegisterDataItem(ctl, viewContext,
							ctl.UseClientDataResolver, false, false);
					}
					else
					{
						MyDataContext.RegisterDataItem((BaseDataControl)Controls[i], viewContext,
							((BaseDataControl)Controls[i]).UseClientDataResolver, true, false);
					}
				}
				else if (Controls[i] is VariableTag)
				{
					VariableTag osvar = (VariableTag)Controls[i];
					MyDataContext.RegisterDataItem(osvar.VariableKey, osvar.GetVariableValue());
				}
			}
		}

		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			return base.AddControl(control);
		}


		protected override void ConfirmDefaultOffset()
		{
			if (null == MyOffset)
			{
				MyOffset = new OffsetItem(0, "DataScript");
			}
		}

	}
}

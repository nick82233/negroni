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
using System.Text;
using Negroni.OpenSocial.Gadget;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.Gadget.Controls;
using Negroni.TemplateFramework;
using Negroni.OpenSocial.OSML.Controls;

namespace Negroni.OpenSocial.OSML.Templates
{
	[MarkupTag("Templates")]
	[RootElement(false, true)]
	[ContextGroup(typeof(Object))]
	[ContextGroup(typeof(GadgetMaster))]
	[ContextGroupContainer]
	public class TemplatesRoot : RootElementMaster
	{
		public TemplatesRoot() 
		{
            this.MyParseContext = new ParseContext(typeof(Object));
        }

		public TemplatesRoot(string markup, GadgetMaster master)
			: this(markup, master, null)
		{ }


		public TemplatesRoot(string markup, GadgetMaster master, OffsetItem offsets)
            : this()
        {
			if (null != offsets)
			{
				MyOffset = offsets;
			}
            base.MyRootMaster = master;
            LoadTag(markup);
        }


		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			if(null == control){
				return null;
			}

			base.AddControl(control);

			if (control is CustomTagTemplate)
			{
				CustomTags.Add((CustomTagTemplate)control);
			}
			return control;

		}


		private List<CustomTagTemplate> _customTags = null;

		/// <summary>
		/// Custom tag templates defined in this template library
		/// </summary>
		public List<CustomTagTemplate> CustomTags
		{
			get
			{
				if (_customTags == null)
				{
					_customTags = new List<CustomTagTemplate>();
				}
				return _customTags;
			}
		}

	}
}

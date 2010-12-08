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
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Navigation Item
	/// </summary>
	[MarkupTag("os:Nav")]
	public class OsmlNav : BaseContainerControl
	{
		const string template = "<a href=\"javascript:OSML.navToView('##viewName##');\">##text##</a>";

		#region Constructors

		public OsmlNav() { }

		public OsmlNav(string markup, GadgetMaster master)
            : this()
        {
            base.MyRootMaster = master;
            LoadTag(markup);
        }

		#endregion

		private Dictionary<string, ParamControl> _params = null;

		/// <summary>
		/// Accessor for params.
		/// Performs lazy load upon first request
		/// </summary>
		public Dictionary<string, ParamControl> Params
		{
			get
			{
				if (_params == null)
				{
					_params = new Dictionary<string, ParamControl>();
				}
				return _params;
			}
		}


		override protected void Clear()
		{
			ViewName = null;
			RawTag = null;
		}

		public override void LoadTag(string markup)
		{
			base.LoadTag(markup);
			ViewName = this.GetAttribute("view");
		}

		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
			base.AddControl(control);
			if (control is ParamControl)
			{
				ParamControl p = (ParamControl)control;
				Params.Add(p.Name, p);
			}
			return control;
		}

		

		private string _viewName = null;
		public string ViewName
		{
			get
			{
				return _viewName;
			}
			set
			{
				if (String.IsNullOrEmpty(value))
				{
					_viewName = null;
				}
				else
				{
					_viewName = value.Replace("\"", String.Empty).Replace("'", String.Empty);
				}
			}
		}

		public override void Render(System.IO.TextWriter writer)
		{
			if (ViewName == null) return;

			string content = InnerMarkup;
			if (string.IsNullOrEmpty(content))
			{
				content = ViewName;
			}

			string x = template.Replace("##viewName##", ViewName).Replace("##text##", content);
			writer.Write(x);
		}
	}
}

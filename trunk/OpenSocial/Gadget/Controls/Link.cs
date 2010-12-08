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
using System.IO;
using Negroni.TemplateFramework.Parsing;
using Negroni.OpenSocial.OSML.Controls;
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
    [MarkupTag("Link")]
	[ContextGroup(typeof(ModulePrefs))]
	public class Link : BaseGadgetControl
    {
        public Link()
        {
			this.MyParseContext = new ParseContext(typeof(ModulePrefs));
        }

        public Link(string markup)
            : this()
        {
            LoadTag(markup);
        }

        public Link(string markup, OffsetItem offset)
            : this()
        {
            this.MyOffset = offset;

            LoadTag(markup);
        }

        public override void LoadTag(string markup)
        {
            base.LoadTag(markup);

            if (this.HasAttributes())
            {
                this.Rel = GetAttribute("rel");
                this.Href = GetAttribute("href");
            }
        }

        public override void Render(TextWriter writer)
        {
            return;
        }

		/// <summary>
		/// Relation.  This is commonly a lifecycle trigger.  Values include:
		/// <list type="bullet">
		/// <item>event</item>
		/// <item>event.addapp</item>
		/// <item>event.removeapp</item>
		/// <item>event.app-action</item>
		/// <item>event.invite</item>
		/// </list>
		/// </summary>
        public string Rel { get; set; }

        public string Href { get; set; }

		private string _method = "GET";
		/// <summary>
		/// HTTP method (GET or POST) by which request is sent
		/// </summary>
		public string Method
		{
			get
			{
				return _method;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (value.ToUpper() == "POST")
					{
						_method = "POST";
					}
					else
					{
						_method = "GET";
					}
				}
			}
		}
    }
}

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
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;

namespace Negroni.OpenSocial.Gadget.Controls
{
    [MarkupTag("msg")]
    [ContextGroup(typeof(Locale))]
    public class Message : BaseGadgetControl
    {
        public Message()
        {
            this.MyParseContext = new ParseContext(typeof(Locale));
        }

        public Message(string markup, GadgetMaster master)
            : this()
        {
            base.MyRootMaster = master;
            LoadTag(markup);
        }

        public Message(string markup, OffsetItem offset)
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
                this.Name = GetAttribute("name");
                this.Description = GetAttribute("desc");
            }

            this.Value = this.InnerMarkup;
        }


        public override void Render(TextWriter writer)
        {
            return;
        }

		/// <summary>
		/// Key for this message.  This value, along with locale, is used to identify
		/// this message in the DataContext
		/// </summary>
        public string Name { get; set; }

		/// <summary>
		/// Internal description of the message
		/// </summary>
        public string Description { get; set; }

		/// <summary>
		/// Text of the message.
		/// </summary>
        public string Value { get; set; }
    }
}

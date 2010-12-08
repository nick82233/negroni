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
using Negroni.DataPipeline;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Parsing;

namespace Negroni.OpenSocial.Gadget.Controls
{
    [MarkupTag("MessageBundle")]
    [ContextGroup(typeof(Locale))]
    public class MessageBundle : BaseContainerControl
    {
        public MessageBundle()
        {
            this.MyParseContext = new ParseContext(typeof(Locale));
        }


        public MessageBundle(string markup, GadgetMaster master)
            : this()
        {
            base.MyRootMaster = master;
            LoadTag(markup);
        }

		public MessageBundle(string markup, OffsetItem offset)
            : this()
        {
            this.MyOffset = offset;
            LoadTag(markup);
        }

        public override void Render(TextWriter writer)
        {
            return;
        }


		private List<Message> _messageControls = null;

		/// <summary>
		/// Accessor for messageControls.
		/// Performs lazy load upon first request
		/// </summary>
		public List<Message> MessageControls
		{
			get
			{
				if (_messageControls == null)
				{
					_messageControls = new List<Message>();
				}
				return _messageControls;
			}
		}


		private GenericResourceBundle _messages = null;

		/// <summary>
		/// Accessor for messages.
		/// Performs lazy load upon first request
		/// </summary>
		public GenericResourceBundle Messages
		{
			get
			{
				if (_messages == null)
				{
					_messages = new GenericResourceBundle();
				}
				return _messages;
			}
		}

		public override BaseGadgetControl AddControl(BaseGadgetControl control)
		{
            if (null == control)
            {
                return null;
            }

			base.AddControl(control);
			
			if (control is Message)
            {
				Messages.AddString(((Message)control).Name, ((Message)control).Value);
				MessageControls.Add((Message)control);
            }

            return control;
        }
    }
}

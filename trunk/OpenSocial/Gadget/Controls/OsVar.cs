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
using Negroni.OpenSocial.Gadget.Controls; 
using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.Gadget.Controls
{
    [MarkupTag("os:Var")]
	[MarkupTag("os:SetVar")]
	[OffsetKey("os_Var")]
	[ContextGroup(typeof(DataScript))]
	[ContextGroup(typeof(ContentBlock))]
	[ContextGroup(typeof(BaseContainerControl))]
	public class OsVar : VariableTag
    {
        public OsVar()
        {
			this.MyParseContext = new ParseContext(typeof(BaseContainerControl));
        }

        public OsVar(string markup)
            : this()
        {
            LoadTag(markup);
        }

		public OsVar(string markup, OffsetItem offset)
            : this()
        {
            this.MyOffset = offset;

            LoadTag(markup);
        }
    }
}

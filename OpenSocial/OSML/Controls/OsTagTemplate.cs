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
using Negroni.OpenSocial.Gadget.Controls; using Negroni.TemplateFramework;

namespace Negroni.OpenSocial.OSML.Controls
{
	/// <summary>
	/// Specialized template which is a definition of a custom tag
	/// </summary>
	[MarkupTag("script")]
	[ContextGroup(typeof(ContentBlock))]
	[ContextGroupContainer(true)]
	[AttributeTagDependent(CustomTagTemplate.ATTRIBUTE_TAGDEF)]
	[AttributeTagDependent("type", "text/os-template")]
	[OffsetKey("TagTemplate")]
	public class OsTagTemplate : CustomTagTemplate
	{

		public OsTagTemplate() 
		{
			MyTemplate = this;
			MyParseContext = new ParseContext(typeof(ContentBlock));
		}

		public OsTagTemplate(string tag) : this()
		{
			this.Tag = tag;
		}

		public OsTagTemplate(string tag, string markup, string controlFactoryKey)
			: this(tag)
		{
			this.MyControlFactory = ControlFactory.GetControlFactory(controlFactoryKey);
			LoadTag(markup);
		}
		public OsTagTemplate(string tag, string markup, ControlFactory controlFactory)
			: this(tag)
		{
			this.MyControlFactory = controlFactory;
			this.MyRootMaster.ReconfirmControlFactorySet(MyControlFactory);
			LoadTag(markup);
		}

		public OsTagTemplate(string tag, string markup, OffsetItem thisOffset, string controlFactoryKey)
			: this(tag)
		{
			this.MyControlFactory = ControlFactory.GetControlFactory(controlFactoryKey);
			this.MyRootMaster.ReconfirmControlFactorySet(MyControlFactory);
			MyOffset = thisOffset;
			LoadTag(markup);
		}

		public OsTagTemplate(string tag, string markup, OffsetItem thisOffset, ControlFactory controlFactory)
			: this(tag)
		{
			this.MyControlFactory = controlFactory;
			this.MyRootMaster.ReconfirmControlFactorySet(MyControlFactory);
			MyOffset = thisOffset;
			LoadTag(markup);
		}


	}
}
